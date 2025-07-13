using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoeLib;
using PoeLib.Tools;
using TradeBotLib;
using PoeTradeMonitor.Service.Interfaces;
using System.Threading;

namespace PoeTradeMonitor.Service.Services;

public class TradeExecutorService : ITradeExecutorService
{
    private readonly ILogger<TradeExecutorService> log;
    private readonly PoeProxyService poeProxy;
    private readonly ITradeCommands tradeCommands;
    private readonly ITradeBotStateMachine stateMachine;
    private readonly INotificationClient notificationClient;
    private readonly ConcurrentBag<string> characterInHideout = new ConcurrentBag<string>();

    public TradeExecutorService(ITradeCommands tc, ITradeBotStateMachine sm, IPoeChatWatcher cw, INotificationClient nc, ILogger<TradeExecutorService> log, PoeProxyService poeProxy)
    {
        tradeCommands = tc;
        stateMachine = sm;
        cw.JoinedArea += OnJoinedArea;
        cw.LeftArea += OnLeftArea;
        notificationClient = nc;
        this.log = log;
        this.poeProxy = poeProxy;
    }

    private void OnJoinedArea(string characterName)
    {
        characterInHideout.Add(characterName);
        log.LogInformation($"Character {characterName} joined the area");
    }

    private void OnLeftArea(string characterName)
    {
        characterInHideout.TryTake(out var removed);
        log.LogInformation($"Character {characterName} left the area");
    }

    public async Task ExecuteItemTrade(ItemTradeRequest tradeRequest, CancellationToken ct = default)
    {
        try
        {
            var characterName = tradeRequest.CharacterName;

            log.LogInformation("Accepting party Invite");
            await stateMachine.ChangeState(Trigger.JoinParty, ct);
            if (!await tradeCommands.AcceptParty(tradeRequest.CharacterName))
            {
                log.LogInformation("Failed to accept party invite");
                await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                return;
            }

            try
            {
                if (!await tradeCommands.WaitForHideout(60000))
                {
                    log.LogInformation($"Timeout waiting to go to hideout");
                    await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                    await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                    throw new TradeFailureException();
                }

                await tradeCommands.OpenStash(ct);
                await tradeCommands.RemoveInventoryItems();
                await tradeCommands.WithdrawCurrency(tradeRequest.Price.Currencies);

                await stateMachine.GotoPlayerHideout(characterName, ct);
                log.LogInformation($"Waiting for {characterName} to join area");
                if (!await tradeCommands.WaitPlayerJoinArea(characterName, 30000, ct))
                {
                    log.LogInformation($"Timeout waiting for {characterName} to join area");
                    await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                    await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                    throw new TradeFailureException();
                }

                bool tradeSuccessful = false;
                for (int i = 0; i < 3; i++)
                {
                    if (await tradeCommands.PlayerLeftParty(characterName, ct))
                    {
                        await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                        await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                        throw new TradeFailureException();
                    }

                    // Opening trade window
                    await stateMachine.OpenTrade(characterName, ct);
                    log.LogInformation($"Waiting for trade window to open");
                    if (!await tradeCommands.WaitTradeWindowOpen(characterName, 20000, ct))
                    {
                        if (await tradeCommands.PlayerLeftParty(characterName, ct))
                        {
                            await stateMachine.ChangeState(Trigger.ItemTradeInviteTimeout, ct);
                            await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                            await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                            throw new TradeFailureException();
                        }

                        log.LogInformation($"Timeout waiting for {characterName} to accept trade request");
                        await stateMachine.ChangeState(Trigger.ItemTradeInviteTimeout, ct);
                        await stateMachine.OpenTrade(characterName, ct);
                        if (!await tradeCommands.WaitTradeWindowOpen(characterName, 20000, ct))
                        {
                            log.LogInformation($"Timeout waiting for {characterName} to accept trade request");
                            await stateMachine.ChangeState(Trigger.ItemTradeInviteTimeout, ct);
                            await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                            await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                            throw new TradeFailureException();
                        }
                    }
                    await stateMachine.ChangeState(Trigger.TradeInviteAccepted);

                    try
                    {
                        // Putting my currency into trade window
                        await tradeCommands.RemoveInventoryCurrency(tradeRequest.Price.Currencies, true, false);
                        await tradeCommands.WaitForItem(tradeRequest, 60000, ct);
                    }
                    catch (TradeTimeoutException ex)
                    {
                        log.LogInformation(ex.Message);
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        break;
                    }
                    catch (WrongItemException ex)
                    {
                        log.LogInformation(ex.Message);
                        switch(i)
                        { 
                            case 0:
                                await tradeCommands.SendMessage(characterName, "wrong item", true);
                                break;
                            case 1:
                                await tradeCommands.SendMessage(characterName, "still wrong", true);
                                break;
                        }
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        continue;
                    }
                    catch (RemoveCurrencyFailedException ex)
                    {
                        log.LogInformation(ex.Message);
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                        await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                        throw new TradeFailureException();
                    }
                    catch (AttemptedScamException ex)
                    {
                        log.LogInformation(ex.Message);
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                        await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                        throw new TradeFailureException();
                    }
                    catch (TradeClosedException ex)
                    {
                        log.LogInformation(ex.Message);
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        continue;
                    }
                    await stateMachine.ChangeState(Trigger.AcceptTrade, ct);

                    // Waiting to complete trade
                    if (!await tradeCommands.WaitCompleteTrade(characterName, 60000, tradeRequest, ct))
                    {
                        log.LogInformation("Failed to complete trade");
                        await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                        await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                        await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                        throw new TradeFailureException();
                    }

                    tradeSuccessful = await tradeCommands.CheckItemReceived(tradeRequest);
                    if (tradeSuccessful)
                    {
                        await stateMachine.ChangeState(Trigger.CompleteItemTrade, ct);
                        await tradeCommands.SendMessage(characterName, "ty", true);
                        break;
                    }

                    await stateMachine.ChangeState(Trigger.CancelItemTrade, ct);
                }

                if (!tradeSuccessful)
                {
                    await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                    await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                    throw new TradeFailureException();
                }

                log.LogInformation($"Successfully bought {tradeRequest.Item.Name} for {tradeRequest.Price}");
                if (tradeRequest.Item.stackSize == 1 && tradeRequest.Item.rarity != Rarity.Currency && !tradeRequest.Item.descrText.Contains("bestiary"))
                {
                    await notificationClient.SendPushNotification("Trade Successful", "Trade Successful", $"Successfully bought {tradeRequest.Item.Name} for {tradeRequest.Price}", "hello.caf");
                }

                // Leave party
                await stateMachine.ChangeState(Trigger.GotoHideout, ct);
                await stateMachine.ChangeState(Trigger.LeaveParty, ct);

                // putting item back in bank
                await tradeCommands.OpenStash(ct);
                await tradeCommands.RemoveInventoryItems();
                await tradeCommands.SelectTab("$");
            }
            catch (FailedEnterHideoutException)
            {
                await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                await tradeCommands.OpenStash(ct);
                await tradeCommands.RemoveInventoryCurrency();
                log.LogInformation($"Failed to trade {tradeRequest.Item.Name} with {tradeRequest.CharacterName}");
            }
            catch (InsufficientCurrencyException ex)
            {
                await stateMachine.ChangeState(Trigger.LeaveParty, ct);
                log.LogError(ex.Message);
            }
            catch (TradeFailureException)
            {
                await tradeCommands.OpenStash(ct);
                await tradeCommands.RemoveInventoryCurrency();
                log.LogInformation($"Failed to trade {tradeRequest.Item.Name} with {tradeRequest.CharacterName}");
            }
        }
        catch (InvalidOperationException ex)
        {
            log.LogError($"Fatal error: {ex}");
            var characterMessage = new PoeTradeMonitorProto.CharacterMessage { Character = "Me", Message = "Fatal Bot Error", Source = PoeTradeMonitorProto.CharacterMessage.Types.MessageSource.Me };
            await poeProxy.AddCharacterMessage(new PoeTradeMonitorProto.AddCharacterMessageRequest { Message = characterMessage }, null);
        }
    }
}

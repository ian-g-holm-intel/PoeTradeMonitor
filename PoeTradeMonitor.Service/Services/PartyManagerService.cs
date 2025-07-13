using Grpc.Core;
using PoeLib.Tools;
using PoeHudWrapper;
using PoeTradeMonitor.Services.Interfaces;
using PoeTradeMonitorProto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeBotLib;

namespace PoeTradeMonitor.Service.Services;

public class PartyManagerService : PartyManager.PartyManagerBase
{
    private readonly ILogger<PartyManagerService> log;
    private readonly IPoeHudWrapper poeHud;
    private readonly ITradeCommands tradeCommands;
    private readonly ITradeBot tradeBot;
    private readonly ICallbackClient callback;
    private CancellationTokenSource ctSource;
    private ConcurrentDictionary<StartRequest, DateTime> charactersWaitList;
    private ConcurrentDictionary<string, DateTime> charactersSkipList;
    private bool running;

    public PartyManagerService(IPoeChatWatcher chatWatcher, IPoeHudWrapper poeHud, ITradeCommands tradeCommands, ICallbackClient callback, ITradeBot tradeBot, ILogger<PartyManagerService> log)
    {
        this.poeHud = poeHud;
        this.tradeCommands = tradeCommands;
        this.callback = callback;
        this.tradeBot = tradeBot;
        this.log = log;

        charactersWaitList = new ConcurrentDictionary<StartRequest, DateTime>();
        charactersSkipList = new ConcurrentDictionary<string, DateTime>();

        chatWatcher.MessageToCharacter += message =>
        {
            charactersSkipList[message.Character] = message.Timestamp;
            Task.Run(() => Task.Delay(TimeSpan.FromMinutes(2)).ContinueWith(task => charactersSkipList.TryRemove(message.Character, out var _)));
        };
    }

    public override Task<StartReply> Start(StartRequest request, ServerCallContext context)
    {
        charactersWaitList[request] = DateTime.Now;
        Task.Run(() => Task.Delay(TimeSpan.FromMinutes(2)).ContinueWith(task => charactersWaitList.TryRemove(request, out var _)));

        if (!running)
        {
            log.LogInformation("Starting");
            running = true;

            ctSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    while (!ctSource.Token.IsCancellationRequested)
                    {
                        if (!tradeBot.IsExecutingTrade && !poeHud.PlayerInParty() && poeHud.PartyInvites.Any())
                        {
                            await Task.Delay(500);
                            var invite = poeHud.PartyInvites.OrderBy(invite => charactersWaitList.Keys.SingleOrDefault(request => request.CharacterName == invite.CharacterName || request.AccountName == invite.AccountName)?.Value ?? 0).FirstOrDefault();
                            if(invite != null)
                            {
                                var accountName = invite.AccountName;
                                var characterName = invite.CharacterName;
                                
                                await tradeCommands.LeftClickMouse(invite.AcceptButtonLocation);
                                await Task.Delay(500);
                                var partyMembers = poeHud.PartyMemberNames;
                                log.LogInformation($"Joined party with members: {partyMembers.Aggregate("", (current, member) => current + member + ", ").TrimEnd(',', ' ')}");

                                var request = charactersWaitList.Keys.SingleOrDefault(request => request.AccountName == invite.AccountName || partyMembers.Contains(request.CharacterName));
                                if (request != null)
                                {
                                    await callback.JoinedPartyAsync(request.AccountName, request.AccountName == invite.AccountName ? invite.CharacterName : request.CharacterName);
                                    charactersWaitList.TryRemove(request, out var _);
                                }
                                else if (!charactersSkipList.Keys.Contains(characterName))
                                {
                                    log.LogInformation($"Leaving party with unknown player {characterName}({accountName})");
                                    await tradeCommands.LeaveParty();
                                }
                                else
                                {
                                    log.LogInformation($"Staying in party with {characterName}({accountName}) for manual trade");
                                }
                            }
                        }

                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Unhandled exception in party manager {ex}");
                }
                finally
                {
                    log.LogInformation("Stopped");
                    running = false;
                }
            });
            return Task.FromResult(new StartReply { Started = true });
        }
        return Task.FromResult(new StartReply { Started = false });
    }

    public override Task<StopReply> Stop(StopRequest request, ServerCallContext context)
    {
        if (running)
        {
            ctSource.Cancel();
            return Task.FromResult(new StopReply { Stopped = true });
        }
        return Task.FromResult(new StopReply { Stopped = false });
    }

    public override async Task<LeavePartyReply> LeaveParty(LeavePartyRequest request, ServerCallContext context)
    {
        log.LogInformation("LeaveParty");
        await tradeCommands.LeaveParty();
        return new LeavePartyReply();
    }

    public override Task<GetPartyMembersReply> GetPartyMembers(GetPartyMembersRequest request, ServerCallContext context)
    {
        log.LogInformation("GetPartyMembers");
        var reply = new GetPartyMembersReply();
        foreach(var member in poeHud.PartyMemberNames)
            reply.PartyMembers.Add(member);
        return Task.FromResult(reply);
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed;
    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                if (running)
                {
                    ctSource.Cancel();
                    while (running)
                        Thread.Sleep(100);
                    ctSource.Dispose();
                }
            }
            
            disposed = true;
        }
    }
    #endregion 
}

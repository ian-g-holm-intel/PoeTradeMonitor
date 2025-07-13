using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoeLib;
using TradeBotLib;
using Stateless;
using PoeTradeMonitor.Service.Interfaces;
using System;

namespace PoeTradeMonitor.Service.Services;

public class TradeBotStateMachine : ITradeBotStateMachine
{
    private readonly ILogger<TradeBotStateMachine> log;
    private readonly StateMachine<State, Trigger> stateMachine;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> tradeWithCharacter;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> gotoPlayersHideout;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> inviteToParty;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> hideoutJoinTimeout;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> kickPlayer;

    public TradeBotStateMachine(ITradeCommands tradeCommands, ILogger<TradeBotStateMachine> log)
    {
        this.log = log;
        stateMachine = new StateMachine<State, Trigger>(State.Hideout);

        tradeWithCharacter = stateMachine.SetTriggerParameters<string>(Trigger.InitiateTrade);
        inviteToParty = stateMachine.SetTriggerParameters<string>(Trigger.InviteToParty);
        hideoutJoinTimeout = stateMachine.SetTriggerParameters<string>(Trigger.PlayerHideoutJoinTimeout);
        kickPlayer = stateMachine.SetTriggerParameters<string>(Trigger.KickPlayer);
        gotoPlayersHideout = stateMachine.SetTriggerParameters<string>(Trigger.GotoPartyHideout);

        stateMachine.Configure(State.Hideout)
            .OnEntry(() => log.LogInformation("Entering Hideout"))
            .OnEntryFromAsync(kickPlayer, tradeCommands.RemoveFromParty)
            .OnEntryFromAsync(hideoutJoinTimeout, tradeCommands.RemoveFromParty)
            .OnEntryFromAsync(Trigger.LeaveParty, tradeCommands.LeaveParty)
            .OnEntryFromAsync(kickPlayer, tradeCommands.RemoveFromParty)
            .OnExit(() => log.LogInformation("Leaving Hideout"))
            .PermitReentry(Trigger.GotoHideout)
            .Permit(Trigger.JoinParty, State.HomeWithPlayerAway)
            .Permit(Trigger.InviteToParty, State.PartyInviteSent);

        stateMachine.Configure(State.PartyInviteSent)
            .OnEntry(() => log.LogInformation("Entering PlayerInviteSent"))
            .OnExit(() => log.LogInformation("Leaving PlayerInviteSent"))
            .Permit(Trigger.PlayerJoinedParty, State.HomeWithPlayerAway)
            .Permit(Trigger.KickPlayer, State.Hideout);

        stateMachine.Configure(State.HomeWithPlayerAway)
            .OnEntry(() => log.LogInformation("Entering HomeWithPlayerAway"))
            .OnEntryFromAsync(Trigger.GotoHideout, () => tradeCommands.GotoHideout(""))
            .OnExit(() => log.LogInformation("Leaving HomeWithPlayerAway"))
            .Permit(Trigger.PlayerJoinedHideout, State.HomeWithPlayerInOwnHideout)
            .Permit(Trigger.PlayerHideoutJoinTimeout, State.Hideout)
            .Permit(Trigger.LeaveParty, State.Hideout)
            .Permit(Trigger.KickPlayer, State.Hideout)
            .Permit(Trigger.GotoPartyHideout, State.AwayInPlayersHideout)
            .PermitReentry(Trigger.GotoHideout);


        stateMachine.Configure(State.HomeWithPlayerInOwnHideout)
            .OnEntry(() => log.LogInformation("Entering HomeWithPlayerInOwnHideout"))
            .OnEntryFromAsync(Trigger.CancelCurrencyTrade, tradeCommands.CloseAllPanels)
            .OnExit(() => log.LogInformation("Leaving HomeWithPlayerInOwnHideout"))
            .Permit(Trigger.InitiateTrade, State.TradeInviteSent)
            .Permit(Trigger.LeaveParty, State.Hideout)
            .Permit(Trigger.KickPlayer, State.Hideout);

        stateMachine.Configure(State.AwayInPlayersHideout)
            .OnEntry(() => log.LogInformation("Enterting AwayInPlayersHideout"))
            .OnEntryFromAsync(gotoPlayersHideout, async characterName =>
            {
                using var ctSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                if (!await tradeCommands.GotoHideout(characterName, ctSource.Token))
                    throw new FailedEnterHideoutException(characterName);
            })
            .OnEntryFromAsync(Trigger.CancelItemTrade, async () => { await tradeCommands.CloseAllPanels(); await Task.Delay(2000); })
            .OnEntryFromAsync(Trigger.ItemTradeInviteTimeout, tradeCommands.CloseAllPanels)
            .OnExit(() => log.LogInformation("Leaving AwayInPlayersHideout"))
            .Permit(Trigger.InitiateTrade, State.TradeInviteSent)
            .Permit(Trigger.GotoHideout, State.HomeWithPlayerAway)
            .Permit(Trigger.LeaveParty, State.Hideout);
        
        stateMachine.Configure(State.TradeInviteSent)
            .OnEntry(() => log.LogInformation("Entering TradeInviteSent"))
            .OnEntryFromAsync(tradeWithCharacter, tradeCommands.TradeWith)
            .OnExit(() => log.LogInformation("Leaving TradeInviteSent"))
            .Permit(Trigger.TradeInviteAccepted, State.TradeOpened)
            .Permit(Trigger.ItemTradeInviteTimeout, State.AwayInPlayersHideout)
            .Permit(Trigger.CurrencyTradeInviteTimeout, State.HomeWithPlayerInOwnHideout)
            .Permit(Trigger.CancelCurrencyTrade, State.HomeWithPlayerInOwnHideout);


        stateMachine.Configure(State.TradeOpened)
            .OnEntry(() => log.LogInformation("Entering TradeOpened"))
            .OnExit(() => log.LogInformation("Leaving TradeOpened"))
            .Permit(Trigger.AcceptTrade, State.TradeAccepted)
            .Permit(Trigger.CancelItemTrade, State.AwayInPlayersHideout)
            .Permit(Trigger.CancelCurrencyTrade, State.HomeWithPlayerInOwnHideout);

        stateMachine.Configure(State.TradeAccepted)
            .OnEntry(() => log.LogInformation("Entering TradeAccepted"))
            .OnExit(() => log.LogInformation("Leaving TradeAccepted"))
            .Permit(Trigger.CompleteCurrencyTrade, State.HomeWithPlayerAway)
            .Permit(Trigger.CompleteItemTrade, State.AwayInPlayersHideout)
            .Permit(Trigger.CancelItemTrade, State.AwayInPlayersHideout)
            .Permit(Trigger.CancelCurrencyTrade, State.HomeWithPlayerInOwnHideout);
    }

    public async Task ChangeState(Trigger trigger, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(trigger);
    }

    public async Task GotoPlayerHideout(string character, CancellationToken ct = default)
    {
        log.LogInformation($"Going to {character}'s hideout");
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(gotoPlayersHideout, character);
    }

    public async Task OpenTrade(string character, CancellationToken ct = default)
    {
        log.LogInformation($"Opening trade with {character}");
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(tradeWithCharacter, character);
    }

    public async Task InviteToParty(string character, CancellationToken ct = default)
    {
        log.LogInformation($"Inviting {character} to party");
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(inviteToParty, character);
    }

    public async Task HideoutJoinTimeout(string character, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(hideoutJoinTimeout, character);
    }

    public async Task KickPlayer(string character, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await stateMachine.FireAsync(kickPlayer, character);
    }
}
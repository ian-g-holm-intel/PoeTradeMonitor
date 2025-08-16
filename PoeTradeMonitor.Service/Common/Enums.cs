namespace PoeTradeMonitor.Service.Common;

public enum State
{
    Town,
    Hideout,
    CharacterInfoOpen,
    PartyInviteSent,
    HomeWithPlayerAway,
    HomeWithPlayerInOwnHideout,
    AwayInPlayersHideout,
    BothInPlayersHideout,
    TradeInviteSent,
    TradeOpened,
    TradeAccepted
}

public enum Trigger
{
    GotoHideout,
    GotoTown,
    GotoPartyHideout,
    InviteToParty,
    PlayerJoinedParty,
    LeaveParty,
    KickPlayer,
    PlayerJoinedHideout,
    PlayerHideoutJoinTimeout,
    TradeInviteAccepted,
    ItemTradeInviteTimeout,
    CurrencyTradeInviteTimeout,
    InitiateTrade,
    AcceptTrade,
    CompleteCurrencyTrade,
    CompleteItemTrade,
    CancelCurrencyTrade,
    PlayerJoinAreaTimeout,
    JoinParty,
    CancelItemTrade
}

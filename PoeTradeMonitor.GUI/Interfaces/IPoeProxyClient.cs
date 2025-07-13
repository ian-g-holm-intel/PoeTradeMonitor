namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeProxyClient
{
    Task SendMessageAsync(string Message, string clientName);
    Task AntiAFK(string clientName);
    Task<bool> CheckPartyInviteAsync(string characterName, int timeoutMs, string clientName);
    Task AddCharacterMessageAsync(PoeLib.CharacterMessage message, string clientName);
    Task AddTradeRequestAsync(PoeLib.ItemTradeRequest tradeRequest, string clientName);
    Task SetAutoReplyAsync(bool enabled, string clientName);
}

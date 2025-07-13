namespace PoeTradeMonitor.GUI.Interfaces;

public interface ITradeBotClient
{
    Task QueueTrade(PoeLib.ItemTradeRequest tradeRequest, string clientName);
    Task<bool> ExecutingTrade(string clientName);
}

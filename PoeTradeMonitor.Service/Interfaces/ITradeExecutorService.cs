using PoeLib.Common;

namespace PoeTradeMonitor.Service.Interfaces;

public interface ITradeExecutorService
{
    Task ExecuteItemTrade(ItemTradeRequest tradeRequest, CancellationToken ct = default);
}

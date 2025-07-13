using PoeLib;
using System.Threading;
using System.Threading.Tasks;

namespace PoeTradeMonitor.Service.Interfaces;

public interface ITradeExecutorService
{
    Task ExecuteItemTrade(ItemTradeRequest tradeRequest, CancellationToken ct = default);
}

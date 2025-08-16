using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemSearchRequestCache : IDisposable
{
    Task<string> LookupId(string league, TradeSearchRequest request);
}

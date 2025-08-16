using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemSearch
{
    Task<TradeSearchResponse?> SearchAsync(string league, TradeSearchRequest request);
    Task<TradeFetchResponse?> FetchItemResults(IEnumerable<string> ids);
}

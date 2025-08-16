using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemLiveSearch : IPoeLiveSearch
{
    string ItemName { get; }
    Task StartAsync(string league, TradeSearchRequest searchRequest, IPoeItemSearchRequestCache itemSearchRequestCache);
    event Action<SearchGuiItem> LiveSearchStopped;
}

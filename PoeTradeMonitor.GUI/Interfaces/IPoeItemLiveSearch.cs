using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.Trade;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemLiveSearch : IPoeLiveSearch
{
    string ItemName { get; }
    Task StartAsync(string league, PoeItemSearchRequest searchRequest, IPoeItemSearchRequestCache itemSearchRequestCache);
    event Action<SearchGuiItem> LiveSearchStopped;
}

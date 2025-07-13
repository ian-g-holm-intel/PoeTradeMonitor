using PoeTradeMonitor.GUI.Services;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface ILiveSearchResultProcessor
{
    void QueueItems(IEnumerable<ItemSearchRequest> items);
}
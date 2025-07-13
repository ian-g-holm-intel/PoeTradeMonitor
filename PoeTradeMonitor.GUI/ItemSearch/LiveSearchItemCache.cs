using PoeLib.GuiDataClasses;

namespace PoeTradeMonitor.GUI.ItemSearch;

public interface ILiveSearchItemCache
{
    int ItemsPerAccount { get; set; }

    void RemoveItem(SearchGuiItem item);
    bool TryAddItem(SearchGuiItem item);
}

public class LiveSearchItemCache : ILiveSearchItemCache
{
    private List<SearchGuiItem> liveSearchCache = new();
    private readonly object cacheLock = new();

    public int ItemsPerAccount { get; set; } = 20;

    public bool TryAddItem(SearchGuiItem item)
    {
        lock (cacheLock)
        {
            if (liveSearchCache.Count < ItemsPerAccount)
            {
                liveSearchCache.Add(item);
                return true;
            }
            return false;
        }
    }

    public void RemoveItem(SearchGuiItem item)
    {
        lock (cacheLock)
        {
            if (liveSearchCache.Contains(item))
                liveSearchCache.Remove(item);
        }
    }
}

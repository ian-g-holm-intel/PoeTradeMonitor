using System.Collections.Concurrent;
using PoeLib.GuiDataClasses;

namespace PoeTradeMonitor.GUI;

public class StatisticsManager : ViewModelBase
{
    private readonly ConcurrentDictionary<SearchGuiItem, ItemStatistics> websocketItemStatistics = new();
    private readonly ConcurrentDictionary<SearchGuiItem, ItemStatistics> processedItemStatistics = new();

    public int TotalProcessedItems => processedItemStatistics.Values.Sum(stats => stats.ItemCount);
    public int TotalProcessedItemsPerHour => processedItemStatistics.Values.Sum(stats => stats.ItemsPerHour);
    public int TotalWebsocketItems => websocketItemStatistics.Values.Sum(stats => stats.ItemCount);
    public int TotalWebsocketItemsPerHour => websocketItemStatistics.Values.Sum(stats => stats.ItemsPerHour);
    public int ItemBacklog => TotalWebsocketItems - TotalProcessedItems;
    public int ItemBacklogRate => TotalWebsocketItemsPerHour - TotalProcessedItemsPerHour;

    public void LogWebsocketItemsReceived(SearchGuiItem searchGuiItem, int itemCount)
    {
        var itemStatistics = websocketItemStatistics.GetOrAdd(searchGuiItem, new ItemStatistics());
        itemStatistics.IncrementItemCount(itemCount);

        RaisePropertyChanged("TotalWebsocketItems");
        RaisePropertyChanged("TotalWebsocketItemsPerHour");
        RaisePropertyChanged("ItemBacklog");
        RaisePropertyChanged("ItemBacklogRate");
    }

    public void LogProcessedItemsReceived(SearchGuiItem searchGuiItem, int itemCount)
    {
        var itemStatistics = processedItemStatistics.GetOrAdd(searchGuiItem, new ItemStatistics());
        itemStatistics.IncrementItemCount(itemCount);

        RaisePropertyChanged("TotalProcessedItems");
        RaisePropertyChanged("TotalProcessedItemsPerHour");
        RaisePropertyChanged("ItemBacklog");
        RaisePropertyChanged("ItemBacklogRate");
    }
}

public class ItemStatistics
{
    private readonly ConcurrentQueue<DateTime> itemUpdates = new();

    private int itemCount;
    public int ItemCount => itemCount;

    public int ItemsPerHour
    {
        get
        {
            while (itemUpdates.TryPeek(out var next) && (DateTime.Now - next) > TimeSpan.FromHours(1))
            {
                itemUpdates.TryDequeue(out var removed);
            }
            return itemUpdates.Count;
        }
    }

    public void IncrementItemCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            itemCount++;
            itemUpdates.Enqueue(DateTime.Now);
        }
    }
}

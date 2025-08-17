using System.Collections.Concurrent;
using PoeTradeMonitor.GUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PoeTradeMonitor.GUI.Services;

public partial class StatisticsManager : ObservableObject
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

        OnPropertyChanged(nameof(TotalWebsocketItems));
        OnPropertyChanged(nameof(TotalWebsocketItemsPerHour));
        OnPropertyChanged(nameof(ItemBacklog));
        OnPropertyChanged(nameof(ItemBacklogRate));
    }

    public void LogProcessedItemsReceived(SearchGuiItem searchGuiItem, int itemCount)
    {
        var itemStatistics = processedItemStatistics.GetOrAdd(searchGuiItem, new ItemStatistics());
        itemStatistics.IncrementItemCount(itemCount);

        OnPropertyChanged(nameof(TotalProcessedItems));
        OnPropertyChanged(nameof(TotalProcessedItemsPerHour));
        OnPropertyChanged(nameof(ItemBacklog));
        OnPropertyChanged(nameof(ItemBacklogRate));
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
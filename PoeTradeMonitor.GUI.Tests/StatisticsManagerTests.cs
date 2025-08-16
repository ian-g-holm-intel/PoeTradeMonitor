using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Tests;

[TestClass]
public class StatisticsManagerTests
{
    private StatisticsManager statisticsManager = null!;
    private SearchGuiItem testItem1 = null!;
    private SearchGuiItem testItem2 = null!;

    [TestInitialize]
    public void Setup()
    {
        statisticsManager = new StatisticsManager();
        testItem1 = new SearchGuiItem("Test Item 1");
        testItem2 = new SearchGuiItem("Test Item 2");
    }

    [TestMethod]
    public void InitialState_ShouldHaveZeroValues()
    {
        Assert.AreEqual(0, statisticsManager.TotalProcessedItems);
        Assert.AreEqual(0, statisticsManager.TotalProcessedItemsPerHour);
        Assert.AreEqual(0, statisticsManager.TotalWebsocketItems);
        Assert.AreEqual(0, statisticsManager.TotalWebsocketItemsPerHour);
        Assert.AreEqual(0, statisticsManager.ItemBacklog);
        Assert.AreEqual(0, statisticsManager.ItemBacklogRate);
    }

    [TestMethod]
    public void LogWebsocketItemsReceived_ShouldIncrementWebsocketCounts()
    {
        statisticsManager.LogWebsocketItemsReceived(testItem1, 5);

        Assert.AreEqual(5, statisticsManager.TotalWebsocketItems);
        Assert.AreEqual(5, statisticsManager.TotalWebsocketItemsPerHour);
        Assert.AreEqual(5, statisticsManager.ItemBacklog); // Since no processed items
        Assert.AreEqual(5, statisticsManager.ItemBacklogRate);
    }

    [TestMethod]
    public void LogProcessedItemsReceived_ShouldIncrementProcessedCounts()
    {
        statisticsManager.LogProcessedItemsReceived(testItem1, 3);

        Assert.AreEqual(3, statisticsManager.TotalProcessedItems);
        Assert.AreEqual(3, statisticsManager.TotalProcessedItemsPerHour);
        Assert.AreEqual(-3, statisticsManager.ItemBacklog); // Negative since more processed than received
        Assert.AreEqual(-3, statisticsManager.ItemBacklogRate);
    }

    [TestMethod]
    public void LogBothWebsocketAndProcessed_ShouldCalculateBacklogCorrectly()
    {
        statisticsManager.LogWebsocketItemsReceived(testItem1, 10);
        statisticsManager.LogProcessedItemsReceived(testItem1, 6);

        Assert.AreEqual(10, statisticsManager.TotalWebsocketItems);
        Assert.AreEqual(6, statisticsManager.TotalProcessedItems);
        Assert.AreEqual(4, statisticsManager.ItemBacklog); // 10 - 6 = 4
        Assert.AreEqual(4, statisticsManager.ItemBacklogRate); // 10 - 6 = 4
    }

    [TestMethod]
    public void LogMultipleItems_ShouldAggregateCorrectly()
    {
        statisticsManager.LogWebsocketItemsReceived(testItem1, 5);
        statisticsManager.LogWebsocketItemsReceived(testItem2, 3);
        statisticsManager.LogProcessedItemsReceived(testItem1, 2);
        statisticsManager.LogProcessedItemsReceived(testItem2, 1);

        Assert.AreEqual(8, statisticsManager.TotalWebsocketItems); // 5 + 3
        Assert.AreEqual(3, statisticsManager.TotalProcessedItems); // 2 + 1
        Assert.AreEqual(5, statisticsManager.ItemBacklog); // 8 - 3
    }

    [TestMethod]
    public void LogSameItemMultipleTimes_ShouldAccumulate()
    {
        statisticsManager.LogWebsocketItemsReceived(testItem1, 2);
        statisticsManager.LogWebsocketItemsReceived(testItem1, 3);
        statisticsManager.LogProcessedItemsReceived(testItem1, 1);
        statisticsManager.LogProcessedItemsReceived(testItem1, 2);

        Assert.AreEqual(5, statisticsManager.TotalWebsocketItems); // 2 + 3
        Assert.AreEqual(3, statisticsManager.TotalProcessedItems); // 1 + 2
        Assert.AreEqual(2, statisticsManager.ItemBacklog); // 5 - 3
    }

    [TestMethod]
    public void LogZeroItems_ShouldNotChangeValues()
    {
        statisticsManager.LogWebsocketItemsReceived(testItem1, 5);
        var initialWebsocket = statisticsManager.TotalWebsocketItems;

        statisticsManager.LogWebsocketItemsReceived(testItem1, 0);

        Assert.AreEqual(initialWebsocket, statisticsManager.TotalWebsocketItems);
    }
}

[TestClass]
public class ItemStatisticsTests
{
    private ItemStatistics itemStatistics = null!;

    [TestInitialize]
    public void Setup()
    {
        itemStatistics = new ItemStatistics();
    }

    [TestMethod]
    public void InitialState_ShouldHaveZeroValues()
    {
        Assert.AreEqual(0, itemStatistics.ItemCount);
        Assert.AreEqual(0, itemStatistics.ItemsPerHour);
    }

    [TestMethod]
    public void IncrementItemCount_ShouldIncreaseItemCount()
    {
        itemStatistics.IncrementItemCount(5);

        Assert.AreEqual(5, itemStatistics.ItemCount);
        Assert.AreEqual(5, itemStatistics.ItemsPerHour);
    }

    [TestMethod]
    public void IncrementItemCount_MultipleTimes_ShouldAccumulate()
    {
        itemStatistics.IncrementItemCount(3);
        itemStatistics.IncrementItemCount(2);

        Assert.AreEqual(5, itemStatistics.ItemCount);
        Assert.AreEqual(5, itemStatistics.ItemsPerHour);
    }

    [TestMethod]
    public void IncrementItemCount_WithZero_ShouldNotChange()
    {
        itemStatistics.IncrementItemCount(3);
        var initialCount = itemStatistics.ItemCount;

        itemStatistics.IncrementItemCount(0);

        Assert.AreEqual(initialCount, itemStatistics.ItemCount);
    }

    [TestMethod]
    public void ItemsPerHour_ShouldReturnCurrentHourCount()
    {
        itemStatistics.IncrementItemCount(10);

        // All items added within the last hour should be counted
        Assert.AreEqual(10, itemStatistics.ItemsPerHour);
    }

    [TestMethod]
    public void ItemCount_ShouldAlwaysIncrement()
    {
        itemStatistics.IncrementItemCount(5);
        itemStatistics.IncrementItemCount(3);

        // ItemCount should be cumulative
        Assert.AreEqual(8, itemStatistics.ItemCount);
    }

    [TestMethod]
    public void IncrementItemCount_WithLargeNumber_ShouldHandleCorrectly()
    {
        itemStatistics.IncrementItemCount(1000);

        Assert.AreEqual(1000, itemStatistics.ItemCount);
        Assert.AreEqual(1000, itemStatistics.ItemsPerHour);
    }
}
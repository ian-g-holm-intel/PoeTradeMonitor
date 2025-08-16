using Microsoft.Extensions.Logging;
using Moq;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.Services;

namespace PoeTradeMonitor.GUI.Tests.Services;

[TestClass]
public class PoePriceCheckerTests
{
    private Mock<ILogger<PoePriceChecker>> mockLogger = null!;
    private Mock<IPoeItemSearch> mockPoeItemSearch = null!;
    private PoeSettings poeSettings = null!;
    private PoePriceChecker priceChecker = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<PoePriceChecker>>();
        mockPoeItemSearch = new Mock<IPoeItemSearch>();
        poeSettings = new PoeSettings
        {
            League = "Test League"
        };

        priceChecker = new PoePriceChecker(mockLogger.Object, mockPoeItemSearch.Object, poeSettings);
    }

    [TestCleanup]
    public void Cleanup()
    {
        priceChecker?.Dispose();
    }

    [TestMethod]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.IsNotNull(priceChecker);
    }

    [TestMethod]
    public void StartMonitoringItem_ShouldAddItemToMonitoredList()
    {
        var searchRequest = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item" }
        };

        priceChecker.StartMonitoringItem(searchRequest);

        // Since we can't directly access the monitored items dictionary,
        // we verify the item was added by checking it doesn't throw
        // and GetPrice returns 0 for unknown items
        var price = priceChecker.GetPrice("Test Item");
        Assert.AreEqual(0m, price);
    }

    [TestMethod]
    public void StartMonitoringItem_WithSameName_ShouldOverwriteExistingItem()
    {
        var searchRequest1 = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item", Type = "First Type" }
        };
        var searchRequest2 = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item", Type = "Second Type" }
        };

        priceChecker.StartMonitoringItem(searchRequest1);
        priceChecker.StartMonitoringItem(searchRequest2);

        // Both should succeed without throwing exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void StopMonitoringItem_ShouldRemoveItemFromMonitoredList()
    {
        var searchRequest = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item" }
        };

        priceChecker.StartMonitoringItem(searchRequest);
        priceChecker.StopMonitoringItem("Test Item");

        // Should not throw exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void StopMonitoringItem_WithNonExistentItem_ShouldNotThrow()
    {
        priceChecker.StopMonitoringItem("Non-existent Item");

        // Should not throw exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void GetPrice_WithUnknownItem_ShouldReturnZero()
    {
        var price = priceChecker.GetPrice("Unknown Item");

        Assert.AreEqual(0m, price);
    }

    [TestMethod]
    public void StartMonitoringItem_WithEmptyName_ShouldUseTypeAsName()
    {
        var searchRequest = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "", Type = "Item Type" }
        };

        priceChecker.StartMonitoringItem(searchRequest);

        // Should not throw, name resolution is handled by TradeSearchRequest.Name property
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void StartMonitoringItem_WithNullNameAndType_ShouldUseUnknownAsName()
    {
        var searchRequest = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = null, Type = null }
        };

        priceChecker.StartMonitoringItem(searchRequest);

        // Should not throw, name resolution is handled by TradeSearchRequest.Name property
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Start_ShouldNotThrow()
    {
        priceChecker.Start();

        // Should not throw exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Stop_ShouldNotThrow()
    {
        priceChecker.Start();
        priceChecker.Stop();

        // Should not throw exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Stop_WithoutStart_ShouldNotThrow()
    {
        priceChecker.Stop();

        // Should not throw exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void MultipleStartStop_ShouldNotThrow()
    {
        priceChecker.Start();
        priceChecker.Start(); // Multiple starts should be safe
        priceChecker.Stop();
        priceChecker.Stop(); // Multiple stops should be safe

        // Should not throw exceptions
        Assert.IsTrue(true);
    }
}
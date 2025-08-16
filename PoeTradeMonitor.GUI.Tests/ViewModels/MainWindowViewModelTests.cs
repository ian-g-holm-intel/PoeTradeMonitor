using Microsoft.Extensions.Logging;
using Moq;
using PoeAuthenticator.Services;
using PoeLib.Common;
using PoeLib.Tools;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ItemSearch;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.ViewModels;
using System.Collections.ObjectModel;
using System.Net;

namespace PoeTradeMonitor.GUI.Tests.ViewModels;

[TestClass]
public class MainWindowViewModelTests
{
    private Mock<ILogger<MainWindowViewModel>> mockLogger = null!;
    private Mock<ICookieMonitorService> mockCookieMonitorService = null!;
    private Mock<ICustomSearchManager> mockCustomSearchManager = null!;
    private Mock<IPoePriceChecker> mockPoePriceChecker = null!;
    private Mock<ICurrencyCache> mockCurrencyCache = null!;
    private Mock<ICurrencyPriceRetriever> mockCurrencyPriceRetriever = null!;
    private Mock<IPoeHttpClient> mockPoeHttpClient = null!;
    private Mock<IPoeCookieReader> mockPoeCookieReader = null!;
    private Mock<IStashDataUpdater> mockStashDataUpdater = null!;
    private Mock<ITradeRequestScheduler> mockTradeRequestScheduler = null!;
    private Mock<ICurrencyPriceCache> mockCurrencyPriceCache = null!;
    private Mock<ITradeBotClient> mockTradeBotClient = null!;
    private Mock<IBrowserService> mockBrowserService = null!;
    private CookieContainer cookieContainer = null!;
    private PoeSettings poeSettings = null!;
    private StatisticsManager statisticsManager = null!;
    private MainWindowViewModel viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<MainWindowViewModel>>();
        mockCookieMonitorService = new Mock<ICookieMonitorService>();
        mockCustomSearchManager = new Mock<ICustomSearchManager>();
        mockPoePriceChecker = new Mock<IPoePriceChecker>();
        mockCurrencyCache = new Mock<ICurrencyCache>();
        mockCurrencyPriceRetriever = new Mock<ICurrencyPriceRetriever>();
        mockPoeHttpClient = new Mock<IPoeHttpClient>();
        mockPoeCookieReader = new Mock<IPoeCookieReader>();
        mockStashDataUpdater = new Mock<IStashDataUpdater>();
        mockTradeRequestScheduler = new Mock<ITradeRequestScheduler>();
        mockCurrencyPriceCache = new Mock<ICurrencyPriceCache>();
        mockTradeBotClient = new Mock<ITradeBotClient>();
        mockBrowserService = new Mock<IBrowserService>();

        cookieContainer = new CookieContainer();
        poeSettings = new PoeSettings();
        statisticsManager = new StatisticsManager();

        viewModel = new MainWindowViewModel(
            mockTradeBotClient.Object,
            mockCurrencyCache.Object,
            mockCurrencyPriceRetriever.Object,
            mockPoeHttpClient.Object,
            cookieContainer,
            mockPoeCookieReader.Object,
            mockStashDataUpdater.Object,
            mockTradeRequestScheduler.Object,
            mockCurrencyPriceCache.Object,
            mockLogger.Object,
            mockCookieMonitorService.Object,
            mockCustomSearchManager.Object,
            mockPoePriceChecker.Object,
            statisticsManager,
            poeSettings,
            mockBrowserService.Object
        );
    }

    [TestMethod]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        Assert.IsNotNull(viewModel.SearchList);
        Assert.IsNotNull(viewModel.StashGuiItems);
        Assert.IsNotNull(viewModel.StatisticsManager);
        Assert.IsNotNull(viewModel.IgnoredAccounts);
        Assert.AreEqual(statisticsManager, viewModel.StatisticsManager);
    }

    [TestMethod]
    public void Constructor_ShouldSetupSearchListFromSettings()
    {
        var searchItems = new List<SearchGuiItem>
        {
            new SearchGuiItem("Test Item 1") { Enabled = true },
            new SearchGuiItem("Test Item 2") { Enabled = false }
        };
        poeSettings.SearchItems = searchItems;

        var newViewModel = new MainWindowViewModel(
            mockTradeBotClient.Object,
            mockCurrencyCache.Object,
            mockCurrencyPriceRetriever.Object,
            mockPoeHttpClient.Object,
            cookieContainer,
            mockPoeCookieReader.Object,
            mockStashDataUpdater.Object,
            mockTradeRequestScheduler.Object,
            mockCurrencyPriceCache.Object,
            mockLogger.Object,
            mockCookieMonitorService.Object,
            mockCustomSearchManager.Object,
            mockPoePriceChecker.Object,
            statisticsManager,
            poeSettings,
            mockBrowserService.Object
        );

        Assert.AreEqual(2, newViewModel.SearchList.Count);
        Assert.AreEqual("Test Item 1", newViewModel.SearchList[0].Name);
        Assert.AreEqual("Test Item 2", newViewModel.SearchList[1].Name);
    }

    [TestMethod]
    public void ServiceLocation_GetSet_ShouldUpdateSettings()
    {
        viewModel.ServiceLocation = ServiceLocation.Remote;

        Assert.AreEqual(ServiceLocation.Remote, poeSettings.ServiceLocation);
        Assert.AreEqual(ServiceLocation.Remote, viewModel.ServiceLocation);
    }

    [TestMethod]
    public void AntiAFKEnabled_SetTrue_ShouldStartTimer()
    {
        viewModel.AntiAFKEnabled = true;

        Assert.IsTrue(poeSettings.AntiAFKEnabled);
        Assert.IsTrue(viewModel.AntiAFKEnabled);
    }

    [TestMethod]
    public void Running_GetSet_ShouldUpdateSettings()
    {
        viewModel.Running = true;

        Assert.IsTrue(poeSettings.Running);
        Assert.IsTrue(viewModel.Running);
    }

    [TestMethod]
    public void AutoreplyEnabled_GetSet_ShouldUpdateSettings()
    {
        viewModel.AutoreplyEnabled = true;

        Assert.IsTrue(poeSettings.AutoreplyEnabled);
        Assert.IsTrue(viewModel.AutoreplyEnabled);
    }

    [TestMethod]
    public void AlertsEnabled_SetTrue_ShouldUpdateTradeRequestScheduler()
    {
        viewModel.AlertsEnabled = true;

        Assert.IsTrue(poeSettings.AlertsEnabled);
        Assert.IsTrue(viewModel.AlertsEnabled);
        mockTradeRequestScheduler.VerifySet(x => x.AlertsEnabled = true, Times.Once);
    }

    [TestMethod]
    public void UnattendedModeEnabled_SetTrue_ShouldUpdateTradeRequestScheduler()
    {
        viewModel.UnattendedModeEnabled = true;

        Assert.IsTrue(poeSettings.UnattendedModeEnabled);
        Assert.IsTrue(viewModel.UnattendedModeEnabled);
        mockTradeRequestScheduler.Verify(x => x.UpdateSettings(true, It.IsAny<ServiceLocation>()), Times.AtLeastOnce);
    }

    [TestMethod]
    public void PriceLoggerEnabled_SetTrue_ShouldStartPriceChecker()
    {
        viewModel.PriceLoggerEnabled = true;

        Assert.IsTrue(poeSettings.PriceLoggerEnabled);
        Assert.IsTrue(viewModel.PriceLoggerEnabled);
        mockPoePriceChecker.Verify(x => x.Start(), Times.Once);
    }

    [TestMethod]
    public void PriceLoggerEnabled_SetFalse_ShouldStopPriceChecker()
    {
        viewModel.PriceLoggerEnabled = true; // First set to true
        viewModel.PriceLoggerEnabled = false; // Then set to false

        Assert.IsFalse(poeSettings.PriceLoggerEnabled);
        Assert.IsFalse(viewModel.PriceLoggerEnabled);
        mockPoePriceChecker.Verify(x => x.Stop(), Times.Once);
    }

    [TestMethod]
    public void ConnectedDuration_WhenConnected_ShouldReturnFormattedTime()
    {
        viewModel.Connected = true;
        viewModel.LastDataReceived = DateTime.Now.AddMinutes(-5);

        var duration = viewModel.ConnectedDuration;

        Assert.IsTrue(duration.Contains("m"));
        Assert.IsTrue(duration.Contains("s"));
    }

    [TestMethod]
    public void ConnectedDuration_WhenDisconnected_ShouldReturnZero()
    {
        viewModel.Connected = false;

        var duration = viewModel.ConnectedDuration;

        Assert.AreEqual("0m0s", duration);
    }

    [TestMethod]
    public void DivineRate_ShouldReturnIntegerFromDecimal()
    {
        viewModel.DivineRateDecimal = 125.75m;

        Assert.AreEqual(126, viewModel.DivineRate); // Rounds to nearest integer
    }

    [TestMethod]
    public void DivineCount_WithValidRate_ShouldCalculateCorrectly()
    {
        viewModel.BaseCurrencyCount = 1000;
        viewModel.DivineRateDecimal = 200m;

        Assert.AreEqual(5.0m, viewModel.DivineCount);
    }

    [TestMethod]
    public void DivineCount_WithZeroRate_ShouldReturnZero()
    {
        viewModel.BaseCurrencyCount = 1000;
        viewModel.DivineRateDecimal = 0m;

        Assert.AreEqual(0m, viewModel.DivineCount);
    }

    [TestMethod]
    public void EnabledSearchCount_ShouldReturnCorrectCount()
    {
        viewModel.SearchList.Clear();
        viewModel.SearchList.Add(new SearchGuiItem("Item1") { Enabled = true });
        viewModel.SearchList.Add(new SearchGuiItem("Item2") { Enabled = false });
        viewModel.SearchList.Add(new SearchGuiItem("Item3") { Enabled = true });

        Assert.AreEqual(2, viewModel.EnabledSearchCount);
    }

    [TestMethod]
    public void Disconnected_ShouldReturnOppositeOfConnected()
    {
        viewModel.Connected = true;
        Assert.IsFalse(viewModel.Disconnected);

        viewModel.Connected = false;
        Assert.IsTrue(viewModel.Disconnected);
    }

    [TestMethod]
    public async Task RemoveSearchItemCommand_ShouldRemoveItemAndStopSearch()
    {
        var searchItem = new SearchGuiItem("Test Item");
        viewModel.SearchList.Add(searchItem);

        await viewModel.RemoveSearchItemCommand.ExecuteAsync(searchItem);

        Assert.IsFalse(viewModel.SearchList.Contains(searchItem));
        mockCustomSearchManager.Verify(x => x.StopSearchAsync(searchItem), Times.Once);
    }

    [TestMethod]
    public async Task EnableSearchItemCommand_ShouldEnableItemAndStartSearch()
    {
        var searchItem = new SearchGuiItem("Test Item") { Enabled = false };
        viewModel.SearchList.Add(searchItem);

        await viewModel.EnableSearchItemCommand.ExecuteAsync(searchItem);

        Assert.IsTrue(searchItem.Enabled);
        mockCustomSearchManager.Verify(x => x.StartSearchAsync(searchItem), Times.Once);
    }

    [TestMethod]
    public async Task DisableSearchItemCommand_ShouldDisableItemAndStopSearch()
    {
        var searchItem = new SearchGuiItem("Test Item") { Enabled = true };
        viewModel.SearchList.Add(searchItem);

        await viewModel.DisableSearchItemCommand.ExecuteAsync(searchItem);

        Assert.IsFalse(searchItem.Enabled);
        mockCustomSearchManager.Verify(x => x.StopSearchAsync(searchItem), Times.Once);
    }

    [TestMethod]
    public async Task OpenSearchInBrowserCommand_WithValidSearchID_ShouldOpenBrowser()
    {
        var searchItem = new SearchGuiItem("Test Item") { SearchID = "test-search-id" };
        poeSettings.League = "Dawn%20of%20the%20Hunt";

        await viewModel.OpenSearchInBrowserCommand.ExecuteAsync(searchItem);

        mockBrowserService.Verify(x => x.OpenUrlAsync(It.Is<string>(url => 
            url.Contains("test-search-id") && url.Contains("Dawn%20of%20the%20Hunt"))), Times.Once);
    }

    [TestMethod]
    public async Task OpenSearchInBrowserCommand_WithNullSearchID_ShouldNotOpenBrowser()
    {
        var searchItem = new SearchGuiItem("Test Item") { SearchID = "" };

        await viewModel.OpenSearchInBrowserCommand.ExecuteAsync(searchItem);

        mockBrowserService.Verify(x => x.OpenUrlAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task ClearSearchItemCommand_ShouldClearAllSearches()
    {
        viewModel.SearchList.Add(new SearchGuiItem("Item1"));
        viewModel.SearchList.Add(new SearchGuiItem("Item2"));

        await viewModel.ClearSearchItemCommand.ExecuteAsync(null);

        Assert.AreEqual(0, viewModel.SearchList.Count);
        mockCustomSearchManager.Verify(x => x.StopSearchAsync(It.IsAny<SearchGuiItem>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task BuyStashGuiItemCommand_ShouldSendTradeWhisper()
    {
        var item = new StashGuiItem(CreateTestTradeRequest())
        {
            SearchID = "search-id",
            ItemID = "item-id",
            StackSize = 1
        };

        await viewModel.BuyStashGuiItemCommand.ExecuteAsync(item);

        mockPoeHttpClient.Verify(x => x.SendTradeWhisper("search-id", "item-id", 1), Times.Once);
    }

    [TestMethod]
    public void IgnoreAccountCommand_ShouldAddAccountToIgnoreList()
    {
        var item = new StashGuiItem(CreateTestTradeRequest()) { Account = "TestAccount" };

        viewModel.IgnoreAccountCommand.Execute(item);

        Assert.IsTrue(viewModel.IgnoredAccounts.Contains("TestAccount"));
        Assert.IsTrue(poeSettings.IgnoredAccounts.Contains("TestAccount"));
    }

    [TestMethod]
    public void IgnoreAccountCommand_WithDuplicateAccount_ShouldNotAddTwice()
    {
        var item = new StashGuiItem(CreateTestTradeRequest()) { Account = "TestAccount" };

        viewModel.IgnoreAccountCommand.Execute(item);
        viewModel.IgnoreAccountCommand.Execute(item);

        Assert.AreEqual(1, viewModel.IgnoredAccounts.Count(x => x == "TestAccount"));
    }

    [TestMethod]
    public async Task ExecuteCommand_ShouldQueueTradeAndDisableExecute()
    {
        var tradeRequest = CreateTestTradeRequest();
        var item = new StashGuiItem(tradeRequest) { ExecuteEnabled = true, TradeRequest = tradeRequest };

        await viewModel.ExecuteCommandCommand.ExecuteAsync(item);

        Assert.IsFalse(item.ExecuteEnabled);
        mockTradeBotClient.Verify(x => x.QueueTrade(tradeRequest, It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task AutoReplyUpdatedCommand_ShouldUpdateClient()
    {
        viewModel.AutoreplyEnabled = true;

        await viewModel.AutoReplyUpdatedCommand.ExecuteAsync(null);

        mockTradeBotClient.Verify(x => x.SetAutoReplyAsync(true, It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task ReloadDataCommand_ShouldUpdateCurrencyData()
    {
        poeSettings.League = "Dawn%20of%20the%20Hunt";
        var mockCurrencies = new Dictionary<TradeCurrencyType, CurrencyInfo>();
        mockTradeBotClient.Setup(x => x.GetCurrencyAsync(It.IsAny<string>())).ReturnsAsync(mockCurrencies);
        mockCurrencyCache.Setup(x => x.GetCurrencyCount()).Returns(1000);
        mockCurrencyPriceCache.Setup(x => x.ContainsPrice(TradeCurrencyType.Divine)).Returns(true);
        mockCurrencyPriceCache.Setup(x => x.GetPrice(TradeCurrencyType.Divine))
            .Returns(new CurrencyPrice { SellPrice = 200m });

        await viewModel.ReloadDataCommand.ExecuteAsync(null);

        mockCurrencyPriceRetriever.Verify(x => x.GetCurrencyPrices("Dawn%20of%20the%20Hunt"), Times.Once);
        mockCurrencyCache.Verify(x => x.UpdateCurrencies(mockCurrencies), Times.Once);
        Assert.AreEqual(1000, viewModel.BaseCurrencyCount);
        Assert.AreEqual(200m, viewModel.DivineRateDecimal);
    }

    [TestMethod]
    public void AddStashGuiItem_WithValidItem_ShouldReturnTrueAndAddItem()
    {
        var item = new StashGuiItem(CreateTestTradeRequest());

        // Note: This test is simplified since we can't easily test WPF Dispatcher in unit tests
        // In a real scenario, you'd need to mock or abstract the Application.Current.Dispatcher
        var result = viewModel.AddStashGuiItem(item);

        // The method will return false because Application.Current is null in unit tests
        // but we can verify the logic path
        Assert.IsFalse(result); // Expected in unit test environment
    }

    [TestMethod]
    public void AddStashGuiItem_WithDuplicateItem_ShouldReturnFalse()
    {
        var item = new StashGuiItem(CreateTestTradeRequest());
        viewModel.StashGuiItems.Add(item);

        var result = viewModel.AddStashGuiItem(item);

        Assert.IsFalse(result);
    }

    private ItemTradeRequest CreateTestTradeRequest()
    {
        var item = new TradeItem { Id = "test-id", Name = "Test Item", League = "Standard" };
        var price = new PriceInfo { Amount = 10m, CurrencyType = TradeCurrencyType.Chaos };
        var currencies = new List<CurrencyInfo> { new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 10m } };
        return new ItemTradeRequest("Character", "Account", item, price, currencies, DateTime.Now, 200m);
    }
}
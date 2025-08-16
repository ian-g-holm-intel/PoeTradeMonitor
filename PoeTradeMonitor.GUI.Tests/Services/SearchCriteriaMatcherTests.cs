using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTrade.Contracts;
using Moq;

namespace PoeTradeMonitor.GUI.Tests.Services;

[TestClass]
public class SearchCriteriaMatcherTests
{
    private SearchCriteriaMatcher matcher = null!;
    private Mock<IPoePriceChecker> mockPoePriceChecker = null!;
    private SearchGuiItem searchItem = null!;
    private TradeItem tradeItem = null!;
    private PriceInfo price = null!;
    private const decimal DivineRate = 200m;

    [TestInitialize]
    public void Setup()
    {
        mockPoePriceChecker = new Mock<IPoePriceChecker>();
        matcher = new SearchCriteriaMatcher(mockPoePriceChecker.Object);
        
        searchItem = new SearchGuiItem("Test Item")
        {
            SearchID = "search123",
            Source = "GUI",
            Name = "Unique Sword",
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };

        tradeItem = new TradeItem
        {
            SearchID = "search123",
            Name = "Unique Sword of Power",
            BaseType = "Sword"
        };

        price = new PriceInfo { Amount = 80, CurrencyType = TradeCurrencyType.Chaos };
    }

    [TestMethod]
    public void MatchesCriteria_WithGUISourceAndMatchingSearchID_ShouldReturnTrue()
    {
        var testPrice = new PriceInfo { Amount = 50, CurrencyType = TradeCurrencyType.Chaos }; // Less than offer price

        var result = matcher.MatchesCriteria(searchItem, tradeItem, testPrice, DivineRate);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithGUISourceAndEqualPrice_ShouldReturnTrue()
    {
        var testPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }; // Equal to offer price

        var result = matcher.MatchesCriteria(searchItem, tradeItem, testPrice, DivineRate);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithGUISourceAndExcessivePrice_ShouldReturnFalse()
    {
        var testPrice = new PriceInfo { Amount = 150, CurrencyType = TradeCurrencyType.Chaos }; // Higher than offer price

        var result = matcher.MatchesCriteria(searchItem, tradeItem, testPrice, DivineRate);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithNonGUISource_ShouldReturnFalse()
    {
        searchItem.Source = "NonGUI";

        var result = matcher.MatchesCriteria(searchItem, tradeItem, price, DivineRate);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithNonMatchingSearchID_ShouldReturnFalse()
    {
        tradeItem.SearchID = "different-search-id";

        var result = matcher.MatchesCriteria(searchItem, tradeItem, price, DivineRate);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithMatchingAutoSearchID_ShouldReturnTrue()
    {
        searchItem.AutoSearchID = "auto123";
        searchItem.SearchID = ""; // Empty SearchID
        tradeItem.SearchID = "auto123";
        mockPoePriceChecker.Setup(x => x.GetPrice(searchItem.Name)).Returns(120m);

        var testPrice = new PriceInfo { Amount = 80, CurrencyType = TradeCurrencyType.Chaos }; // Should pass auto search criteria

        var result = matcher.MatchesCriteria(searchItem, tradeItem, testPrice, DivineRate);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithAutoSearchIDButZeroPrice_ShouldReturnFalse()
    {
        searchItem.AutoSearchID = "auto123";
        searchItem.SearchID = ""; // Empty SearchID
        tradeItem.SearchID = "auto123";
        mockPoePriceChecker.Setup(x => x.GetPrice(searchItem.Name)).Returns(0m); // No price data

        var result = matcher.MatchesCriteria(searchItem, tradeItem, price, DivineRate);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchesCriteria_WithAutoSearchIDAndHighPrice_ShouldReturnFalse()
    {
        searchItem.AutoSearchID = "auto123";
        searchItem.SearchID = ""; // Empty SearchID
        searchItem.OfferPrice = new PriceInfo { Amount = 20, CurrencyType = TradeCurrencyType.Chaos }; // 0.1 divine
        tradeItem.SearchID = "auto123";
        mockPoePriceChecker.Setup(x => x.GetPrice(searchItem.Name)).Returns(0.3m); // Average price is 0.3 divine

        var testPrice = new PriceInfo { Amount = 50, CurrencyType = TradeCurrencyType.Chaos }; // 0.25 divine, should be > (0.3 - 0.1) = 0.2

        var result = matcher.MatchesCriteria(searchItem, tradeItem, testPrice, DivineRate);

        Assert.IsFalse(result);
    }
}
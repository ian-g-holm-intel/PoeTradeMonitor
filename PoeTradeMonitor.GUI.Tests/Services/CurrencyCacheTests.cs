using Microsoft.Extensions.Logging;
using Moq;
using PoeLib.Common;
using PoeLib.Tools;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Services;

namespace PoeTradeMonitor.GUI.Tests.Services;

[TestClass]
public class CurrencyCacheTests
{
    private Mock<ILogger<CurrencyCache>> mockLogger = null!;
    private Mock<IStashCurrencyRetriever> mockCurrencyRetriever = null!;
    private Mock<ICurrencyPriceCache> mockPriceCache = null!;
    private CurrencyCache currencyCache = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<CurrencyCache>>();
        mockCurrencyRetriever = new Mock<IStashCurrencyRetriever>();
        mockPriceCache = new Mock<ICurrencyPriceCache>();
        currencyCache = new CurrencyCache(mockCurrencyRetriever.Object, mockPriceCache.Object, mockLogger.Object);
    }

    [TestMethod]
    public void GetCurrency_WithExistingType_ShouldReturnCurrency()
    {
        var currencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Chaos, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 100 } }
        };
        currencyCache.UpdateCurrencies(currencies);

        var result = currencyCache.GetCurrency(TradeCurrencyType.Chaos);

        Assert.IsNotNull(result);
        Assert.AreEqual(TradeCurrencyType.Chaos, result.Type);
        Assert.AreEqual(100, result.Amount);
    }

    [TestMethod]
    public void GetCurrency_WithNonExistingType_ShouldReturnZeroAmount()
    {
        var result = currencyCache.GetCurrency(TradeCurrencyType.Divine);

        Assert.IsNotNull(result);
        Assert.AreEqual(TradeCurrencyType.Divine, result.Type);
        Assert.AreEqual(0, result.Amount);
    }

    [TestMethod]
    public void UpdateCurrencies_ShouldReplaceCurrencyDictionary()
    {
        var initialCurrencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Chaos, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 50 } }
        };
        currencyCache.UpdateCurrencies(initialCurrencies);

        var newCurrencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Divine, new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = 10 } }
        };
        currencyCache.UpdateCurrencies(newCurrencies);

        var chaosResult = currencyCache.GetCurrency(TradeCurrencyType.Chaos);
        var divineResult = currencyCache.GetCurrency(TradeCurrencyType.Divine);

        Assert.AreEqual(0, chaosResult.Amount); // Should be cleared
        Assert.AreEqual(10, divineResult.Amount); // Should be new value
    }

    [TestMethod]
    public async Task UpdateCurrenciesAsync_WithEmptyResponse_ShouldNotUpdateCache()
    {
        mockCurrencyRetriever.Setup(x => x.GetStashCurrency("TestLeague"))
            .ReturnsAsync(Array.Empty<TradeItem>());

        await currencyCache.UpdateCurrenciesAsync("TestLeague");

        var result = currencyCache.GetCurrency(TradeCurrencyType.Chaos);
        Assert.AreEqual(0, result.Amount);
    }

    [TestMethod]
    public async Task UpdateCurrenciesAsync_WithValidCurrency_ShouldUpdateCache()
    {
        var currencyItems = new[]
        {
            new TradeItem
            {
                Name = "Chaos Orb",
                StackSize = 100,
                FrameType = (int)ItemRarity.Currency
            }
        };

        mockCurrencyRetriever.Setup(x => x.GetStashCurrency("TestLeague"))
            .ReturnsAsync(currencyItems);

        await currencyCache.UpdateCurrenciesAsync("TestLeague");

        var result = currencyCache.GetCurrency(TradeCurrencyType.Chaos);
        Assert.AreEqual(100, result.Amount);
    }

    [TestMethod]
    public async Task UpdateCurrenciesAsync_WithDuplicateCurrency_ShouldCombineAmounts()
    {
        var currencyItems = new[]
        {
            new TradeItem { Name = "Chaos Orb", StackSize = 50, FrameType = (int)ItemRarity.Currency },
            new TradeItem { Name = "Chaos Orb", StackSize = 30, FrameType = (int)ItemRarity.Currency }
        };

        mockCurrencyRetriever.Setup(x => x.GetStashCurrency("TestLeague"))
            .ReturnsAsync(currencyItems);

        await currencyCache.UpdateCurrenciesAsync("TestLeague");

        var result = currencyCache.GetCurrency(TradeCurrencyType.Chaos);
        Assert.AreEqual(80, result.Amount);
    }

    [TestMethod]
    public void GetCurrencyCount_WithDivineAndBaseCurrency_ShouldCalculateCorrectTotal()
    {
        var currencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Divine, new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = 2 } },
            { Constants.BaseCurrencyType, new CurrencyInfo { Type = Constants.BaseCurrencyType, Amount = 100 } }
        };
        currencyCache.UpdateCurrencies(currencies);

        mockPriceCache.Setup(x => x.GetPrice(TradeCurrencyType.Divine))
            .Returns(new CurrencyPrice { Type = TradeCurrencyType.Divine, SellPrice = 200 });

        var result = currencyCache.GetCurrencyCount();

        Assert.AreEqual(500, result); // 100 + (2 * 200)
    }

    [TestMethod]
    public void ConvertDivFractions_WithDivinePrice_ShouldConvertCorrectly()
    {
        var price = new PriceInfo { Amount = 1.5m, CurrencyType = TradeCurrencyType.Divine };
        var stackSize = 2;
        var divineRate = 200m;

        var result = currencyCache.ConvertDivFractions(price, stackSize, divineRate);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(3m, result.First(c => c.Type == TradeCurrencyType.Divine).Amount);
        Assert.AreEqual(0m, result.First(c => c.Type == Constants.BaseCurrencyType).Amount);
    }

    [TestMethod]
    public void ConvertDivFractions_WithBaseCurrencyPrice_ShouldRoundUp()
    {
        var price = new PriceInfo { Amount = 5.3m, CurrencyType = Constants.BaseCurrencyType };
        var stackSize = 1;
        var divineRate = 200m;

        var result = currencyCache.ConvertDivFractions(price, stackSize, divineRate);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(6m, result[0].Amount); // Ceiling of 5.3
        Assert.AreEqual(Constants.BaseCurrencyType, result[0].Type);
    }

    [TestMethod]
    public void EnoughCurrencyForTrade_WithSufficientCurrency_ShouldReturnTrue()
    {
        var currencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Chaos, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 100 } }
        };
        currencyCache.UpdateCurrencies(currencies);

        var requiredCurrencies = new[]
        {
            new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 50 }
        };

        var result = currencyCache.EnoughCurrencyForTrade(requiredCurrencies);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void EnoughCurrencyForTrade_WithInsufficientCurrency_ShouldReturnFalse()
    {
        var currencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Chaos, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 30 } }
        };
        currencyCache.UpdateCurrencies(currencies);

        var requiredCurrencies = new[]
        {
            new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 50 }
        };

        var result = currencyCache.EnoughCurrencyForTrade(requiredCurrencies);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void EnoughCurrencyForTrade_WithMissingCurrencyType_ShouldReturnFalse()
    {
        var currencies = new Dictionary<TradeCurrencyType, CurrencyInfo>
        {
            { TradeCurrencyType.Chaos, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 100 } }
        };
        currencyCache.UpdateCurrencies(currencies);

        var requiredCurrencies = new[]
        {
            new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = 1 }
        };

        var result = currencyCache.EnoughCurrencyForTrade(requiredCurrencies);

        Assert.IsFalse(result);
    }
}
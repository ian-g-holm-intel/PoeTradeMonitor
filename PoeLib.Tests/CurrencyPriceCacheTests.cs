using PoeLib.Common;
using PoeLib.Tools;
using PoeTrade.Contracts;

namespace PoeLib.Tests;

[TestClass]
public class CurrencyPriceCacheTests
{
    private CurrencyPriceCache cache = null!;
    private List<CurrencyPrice> testPrices = null!;

    [TestInitialize]
    public void Setup()
    {
        cache = new CurrencyPriceCache();
        testPrices = new List<CurrencyPrice>
        {
            new CurrencyPrice { Type = TradeCurrencyType.Chaos, BuyPrice = 1.0m, SellPrice = 1.0m, AvgPrice = 1.0m },
            new CurrencyPrice { Type = TradeCurrencyType.Divine, BuyPrice = 200.0m, SellPrice = 180.0m, AvgPrice = 190.0m },
            new CurrencyPrice { Type = TradeCurrencyType.Exalted, BuyPrice = 50.0m, SellPrice = 45.0m, AvgPrice = 47.5m }
        };
    }

    [TestMethod]
    public void Age_WhenJustCreated_ShouldBeVeryLarge()
    {
        var age = cache.Age;

        Assert.IsTrue(age > TimeSpan.FromDays(1000)); // Should be very large since lastUpdated is DateTime.MinValue
    }

    [TestMethod]
    public void SetPrices_ShouldUpdatePricesAndAge()
    {
        cache.SetPrices(testPrices);

        var age = cache.Age;
        Assert.IsTrue(age < TimeSpan.FromSeconds(1)); // Should be very recent
        Assert.IsTrue(cache.ContainsPrice(TradeCurrencyType.Chaos));
        Assert.IsTrue(cache.ContainsPrice(TradeCurrencyType.Divine));
        Assert.IsTrue(cache.ContainsPrice(TradeCurrencyType.Exalted));
    }

    [TestMethod]
    public void ContainsPrice_WithExistingCurrency_ShouldReturnTrue()
    {
        cache.SetPrices(testPrices);

        var result = cache.ContainsPrice(TradeCurrencyType.Chaos);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsPrice_WithNonExistingCurrency_ShouldReturnFalse()
    {
        cache.SetPrices(testPrices);

        var result = cache.ContainsPrice(TradeCurrencyType.Alt);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetPrice_WithExistingCurrency_ShouldReturnCorrectPrice()
    {
        cache.SetPrices(testPrices);

        var result = cache.GetPrice(TradeCurrencyType.Divine);

        Assert.AreEqual(TradeCurrencyType.Divine, result.Type);
        Assert.AreEqual(200.0m, result.BuyPrice);
        Assert.AreEqual(180.0m, result.SellPrice);
        Assert.AreEqual(190.0m, result.AvgPrice);
    }

    [TestMethod]
    public void GetPrice_WithNonExistingCurrency_ShouldThrowException()
    {
        cache.SetPrices(testPrices);

        Assert.ThrowsExactly<CurrencyPriceNotFoundException>(() => cache.GetPrice(TradeCurrencyType.Alt));
    }

    [TestMethod]
    public void PricesHaveChanged_WithSamePrices_ShouldReturnFalse()
    {
        cache.SetPrices(testPrices);

        var result = cache.PricesHaveChanged(testPrices);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PricesHaveChanged_WithDifferentPrices_ShouldReturnTrue()
    {
        cache.SetPrices(testPrices);

        var modifiedPrices = new List<CurrencyPrice>
        {
            new CurrencyPrice { Type = TradeCurrencyType.Chaos, BuyPrice = 1.1m, SellPrice = 1.0m, AvgPrice = 1.0m }
        };

        var result = cache.PricesHaveChanged(modifiedPrices);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PricesHaveChanged_WithNewCurrency_ShouldReturnTrue()
    {
        cache.SetPrices(testPrices);

        var newPrices = new List<CurrencyPrice>
        {
            new CurrencyPrice { Type = TradeCurrencyType.Alt, BuyPrice = 5.0m, SellPrice = 4.5m, AvgPrice = 4.75m }
        };

        var result = cache.PricesHaveChanged(newPrices);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PricesHaveChanged_WithEmptyCache_ShouldReturnTrue()
    {
        var result = cache.PricesHaveChanged(testPrices);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetAllPrices_ShouldReturnAllStoredPrices()
    {
        cache.SetPrices(testPrices);

        var result = cache.GetAllPrices().ToList();

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Any(p => p.Type == TradeCurrencyType.Chaos));
        Assert.IsTrue(result.Any(p => p.Type == TradeCurrencyType.Divine));
        Assert.IsTrue(result.Any(p => p.Type == TradeCurrencyType.Exalted));
    }

    [TestMethod]
    public void GetAllPrices_WithEmptyCache_ShouldReturnEmptyCollection()
    {
        var result = cache.GetAllPrices().ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SetPrices_WithDuplicateCurrency_ShouldOverwriteExisting()
    {
        cache.SetPrices(testPrices);

        var updatedPrices = new List<CurrencyPrice>
        {
            new CurrencyPrice { Type = TradeCurrencyType.Chaos, BuyPrice = 1.5m, SellPrice = 1.4m, AvgPrice = 1.45m }
        };

        cache.SetPrices(updatedPrices);

        var result = cache.GetPrice(TradeCurrencyType.Chaos);
        Assert.AreEqual(1.5m, result.BuyPrice);
        Assert.AreEqual(1.4m, result.SellPrice);
        Assert.AreEqual(1.45m, result.AvgPrice);
    }

    [TestMethod]
    public void Age_AfterSettingPrices_ShouldIncreaseOverTime()
    {
        cache.SetPrices(testPrices);
        var initialAge = cache.Age;

        Thread.Sleep(10); // Small delay

        var laterAge = cache.Age;
        Assert.IsTrue(laterAge > initialAge);
    }
}
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Services;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Tests.Services;

[TestClass]
public class ItemPriceCacheTests
{
    private ItemPriceCache itemPriceCache = null!;
    private SearchGuiItem testItem1 = null!;
    private SearchGuiItem testItem2 = null!;

    [TestInitialize]
    public void Setup()
    {
        itemPriceCache = new ItemPriceCache();
        testItem1 = new SearchGuiItem("Test Item 1")
        {
            OfferPrice = new PriceInfo { Amount = 10, CurrencyType = TradeCurrencyType.Chaos }
        };
        testItem2 = new SearchGuiItem("Test Item 2")
        {
            OfferPrice = new PriceInfo { Amount = 20, CurrencyType = TradeCurrencyType.Divine }
        };
    }

    [TestMethod]
    public void SetItemPrices_ShouldAddItemsToCache()
    {
        var items = new List<SearchGuiItem> { testItem1, testItem2 };

        itemPriceCache.SetItemPrices(items);

        var result = itemPriceCache.GetAllItemPrices();
        Assert.AreEqual(2, result.Length);
        Assert.IsTrue(result.Any(i => i.Name == "Test Item 1"));
        Assert.IsTrue(result.Any(i => i.Name == "Test Item 2"));
    }

    [TestMethod]
    public void SetItemPrices_ShouldClearExistingItems()
    {
        var initialItems = new List<SearchGuiItem> { testItem1 };
        var newItems = new List<SearchGuiItem> { testItem2 };

        itemPriceCache.SetItemPrices(initialItems);
        itemPriceCache.SetItemPrices(newItems);

        var result = itemPriceCache.GetAllItemPrices();
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("Test Item 2", result[0].Name);
    }

    [TestMethod]
    public void GetAllItemPrices_WhenEmpty_ShouldReturnEmptyArray()
    {
        var result = itemPriceCache.GetAllItemPrices();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void ClearItemPrices_ShouldRemoveAllItems()
    {
        var items = new List<SearchGuiItem> { testItem1, testItem2 };
        itemPriceCache.SetItemPrices(items);

        itemPriceCache.ClearItemPrices();

        var result = itemPriceCache.GetAllItemPrices();
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void SetItemPrices_WithEmptyCollection_ShouldResultInEmptyCache()
    {
        var items = new List<SearchGuiItem>();

        itemPriceCache.SetItemPrices(items);

        var result = itemPriceCache.GetAllItemPrices();
        Assert.AreEqual(0, result.Length);
    }
}
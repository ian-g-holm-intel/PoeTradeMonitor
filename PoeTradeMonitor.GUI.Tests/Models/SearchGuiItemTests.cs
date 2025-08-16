using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Tests.Models;

[TestClass]
public class SearchGuiItemTests
{
    [TestMethod]
    public void Constructor_WithName_ShouldSetName()
    {
        var item = new SearchGuiItem("Test Item");

        Assert.AreEqual("Test Item", item.Name);
    }

    [TestMethod]
    public void Constructor_Default_ShouldInitializeWithDefaults()
    {
        var item = new SearchGuiItem();

        Assert.AreEqual(string.Empty, item.Name);
        Assert.IsFalse(item.Enabled);
        Assert.IsFalse(item.IsTypeName);
        Assert.AreEqual(string.Empty, item.SearchID);
        Assert.IsNotNull(item.OfferPrice);
    }

    [TestMethod]
    public void ToString_ShouldReturnName()
    {
        var item = new SearchGuiItem("Test Item");

        Assert.AreEqual("Test Item", item.ToString());
    }

    [TestMethod]
    public void Equals_WithSameProperties_ShouldReturnTrue()
    {
        var item1 = new SearchGuiItem("Test Item")
        {
            SearchID = "search123",
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };

        var item2 = new SearchGuiItem("Test Item")
        {
            SearchID = "search123",
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };

        Assert.IsTrue(item1.Equals(item2));
    }

    [TestMethod]
    public void Equals_WithDifferentNames_ShouldReturnFalse()
    {
        var item1 = new SearchGuiItem("Test Item 1");
        var item2 = new SearchGuiItem("Test Item 2");

        Assert.IsFalse(item1.Equals(item2));
    }

    [TestMethod]
    public void Equals_WithDifferentSearchID_ShouldReturnFalse()
    {
        var item1 = new SearchGuiItem("Test Item") { SearchID = "search1" };
        var item2 = new SearchGuiItem("Test Item") { SearchID = "search2" };

        Assert.IsFalse(item1.Equals(item2));
    }

    [TestMethod]
    public void Equals_WithDifferentOfferPrice_ShouldReturnFalse()
    {
        var item1 = new SearchGuiItem("Test Item")
        {
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };
        var item2 = new SearchGuiItem("Test Item")
        {
            OfferPrice = new PriceInfo { Amount = 200, CurrencyType = TradeCurrencyType.Chaos }
        };

        Assert.IsFalse(item1.Equals(item2));
    }

    [TestMethod]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var item = new SearchGuiItem("Test Item");

        Assert.IsFalse(item.Equals(null));
    }

    [TestMethod]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var item = new SearchGuiItem("Test Item");
        var other = "Not a SearchGuiItem";

        Assert.IsFalse(item.Equals(other));
    }

    [TestMethod]
    public void GetHashCode_WithSameProperties_ShouldReturnSameHash()
    {
        var item1 = new SearchGuiItem("Test Item")
        {
            SearchID = "search123",
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };

        var item2 = new SearchGuiItem("Test Item")
        {
            SearchID = "search123",
            OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
        };

        Assert.AreEqual(item1.GetHashCode(), item2.GetHashCode());
    }

    [TestMethod]
    public void Properties_ShouldBeSettableAndGettable()
    {
        var item = new SearchGuiItem();

        item.Name = "Test Name";
        item.Enabled = true;
        item.IsTypeName = true;
        item.SearchID = "search123";
        item.AutoSearchID = "auto123";
        item.Source = "Test Source";

        Assert.AreEqual("Test Name", item.Name);
        Assert.IsTrue(item.Enabled);
        Assert.IsTrue(item.IsTypeName);
        Assert.AreEqual("search123", item.SearchID);
        Assert.AreEqual("auto123", item.AutoSearchID);
        Assert.AreEqual("Test Source", item.Source);
    }

    [TestMethod]
    public void IsTypeName_ShouldBeSettableAndGettable()
    {
        var item = new SearchGuiItem();

        Assert.IsFalse(item.IsTypeName);

        item.IsTypeName = true;
        Assert.IsTrue(item.IsTypeName);

        item.IsTypeName = false;
        Assert.IsFalse(item.IsTypeName);
    }
}
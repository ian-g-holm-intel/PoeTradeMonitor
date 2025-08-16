using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Tests;

[TestClass]
public class SerializationUtilitiesTests
{
    [TestMethod]
    public void Serialize_StringList_ShouldReturnValidJson()
    {
        var list = new List<string> { "item1", "item2", "item3" };

        var result = list.Serialize();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("item1"));
        Assert.IsTrue(result.Contains("item2"));
        Assert.IsTrue(result.Contains("item3"));
    }

    [TestMethod]
    public void Serialize_EmptyStringList_ShouldReturnEmptyArrayJson()
    {
        var list = new List<string>();

        var result = list.Serialize();

        Assert.AreEqual("[]", result);
    }

    [TestMethod]
    public void DeserializeAccounts_ValidJson_ShouldReturnList()
    {
        var json = "[\"account1\",\"account2\",\"account3\"]";

        var result = json.DeserializeAccounts();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("account1", result[0]);
        Assert.AreEqual("account2", result[1]);
        Assert.AreEqual("account3", result[2]);
    }

    [TestMethod]
    public void DeserializeAccounts_EmptyString_ShouldReturnEmptyList()
    {
        var result = string.Empty.DeserializeAccounts();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeserializeAccounts_NullString_ShouldReturnEmptyList()
    {
        string? nullString = null;
        var result = nullString!.DeserializeAccounts();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeserializeAccounts_InvalidJson_ShouldReturnEmptyList()
    {
        var invalidJson = "invalid json string";

        var result = invalidJson.DeserializeAccounts();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Serialize_SearchGuiItemList_ShouldReturnValidJson()
    {
        var items = new List<SearchGuiItem>
        {
            new SearchGuiItem("Item1") { Enabled = true },
            new SearchGuiItem("Item2") { Enabled = false }
        };

        var result = items.Serialize();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("Item1"));
        Assert.IsTrue(result.Contains("Item2"));
    }

    [TestMethod]
    public void Serialize_EmptySearchGuiItemList_ShouldReturnEmptyArrayJson()
    {
        var items = new List<SearchGuiItem>();

        var result = items.Serialize();

        Assert.AreEqual("[]", result);
    }

    [TestMethod]
    public void DeserializeItems_ValidJson_ShouldReturnList()
    {
        var item1 = new SearchGuiItem("TestItem1") 
        { 
            Enabled = true,
            OfferPrice = new PriceInfo { Amount = 10, CurrencyType = TradeCurrencyType.Chaos }
        };
        var item2 = new SearchGuiItem("TestItem2") 
        { 
            Enabled = false,
            OfferPrice = new PriceInfo { Amount = 20, CurrencyType = TradeCurrencyType.Divine }
        };
        var items = new List<SearchGuiItem> { item1, item2 };
        var json = items.Serialize();

        var result = json.DeserializeItems();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("TestItem1", result[0].Name);
        Assert.AreEqual("TestItem2", result[1].Name);
        Assert.AreEqual(true, result[0].Enabled);
        Assert.AreEqual(false, result[1].Enabled);
    }

    [TestMethod]
    public void DeserializeItems_EmptyString_ShouldReturnEmptyList()
    {
        var result = string.Empty.DeserializeItems();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeserializeItems_NullString_ShouldReturnEmptyList()
    {
        string? nullString = null;
        var result = nullString!.DeserializeItems();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeserializeItems_InvalidJson_ShouldReturnEmptyList()
    {
        var invalidJson = "invalid json string";

        var result = invalidJson.DeserializeItems();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SerializeDeserialize_RoundTrip_ShouldPreserveData()
    {
        var originalList = new List<string> { "test1", "test2", "test3" };

        var serialized = originalList.Serialize();
        var deserialized = serialized.DeserializeAccounts();

        Assert.AreEqual(originalList.Count, deserialized.Count);
        for (int i = 0; i < originalList.Count; i++)
        {
            Assert.AreEqual(originalList[i], deserialized[i]);
        }
    }

    [TestMethod]
    public void SerializeDeserializeItems_RoundTrip_ShouldPreserveData()
    {
        var originalItems = new List<SearchGuiItem>
        {
            new SearchGuiItem("Item1") 
            { 
                Enabled = true, 
                SearchID = "search1",
                OfferPrice = new PriceInfo { Amount = 100, CurrencyType = TradeCurrencyType.Chaos }
            }
        };

        var serialized = originalItems.Serialize();
        var deserialized = serialized.DeserializeItems();

        Assert.AreEqual(originalItems.Count, deserialized.Count);
        Assert.AreEqual(originalItems[0].Name, deserialized[0].Name);
        Assert.AreEqual(originalItems[0].Enabled, deserialized[0].Enabled);
        Assert.AreEqual(originalItems[0].SearchID, deserialized[0].SearchID);
    }
}
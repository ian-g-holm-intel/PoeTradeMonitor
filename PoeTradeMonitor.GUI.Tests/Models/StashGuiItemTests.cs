using System.Drawing;
using PoeLib.Common;
using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Tests.Models;

[TestClass]
public class StashGuiItemTests
{
    private ItemTradeRequest testTradeRequest = null!;
    private StashGuiItem stashItem = null!;

    [TestInitialize]
    public void Setup()
    {
        var item = new TradeItem
        {
            Id = "test-item-id",
            Name = "Unique Sword",
            League = "Standard"
        };
        var price = new PriceInfo { Amount = 50m, CurrencyType = TradeCurrencyType.Chaos };
        var currencies = new List<CurrencyInfo>
        {
            new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 50m }
        };

        testTradeRequest = new ItemTradeRequest("TestCharacter", "TestAccount", item, price, currencies, DateTime.Now, 200m);
        stashItem = new StashGuiItem(testTradeRequest);
    }

    [TestMethod]
    public void Constructor_ShouldSetTradeRequest()
    {
        Assert.AreEqual(testTradeRequest, stashItem.TradeRequest);
    }

    [TestMethod]
    public void Properties_ShouldBeSettableAndGettable()
    {
        var timestamp = "2023-01-01 12:00:00";
        var name = "Test Item Name";
        var searchId = "search-123";
        var itemId = "item-456";
        var whisperToken = "whisper-token";
        var character = "PlayerName";
        var account = "AccountName";
        var source = "live-search";
        var location = new Point(10, 20);

        stashItem.Timestamp = timestamp;
        stashItem.Name = name;
        stashItem.SearchID = searchId;
        stashItem.ItemID = itemId;
        stashItem.Rarity = ItemRarity.Unique;
        stashItem.Price = new PriceInfo { Amount = 100m, CurrencyType = TradeCurrencyType.Divine };
        stashItem.StackSize = 20;
        stashItem.NumSockets = 6;
        stashItem.WhisperToken = whisperToken;
        stashItem.WhisperValue = 42;
        stashItem.Character = character;
        stashItem.Account = account;
        stashItem.ExecuteEnabled = true;
        stashItem.Source = source;
        stashItem.ServiceLocation = ServiceLocation.Local;

        Assert.AreEqual(timestamp, stashItem.Timestamp);
        Assert.AreEqual(name, stashItem.Name);
        Assert.AreEqual(searchId, stashItem.SearchID);
        Assert.AreEqual(itemId, stashItem.ItemID);
        Assert.AreEqual(ItemRarity.Unique, stashItem.Rarity);
        Assert.AreEqual(100m, stashItem.Price.Amount);
        Assert.AreEqual(TradeCurrencyType.Divine, stashItem.Price.CurrencyType);
        Assert.AreEqual(20, stashItem.StackSize);
        Assert.AreEqual(6, stashItem.NumSockets);
        Assert.AreEqual(whisperToken, stashItem.WhisperToken);
        Assert.AreEqual(42, stashItem.WhisperValue);
        Assert.AreEqual(character, stashItem.Character);
        Assert.AreEqual(account, stashItem.Account);
        Assert.IsTrue(stashItem.ExecuteEnabled);
        Assert.AreEqual(source, stashItem.Source);
        Assert.AreEqual(ServiceLocation.Local, stashItem.ServiceLocation);
    }

    [TestMethod]
    public void ExplicitModsDisplayString_WithEmptyMods_ShouldReturnEmpty()
    {
        stashItem.ExplicitMods = new List<Mod>();
        stashItem.FracturedMods = new List<Mod>();

        var result = stashItem.ExplicitModsDisplayString;

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ExplicitModsDisplayString_WithMods_ShouldFormatCorrectly()
    {
        stashItem.ExplicitMods = new List<Mod>
        {
            new Mod { RawModText = "+50 to Life" },
            new Mod { RawModText = "25% increased Damage" }
        };
        stashItem.FracturedMods = new List<Mod>
        {
            new Mod { RawModText = "Fractured Mod" }
        };

        var result = stashItem.ExplicitModsDisplayString;

        Assert.IsTrue(result.Contains("Fractured Mod"));
        Assert.IsTrue(result.Contains("+50 to Life"));
        Assert.IsTrue(result.Contains("25% increased Damage"));
        Assert.IsTrue(result.Contains(Environment.NewLine));
    }

    [TestMethod]
    public void Currencies_ShouldBeInitializedAsList()
    {
        Assert.IsNotNull(stashItem.Currencies);
        Assert.IsInstanceOfType(stashItem.Currencies, typeof(List<CurrencyInfo>));
    }

    [TestMethod]
    public void ToString_ShouldFormatItemInformation()
    {
        stashItem.Name = "Test Sword";
        stashItem.Character = "PlayerOne";
        stashItem.Account = "AccountOne";
        stashItem.Price = new PriceInfo { Amount = 75m, CurrencyType = TradeCurrencyType.Chaos };
        stashItem.Source = "trade-api";
        stashItem.Rarity = ItemRarity.Rare;
        stashItem.NumSockets = 4;

        var result = stashItem.ToString();

        Assert.IsTrue(result.Contains("Test Sword"));
        Assert.IsTrue(result.Contains("PlayerOne"));
        Assert.IsTrue(result.Contains("AccountOne"));
        Assert.IsTrue(result.Contains("75"));
        Assert.IsTrue(result.Contains("trade-api"));
        Assert.IsTrue(result.Contains("Rare"));
        Assert.IsTrue(result.Contains("Sockets: 4"));
    }

    [TestMethod]
    public void DefaultValues_ShouldBeCorrect()
    {
        var newItem = new StashGuiItem(testTradeRequest);

        Assert.AreEqual(string.Empty, newItem.Timestamp);
        Assert.AreEqual(string.Empty, newItem.Name);
        Assert.AreEqual(string.Empty, newItem.SearchID);
        Assert.AreEqual(string.Empty, newItem.ItemID);
        Assert.AreEqual(ItemRarity.Normal, newItem.Rarity);
        Assert.IsNotNull(newItem.Price);
        Assert.AreEqual(0, newItem.StackSize);
        Assert.AreEqual(0, newItem.NumSockets);
        Assert.AreEqual(string.Empty, newItem.WhisperToken);
        Assert.AreEqual(0, newItem.WhisperValue);
        Assert.AreEqual(string.Empty, newItem.Character);
        Assert.AreEqual(string.Empty, newItem.Account);
        Assert.IsFalse(newItem.ExecuteEnabled);
        Assert.AreEqual(string.Empty, newItem.Source);
        Assert.AreEqual(ServiceLocation.Local, newItem.ServiceLocation);
    }

    [TestMethod]
    public void ExplicitMods_ShouldSupportModification()
    {
        var mods = new List<Mod>
        {
            new Mod { RawModText = "Test Mod 1" },
            new Mod { RawModText = "Test Mod 2" }
        };

        stashItem.ExplicitMods = mods;

        Assert.AreEqual(2, stashItem.ExplicitMods.Count);
        Assert.AreEqual("Test Mod 1", stashItem.ExplicitMods[0].RawModText);
        Assert.AreEqual("Test Mod 2", stashItem.ExplicitMods[1].RawModText);
    }

    [TestMethod]
    public void FracturedMods_ShouldSupportModification()
    {
        var mods = new List<Mod>
        {
            new Mod { RawModText = "Fractured Test Mod" }
        };

        stashItem.FracturedMods = mods;

        Assert.AreEqual(1, stashItem.FracturedMods.Count);
        Assert.AreEqual("Fractured Test Mod", stashItem.FracturedMods[0].RawModText);
    }
}
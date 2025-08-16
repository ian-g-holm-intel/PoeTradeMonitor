namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE1DeserializationTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task DeserializeAbyssJewel_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/AbyssJewel.json");
        
        ValidateBasicTradeSearchResponse(response, "AbyssJewel.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "AbyssJewel.json");
        ValidateTradeListing(firstItem.Listing, "AbyssJewel.json");
        ValidateTradeItem(firstItem.Item, "AbyssJewel.json");
        
        // Verify PoE1 specific properties
        Assert.IsNull(firstItem.Item.Realm, "PoE1 items should not have realm property");
        Assert.IsTrue(firstItem.Listing.Account.Realm == "pc", "PoE1 account realm should be 'pc'");
    }

    [TestMethod]
    public async Task DeserializeAccessory_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Accessory.json");
        
        ValidateBasicTradeSearchResponse(response, "Accessory.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Accessory.json");
        ValidateTradeListing(firstItem.Listing, "Accessory.json");
        ValidateTradeItem(firstItem.Item, "Accessory.json");
    }

    [TestMethod]
    public async Task DeserializeBaseJewel_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/BaseJewel.json");
        
        ValidateBasicTradeSearchResponse(response, "BaseJewel.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "BaseJewel.json");
        ValidateTradeListing(firstItem.Listing, "BaseJewel.json");
        ValidateTradeItem(firstItem.Item, "BaseJewel.json");
    }

    [TestMethod]
    public async Task DeserializeBeasts_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Beasts.json");
        
        ValidateBasicTradeSearchResponse(response, "Beasts.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Beasts.json");
        ValidateTradeListing(firstItem.Listing, "Beasts.json");
        ValidateTradeItem(firstItem.Item, "Beasts.json");
    }

    [TestMethod]
    public async Task DeserializeBodyArmour_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/BodyArmour.json");
        
        ValidateBasicTradeSearchResponse(response, "BodyArmour.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "BodyArmour.json");
        ValidateTradeListing(firstItem.Listing, "BodyArmour.json");
        ValidateTradeItem(firstItem.Item, "BodyArmour.json");
    }

    [TestMethod]
    public async Task DeserializeClusterJewel_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/ClusterJewel.json");
        
        ValidateBasicTradeSearchResponse(response, "ClusterJewel.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "ClusterJewel.json");
        ValidateTradeListing(firstItem.Listing, "ClusterJewel.json");
        ValidateTradeItem(firstItem.Item, "ClusterJewel.json");
    }

    [TestMethod]
    public async Task DeserializeDivinationCard_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/DivinationCard.json");
        
        ValidateBasicTradeSearchResponse(response, "DivinationCard.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "DivinationCard.json");
        ValidateTradeListing(firstItem.Listing, "DivinationCard.json");
        ValidateTradeItem(firstItem.Item, "DivinationCard.json");
    }

    [TestMethod]
    public async Task DeserializeExpeditionLogbook_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/ExpeditionLogbook.json");
        
        ValidateBasicTradeSearchResponse(response, "ExpeditionLogbook.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "ExpeditionLogbook.json");
        ValidateTradeListing(firstItem.Listing, "ExpeditionLogbook.json");
        ValidateTradeItem(firstItem.Item, "ExpeditionLogbook.json");
    }

    [TestMethod]
    public async Task DeserializeFlask_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Flask.json");
        
        ValidateBasicTradeSearchResponse(response, "Flask.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Flask.json");
        ValidateTradeListing(firstItem.Listing, "Flask.json");
        ValidateTradeItem(firstItem.Item, "Flask.json");
    }

    [TestMethod]
    public async Task DeserializeHeistBlueprint_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/HeistBlueprint.json");
        
        ValidateBasicTradeSearchResponse(response, "HeistBlueprint.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "HeistBlueprint.json");
        ValidateTradeListing(firstItem.Listing, "HeistBlueprint.json");
        ValidateTradeItem(firstItem.Item, "HeistBlueprint.json");
    }

    [TestMethod]
    public async Task DeserializeHeistContract_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/HeistContract.json");
        
        ValidateBasicTradeSearchResponse(response, "HeistContract.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "HeistContract.json");
        ValidateTradeListing(firstItem.Listing, "HeistContract.json");
        ValidateTradeItem(firstItem.Item, "HeistContract.json");
    }

    [TestMethod]
    public async Task DeserializeHeistEquipment_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/HeistEquipment.json");
        
        ValidateBasicTradeSearchResponse(response, "HeistEquipment.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "HeistEquipment.json");
        ValidateTradeListing(firstItem.Listing, "HeistEquipment.json");
        ValidateTradeItem(firstItem.Item, "HeistEquipment.json");
    }

    [TestMethod]
    public async Task DeserializeMap_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Map.json");
        
        ValidateBasicTradeSearchResponse(response, "Map.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Map.json");
        ValidateTradeListing(firstItem.Listing, "Map.json");
        ValidateTradeItem(firstItem.Item, "Map.json");
    }

    [TestMethod]
    public async Task DeserializeMapFragment_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/MapFragment.json");
        
        ValidateBasicTradeSearchResponse(response, "MapFragment.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "MapFragment.json");
        ValidateTradeListing(firstItem.Listing, "MapFragment.json");
        ValidateTradeItem(firstItem.Item, "MapFragment.json");
    }

    [TestMethod]
    public async Task DeserializeMisc_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Misc.json");
        
        ValidateBasicTradeSearchResponse(response, "Misc.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Misc.json");
        ValidateTradeListing(firstItem.Listing, "Misc.json");
        ValidateTradeItem(firstItem.Item, "Misc.json");
    }

    [TestMethod]
    public async Task DeserializeOmen_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Omen.json");
        
        ValidateBasicTradeSearchResponse(response, "Omen.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Omen.json");
        ValidateTradeListing(firstItem.Listing, "Omen.json");
        ValidateTradeItem(firstItem.Item, "Omen.json");
    }

    [TestMethod]
    public async Task DeserializeRarity_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Rarity.json");
        
        ValidateBasicTradeSearchResponse(response, "Rarity.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Rarity.json");
        ValidateTradeListing(firstItem.Listing, "Rarity.json");
        ValidateTradeItem(firstItem.Item, "Rarity.json");
        
        // Verify socket structure for PoE1
        if (firstItem.Item.Sockets != null && firstItem.Item.Sockets.Count > 0)
        {
            var firstSocket = firstItem.Item.Sockets.First();
            Assert.IsNotNull(firstSocket.Attr, "PoE1 sockets should have Attr property");
            Assert.IsNotNull(firstSocket.SColour, "PoE1 sockets should have SColour property");
            Assert.IsNull(firstSocket.Type, "PoE1 sockets should not have Type property");
            Assert.IsNull(firstSocket.Item, "PoE1 sockets should not have Item property");
        }
    }

    [TestMethod]
    public async Task DeserializeSkillGem_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SkillGem.json");
        
        ValidateBasicTradeSearchResponse(response, "SkillGem.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "SkillGem.json");
        ValidateTradeListing(firstItem.Listing, "SkillGem.json");
        ValidateTradeItem(firstItem.Item, "SkillGem.json");
        
        // Verify gem-specific properties
        Assert.IsNotNull(firstItem.Item.Support, "Gem items should have Support property");
        Assert.AreEqual(false, firstItem.Item.Support, "This should be a skill gem, not support gem");
    }

    [TestMethod]
    public async Task DeserializeSupportGem_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SupportGem.json");
        
        ValidateBasicTradeSearchResponse(response, "SupportGem.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "SupportGem.json");
        ValidateTradeListing(firstItem.Listing, "SupportGem.json");
        ValidateTradeItem(firstItem.Item, "SupportGem.json");
        
        // Verify support gem-specific properties
        Assert.IsNotNull(firstItem.Item.Support, "Gem items should have Support property");
        Assert.AreEqual(true, firstItem.Item.Support, "This should be a support gem");
    }

    [TestMethod]
    public async Task DeserializeTattoo_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Tattoo.json");
        
        ValidateBasicTradeSearchResponse(response, "Tattoo.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Tattoo.json");
        ValidateTradeListing(firstItem.Listing, "Tattoo.json");
        ValidateTradeItem(firstItem.Item, "Tattoo.json");
    }

    [TestMethod]
    public async Task DeserializeWeapon_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Weapon.json");
        
        ValidateBasicTradeSearchResponse(response, "Weapon.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Weapon.json");
        ValidateTradeListing(firstItem.Listing, "Weapon.json");
        ValidateTradeItem(firstItem.Item, "Weapon.json");
        
        // Verify weapon-specific extended properties
        if (firstItem.Item.Extended != null)
        {
            Assert.IsTrue(firstItem.Item.Extended.Dps > 0 || firstItem.Item.Extended.PhysicalDps > 0, 
                "Weapons should have DPS information");
        }
    }
}
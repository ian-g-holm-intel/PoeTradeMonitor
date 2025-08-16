namespace PoeTrade.Contracts.Tests;

[TestClass]
public class CrossCompatibilityTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task AllPoE1Examples_ShouldDeserializeSuccessfully()
    {
        var poe1Files = new[]
        {
            "Responses/PoE1/AbyssJewel.json",
            "Responses/PoE1/Accessory.json",
            "Responses/PoE1/BaseJewel.json",
            "Responses/PoE1/Beasts.json",
            "Responses/PoE1/BodyArmour.json",
            "Responses/PoE1/ClusterJewel.json",
            "Responses/PoE1/DivinationCard.json",
            "Responses/PoE1/ExpeditionLogbook.json",
            "Responses/PoE1/Flask.json",
            "Responses/PoE1/HeistBlueprint.json",
            "Responses/PoE1/HeistContract.json",
            "Responses/PoE1/HeistEquipment.json",
            "Responses/PoE1/Map.json",
            "Responses/PoE1/MapFragment.json",
            "Responses/PoE1/Misc.json",
            "Responses/PoE1/Omen.json",
            "Responses/PoE1/Rarity.json",
            "Responses/PoE1/SkillGem.json",
            "Responses/PoE1/SupportGem.json",
            "Responses/PoE1/Tattoo.json",
            "Responses/PoE1/Weapon.json"
        };

        foreach (var fileName in poe1Files)
        {
            var response = await DeserializeJsonFileAsync(fileName);
            ValidateBasicTradeSearchResponse(response, fileName);
            
            var firstItem = response.Result.First();
            ValidateTradeSearchResult(firstItem, fileName);
            ValidateTradeListing(firstItem.Listing, fileName);
            ValidateTradeItem(firstItem.Item, fileName);
            
            // Verify PoE1 characteristics
            Assert.IsNull(firstItem.Item.Realm, $"PoE1 items should not have realm property: {fileName}");
            Assert.IsNull(firstItem.Item.GrantedSkills, $"PoE1 items should not have granted skills: {fileName}");
            Assert.IsNull(firstItem.Item.RuneMods, $"PoE1 items should not have rune mods: {fileName}");
            // Note: PoE1 can have fractured items, but they're less common than in PoE2
        }
    }

    [TestMethod]
    public async Task AllPoE2Examples_ShouldDeserializeSuccessfully()
    {
        var poe2Files = new[]
        {
            "Responses/PoE2/Accessory.json",
            "Responses/PoE2/BodyArmour.json",
            "Responses/PoE2/Flask.json",
            "Responses/PoE2/Gems.json",
            "Responses/PoE2/Jewel.json",
            "Responses/PoE2/Misc.json",
            "Responses/PoE2/Omen.json",
            "Responses/PoE2/Rarity.json",
            "Responses/PoE2/Talisman.json",
            "Responses/PoE2/Waystone.json",
            "Responses/PoE2/Weapon.json"
        };

        foreach (var fileName in poe2Files)
        {
            var response = await DeserializeJsonFileAsync(fileName);
            ValidateBasicTradeSearchResponse(response, fileName);
            
            var firstItem = response.Result.First();
            ValidateTradeSearchResult(firstItem, fileName);
            ValidateTradeListing(firstItem.Listing, fileName);
            ValidateTradeItem(firstItem.Item, fileName);
            
            // Verify PoE2 characteristics
            Assert.AreEqual("poe2", firstItem.Item.Realm, $"PoE2 items should have realm property: {fileName}");
            Assert.AreEqual("poe2", firstItem.Listing.Account.Realm, $"PoE2 accounts should have poe2 realm: {fileName}");
        }
    }

    [TestMethod]
    public async Task SocketStructure_ShouldWorkForBothVersions()
    {
        // Test PoE1 socket structure (traditional gem sockets)
        var poe1Response = await DeserializeJsonFileAsync("Responses/PoE1/Rarity.json");
        var poe1Item = poe1Response.Result.First().Item;
        
        if (poe1Item.Sockets != null && poe1Item.Sockets.Count > 0)
        {
            foreach (var socket in poe1Item.Sockets)
            {
                Assert.IsNotNull(socket.Attr, "PoE1 sockets should have Attr");
                Assert.IsNotNull(socket.SColour, "PoE1 sockets should have SColour");
                Assert.IsNull(socket.Type, "PoE1 sockets should not have Type");
                Assert.IsNull(socket.Item, "PoE1 sockets should not have Item");
            }
        }

        // Test PoE2 socket structure (rune sockets)
        var poe2Response = await DeserializeJsonFileAsync("Responses/PoE2/Rarity.json");
        var poe2Item = poe2Response.Result.First().Item;
        
        if (poe2Item.Sockets != null && poe2Item.Sockets.Count > 0)
        {
            var runeSockets = poe2Item.Sockets.Where(s => s.Type == "rune").ToList();
            foreach (var socket in runeSockets)
            {
                Assert.AreEqual("rune", socket.Type, "PoE2 rune sockets should have Type = 'rune'");
                Assert.IsNotNull(socket.Item, "PoE2 rune sockets should have Item");
                Assert.IsNull(socket.Attr, "PoE2 rune sockets should not have Attr");
                Assert.IsNull(socket.SColour, "PoE2 rune sockets should not have SColour");
            }
        }
    }

    [TestMethod]
    public async Task ExtendedModStructure_ShouldWorkForBothVersions()
    {
        // Test PoE1 extended mod structure
        var poe1Response = await DeserializeJsonFileAsync("Responses/PoE1/Weapon.json");
        var poe1Item = poe1Response.Result.First().Item;
        
        if (poe1Item.Extended?.Mods != null)
        {
            Assert.IsNotNull(poe1Item.Extended.Mods.Explicit, "PoE1 should have explicit mods");
            Assert.IsNotNull(poe1Item.Extended.Mods.Implicit, "PoE1 should have implicit mods");
            // PoE1 should not have fractured mods in extended
            Assert.IsNull(poe1Item.Extended.Mods.Fractured, "PoE1 should not have fractured mods");
        }

        // Test PoE2 extended mod structure
        var poe2Response = await DeserializeJsonFileAsync("Responses/PoE2/Weapon.json");
        var poe2Item = poe2Response.Result.First().Item;
        
        if (poe2Item.Extended?.Mods != null)
        {
            Assert.IsNotNull(poe2Item.Extended.Mods.Explicit, "PoE2 should have explicit mods");
            
            // PoE2 may have fractured mods
            if (poe2Item.Fractured == true)
            {
                Assert.IsNotNull(poe2Item.Extended.Mods.Fractured, "Fractured PoE2 items should have fractured mods in extended");
            }
        }

        // Test PoE2 specific rune hashes
        if (poe2Item.Extended?.Hashes?.Rune != null)
        {
            Assert.IsTrue(poe2Item.Extended.Hashes.Rune.Count > 0, "PoE2 items with runes should have rune hashes");
        }
    }

    [TestMethod]
    public async Task GemHandling_ShouldWorkForBothVersions()
    {
        // Test PoE1 gems
        var poe1SkillResponse = await DeserializeJsonFileAsync("Responses/PoE1/SkillGem.json");
        var poe1SkillGem = poe1SkillResponse.Result.First().Item;
        
        Assert.AreEqual(false, poe1SkillGem.Support, "PoE1 skill gem should not be support");
        Assert.IsNull(poe1SkillGem.SupportGemRequirements, "PoE1 skill gems should not have support requirements");
        Assert.IsNull(poe1SkillGem.GemTabs, "PoE1 gems should not have gem tabs");

        var poe1SupportResponse = await DeserializeJsonFileAsync("Responses/PoE1/SupportGem.json");
        var poe1SupportGem = poe1SupportResponse.Result.First().Item;
        
        Assert.AreEqual(true, poe1SupportGem.Support, "PoE1 support gem should be support");
        Assert.IsNull(poe1SupportGem.SupportGemRequirements, "PoE1 support gems should not have PoE2 requirements");

        // Test PoE2 gems
        var poe2Response = await DeserializeJsonFileAsync("Responses/PoE2/Gems.json");
        var poe2Gem = poe2Response.Result.First().Item;
        
        Assert.IsNotNull(poe2Gem.Support, "PoE2 gems should have support property");
        
        if (poe2Gem.Support == true)
        {
            // PoE2 support gems may have special requirements
            if (poe2Gem.SupportGemRequirements != null)
            {
                Assert.IsTrue(poe2Gem.SupportGemRequirements.Count > 0, "PoE2 support gems should have requirements");
            }
        }

        // PoE2 gems should have gem tabs
        if (poe2Gem.GemTabs != null)
        {
            Assert.IsTrue(poe2Gem.GemTabs.Count > 0, "PoE2 gems should have gem tabs");
        }
    }

    [TestMethod]
    public async Task ItemProperties_ShouldDeserializeCorrectly()
    {
        // Test complex property values across both game versions
        var poe1Response = await DeserializeJsonFileAsync("Responses/PoE1/Weapon.json");
        var poe1Item = poe1Response.Result.First().Item;
        
        if (poe1Item.Properties != null)
        {
            foreach (var property in poe1Item.Properties)
            {
                Assert.IsFalse(string.IsNullOrEmpty(property.Name), "Property names should not be empty");
                
                if (property.Values != null && property.Values.Count > 0)
                {
                    foreach (var value in property.Values)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(value.Value), "Property values should not be empty");
                        // Value contains the actual property value, Index contains formatting info
                    }
                }
            }
        }

        var poe2Response = await DeserializeJsonFileAsync("Responses/PoE2/Weapon.json");
        var poe2Item = poe2Response.Result.First().Item;
        
        if (poe2Item.Properties != null)
        {
            foreach (var property in poe2Item.Properties)
            {
                Assert.IsFalse(string.IsNullOrEmpty(property.Name), "Property names should not be empty");
                
                if (property.Values != null && property.Values.Count > 0)
                {
                    foreach (var value in property.Values)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(value.Value), "Property values should not be empty");
                    }
                }
            }
        }
    }

    [TestMethod]
    public async Task PriceInformation_ShouldBeConsistent()
    {
        // Test that price information is consistent across both game versions
        var poe1Response = await DeserializeJsonFileAsync("Responses/PoE1/Rarity.json");
        var poe2Response = await DeserializeJsonFileAsync("Responses/PoE2/Rarity.json");
        
        var poe1Listing = poe1Response.Result.First().Listing;
        var poe2Listing = poe2Response.Result.First().Listing;
        
        // Both should have price information
        Assert.IsNotNull(poe1Listing.Price, "PoE1 listings should have price");
        Assert.IsNotNull(poe2Listing.Price, "PoE2 listings should have price");
        
        // Price structure should be the same
        Assert.AreNotEqual(TradeCurrencyType.Unknown, poe1Listing.Price.CurrencyType, "PoE1 price should have valid currency");
        Assert.AreNotEqual(TradeCurrencyType.Unknown, poe2Listing.Price.CurrencyType, "PoE2 price should have valid currency");
        
        Assert.IsTrue(poe1Listing.Price.Amount > 0, "PoE1 price amount should be positive");
        Assert.IsTrue(poe2Listing.Price.Amount > 0, "PoE2 price amount should be positive");
    }
}
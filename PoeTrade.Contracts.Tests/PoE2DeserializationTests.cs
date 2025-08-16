namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE2DeserializationTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task DeserializeAccessory_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Accessory.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Accessory.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Accessory.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Accessory.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Accessory.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
        Assert.IsTrue(firstItem.Listing.Account.Realm == "poe2", "PoE2 account realm should be 'poe2'");
        
        // Check for fractured items (PoE2 specific)
        if (firstItem.Item.Fractured == true)
        {
            Assert.IsNotNull(firstItem.Item.FracturedMods, "Fractured items should have fractured mods");
            Assert.IsTrue(firstItem.Item.FracturedMods.Count > 0, "Fractured items should have at least one fractured mod");
        }
    }

    [TestMethod]
    public async Task DeserializeBodyArmour_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/BodyArmour.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/BodyArmour.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/BodyArmour.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/BodyArmour.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/BodyArmour.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeFlask_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Flask.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Flask.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Flask.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Flask.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Flask.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeGems_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Gems.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Gems.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Gems.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Gems.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Gems.json");
        
        // Verify PoE2 gem specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
        Assert.IsNotNull(firstItem.Item.Support, "Gem items should have Support property");
        
        // Check for PoE2 specific gem features
        if (firstItem.Item.SupportGemRequirements != null)
        {
            Assert.IsTrue(firstItem.Item.SupportGemRequirements.Count > 0, "Support gems should have requirements");
        }
        
        if (firstItem.Item.GemTabs != null)
        {
            Assert.IsTrue(firstItem.Item.GemTabs.Count > 0, "PoE2 gems should have gem tabs");
            var firstTab = firstItem.Item.GemTabs.First();
            if (firstTab.Pages != null && firstTab.Pages.Count > 0)
            {
                var firstPage = firstTab.Pages.First();
                Assert.IsNotNull(firstPage.Stats, "Gem pages should have stats");
            }
        }
    }

    [TestMethod]
    public async Task DeserializeJewel_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Jewel.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Jewel.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Jewel.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Jewel.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Jewel.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeMisc_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Misc.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Misc.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Misc.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Misc.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Misc.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeOmen_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Omen.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Omen.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Omen.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Omen.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Omen.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeRarity_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Rarity.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Rarity.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Rarity.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Rarity.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Rarity.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
        
        // Check for PoE2 specific features like granted skills
        if (firstItem.Item.GrantedSkills != null && firstItem.Item.GrantedSkills.Count > 0)
        {
            var firstSkill = firstItem.Item.GrantedSkills.First();
            Assert.IsFalse(string.IsNullOrEmpty(firstSkill.Name), "Granted skills should have a name");
            Assert.IsNotNull(firstSkill.Values, "Granted skills should have values");
        }
        
        // Verify rune socket structure for PoE2
        if (firstItem.Item.Sockets != null && firstItem.Item.Sockets.Count > 0)
        {
            var runeSocket = firstItem.Item.Sockets.FirstOrDefault(s => s.Type == "rune");
            if (runeSocket != null)
            {
                Assert.AreEqual("rune", runeSocket.Type, "PoE2 rune sockets should have Type = 'rune'");
                Assert.IsNotNull(runeSocket.Item, "PoE2 rune sockets should have Item property");
                Assert.IsNull(runeSocket.Attr, "PoE2 rune sockets should not have Attr property");
                Assert.IsNull(runeSocket.SColour, "PoE2 rune sockets should not have SColour property");
            }
        }
        
        // Check for rune mods (PoE2 specific)
        if (firstItem.Item.RuneMods != null)
        {
            Assert.IsTrue(firstItem.Item.RuneMods.Count > 0, "Items with rune mods should have at least one");
        }
        
        // Check for socketed items (runes/soul cores)
        if (firstItem.Item.SocketedItems != null && firstItem.Item.SocketedItems.Count > 0)
        {
            var socketedItem = firstItem.Item.SocketedItems.First();
            Assert.AreEqual("poe2", socketedItem.Realm, "Socketed items should also have PoE2 realm");
            Assert.IsNotNull(socketedItem.Socket, "Socketed items should have socket index");
        }
    }

    [TestMethod]
    public async Task DeserializeTalisman_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Talisman.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Talisman.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Talisman.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Talisman.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Talisman.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeWaystone_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Waystone.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Waystone.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Waystone.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Waystone.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Waystone.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
    }

    [TestMethod]
    public async Task DeserializeWeapon_ShouldSucceed()
    {
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Weapon.json");
        
        ValidateBasicTradeSearchResponse(response, "Responses/PoE2/Weapon.json");
        
        var firstItem = response.Result.First();
        ValidateTradeSearchResult(firstItem, "Responses/PoE2/Weapon.json");
        ValidateTradeListing(firstItem.Listing, "Responses/PoE2/Weapon.json");
        ValidateTradeItem(firstItem.Item, "Responses/PoE2/Weapon.json");
        
        // Verify PoE2 specific properties
        Assert.AreEqual("poe2", firstItem.Item.Realm, "PoE2 items should have realm property set to 'poe2'");
        
        // Check for PoE2 weapon specific features like granted skills
        if (firstItem.Item.GrantedSkills != null && firstItem.Item.GrantedSkills.Count > 0)
        {
            var firstSkill = firstItem.Item.GrantedSkills.First();
            Assert.IsFalse(string.IsNullOrEmpty(firstSkill.Name), "Granted skills should have a name");
            Assert.IsNotNull(firstSkill.Icon, "Granted skills should have an icon");
        }
        
        // Verify weapon-specific extended properties
        if (firstItem.Item.Extended != null)
        {
            Assert.IsTrue(firstItem.Item.Extended.Dps > 0 || firstItem.Item.Extended.PhysicalDps > 0, 
                "Weapons should have DPS information");
        }
        
        // Check for fractured properties (common in PoE2)
        if (firstItem.Item.Fractured == true)
        {
            Assert.IsNotNull(firstItem.Item.FracturedMods, "Fractured weapons should have fractured mods");
            Assert.IsTrue(firstItem.Item.FracturedMods.Count > 0, "Fractured weapons should have at least one fractured mod");
            
            // Verify fractured mods in extended information
            if (firstItem.Item.Extended?.Mods?.Fractured != null)
            {
                Assert.IsTrue(firstItem.Item.Extended.Mods.Fractured.Count > 0, 
                    "Extended mods should contain fractured mod information");
            }
        }
        
        // Check for rune sockets and socketed runes/soul cores
        if (firstItem.Item.Sockets != null)
        {
            var runeSockets = firstItem.Item.Sockets.Where(s => s.Type == "rune").ToList();
            if (runeSockets.Count > 0)
            {
                Assert.IsTrue(runeSockets.All(s => s.Item != null), "Rune sockets should specify item type");
                
                // If there are socketed items, verify they match the socket configuration
                if (firstItem.Item.SocketedItems != null && firstItem.Item.SocketedItems.Count > 0)
                {
                    foreach (var socketedItem in firstItem.Item.SocketedItems)
                    {
                        Assert.IsNotNull(socketedItem.Socket, "Socketed items should have socket index");
                        Assert.IsTrue(socketedItem.Socket >= 0 && socketedItem.Socket < firstItem.Item.Sockets.Count,
                            "Socket index should be valid");
                    }
                }
            }
        }
    }

    [TestMethod]
    public async Task VerifyPoE2SpecificFeatures_ShouldBePresent()
    {
        // Test that PoE2 specific features are properly deserialized across different item types
        var weaponResponse = await DeserializeJsonFileAsync("Responses/PoE2/Weapon.json");
        var gemResponse = await DeserializeJsonFileAsync("Responses/PoE2/Gems.json");
        
        // Check for PoE2 specific properties across different item types
        var weaponItem = weaponResponse.Result.First().Item;
        var gemItem = gemResponse.Result.First().Item;
        
        // All PoE2 items should have realm
        Assert.AreEqual("poe2", weaponItem.Realm);
        Assert.AreEqual("poe2", gemItem.Realm);
        
        // Check for PoE2 specific mod types in extended hashes
        if (weaponItem.Extended?.Hashes?.Rune != null)
        {
            Assert.IsTrue(weaponItem.Extended.Hashes.Rune.Count > 0, "PoE2 items should have rune hashes");
        }
        
        // Support gems should have PoE2 specific requirements
        if (gemItem.Support == true && gemItem.SupportGemRequirements != null)
        {
            Assert.IsTrue(gemItem.SupportGemRequirements.Count > 0, 
                "PoE2 support gems should have specific requirements");
        }
    }
}
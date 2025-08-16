namespace PoeTrade.Contracts.Tests;

[TestClass]
public class TradeItemModIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void TradeItem_WithModProperties_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "id": "test-item",
            "league": "Standard",
            "typeLine": "Test Item",
            "baseType": "Test Base",
            "w": 1,
            "h": 1,
            "verified": true,
            "identified": true,
            "ilvl": 75,
            "frameType": 2,
            "icon": "",
            "implicitMods": [
                "+10% increased Movement Speed"
            ],
            "explicitMods": [
                "Adds 15 to 25 Physical Damage",
                "+12% increased Attack Speed",
                "+(20-30) to maximum Life"
            ],
            "fracturedMods": [
                "+5% increased Critical Strike Chance"
            ]
        }
        """;

        // Act
        var item = JsonSerializer.Deserialize<TradeItem>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(item);
        
        // Test implicit mods
        Assert.IsNotNull(item.ImplicitMods);
        Assert.AreEqual(1, item.ImplicitMods.Count);
        Assert.AreEqual("+10% increased Movement Speed", item.ImplicitMods[0].RawModText);
        Assert.AreEqual("+#% increased Movement Speed", item.ImplicitMods[0].ModText);
        Assert.AreEqual(1, item.ImplicitMods[0].Values.Count);
        Assert.AreEqual(10, item.ImplicitMods[0].Values[0]);
        
        // Test explicit mods
        Assert.IsNotNull(item.ExplicitMods);
        Assert.AreEqual(3, item.ExplicitMods.Count);
        
        var firstExplicitMod = item.ExplicitMods[0];
        Assert.AreEqual("Adds 15 to 25 Physical Damage", firstExplicitMod.RawModText);
        Assert.AreEqual("Adds # to # Physical Damage", firstExplicitMod.ModText);
        Assert.AreEqual(2, firstExplicitMod.Values.Count);
        Assert.AreEqual(15, firstExplicitMod.Values[0]);
        Assert.AreEqual(25, firstExplicitMod.Values[1]);
        
        var secondExplicitMod = item.ExplicitMods[1];
        Assert.AreEqual("+12% increased Attack Speed", secondExplicitMod.RawModText);
        Assert.AreEqual("+#% increased Attack Speed", secondExplicitMod.ModText);
        Assert.AreEqual(1, secondExplicitMod.Values.Count);
        Assert.AreEqual(12, secondExplicitMod.Values[0]);
        
        var thirdExplicitMod = item.ExplicitMods[2];
        Assert.AreEqual("+(20-30) to maximum Life", thirdExplicitMod.RawModText);
        Assert.AreEqual("+(#-#) to maximum Life", thirdExplicitMod.ModText);
        Assert.AreEqual(2, thirdExplicitMod.Values.Count);
        Assert.AreEqual(20, thirdExplicitMod.Values[0]);
        Assert.AreEqual(30, thirdExplicitMod.Values[1]);
        
        // Test fractured mods
        Assert.IsNotNull(item.FracturedMods);
        Assert.AreEqual(1, item.FracturedMods.Count);
        Assert.AreEqual("+5% increased Critical Strike Chance", item.FracturedMods[0].RawModText);
        Assert.AreEqual("+#% increased Critical Strike Chance", item.FracturedMods[0].ModText);
        Assert.AreEqual(1, item.FracturedMods[0].Values.Count);
        Assert.AreEqual(5, item.FracturedMods[0].Values[0]);
    }

    [TestMethod]
    public void TradeItem_WithNullModProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var json = """
        {
            "id": "test-item",
            "league": "Standard",
            "typeLine": "Test Item",
            "baseType": "Test Base",
            "w": 1,
            "h": 1,
            "verified": true,
            "identified": true,
            "ilvl": 75,
            "frameType": 1,
            "icon": "",
            "implicitMods": null,
            "explicitMods": null,
            "fracturedMods": null
        }
        """;

        // Act
        var item = JsonSerializer.Deserialize<TradeItem>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNull(item.ImplicitMods);
        Assert.IsNull(item.ExplicitMods);
        Assert.IsNull(item.FracturedMods);
    }

    [TestMethod]
    public void TradeItem_WithEmptyModProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var json = """
        {
            "id": "test-item",
            "league": "Standard",
            "typeLine": "Test Item",
            "baseType": "Test Base",
            "w": 1,
            "h": 1,
            "verified": true,
            "identified": true,
            "ilvl": 75,
            "frameType": 1,
            "icon": "",
            "implicitMods": [],
            "explicitMods": [],
            "fracturedMods": []
        }
        """;

        // Act
        var item = JsonSerializer.Deserialize<TradeItem>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNotNull(item.ImplicitMods);
        Assert.AreEqual(0, item.ImplicitMods.Count);
        Assert.IsNotNull(item.ExplicitMods);
        Assert.AreEqual(0, item.ExplicitMods.Count);
        Assert.IsNotNull(item.FracturedMods);
        Assert.AreEqual(0, item.FracturedMods.Count);
    }

    [TestMethod]
    public void TradeItem_ModSerialization_ShouldSerializeBackToStringArrays()
    {
        // Arrange
        var item = new TradeItem
        {
            Id = "test-item",
            League = "Standard",
            TypeLine = "Test Item",
            BaseType = "Test Base",
            Width = 1,
            Height = 1,
            Verified = true,
            Identified = true,
            ItemLevel = 75,
            FrameType = 2,
            Icon = "",
            ExplicitMods = new List<Mod>
            {
                new() { RawModText = "Adds 15 to 25 Physical Damage", ModText = "Adds # to # Physical Damage", Values = new List<double> { 15, 25 } },
                new() { RawModText = "+12% increased Attack Speed", ModText = "+#% increased Attack Speed", Values = new List<double> { 12 } }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(item, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"explicitMods\":["));
        Assert.IsTrue(json.Contains("\"Adds 15 to 25 Physical Damage\""));
        // The + character is escaped as \u002B in JSON
        Assert.IsTrue(json.Contains("\"\\u002B12% increased Attack Speed\"") || json.Contains("\"+12% increased Attack Speed\""));
        Assert.IsFalse(json.Contains("modText")); // Should not include internal properties
        Assert.IsFalse(json.Contains("values"));
    }

    [TestMethod]
    public void TradeItem_RoundTripSerialization_ShouldMaintainModData()
    {
        // Arrange
        var originalJson = """
        {
            "id": "test-item",
            "league": "Standard",
            "typeLine": "Test Weapon",
            "baseType": "Test Base",
            "w": 2,
            "h": 4,
            "verified": true,
            "identified": true,
            "ilvl": 80,
            "frameType": 2,
            "icon": "",
            "implicitMods": [
                "+10% increased Movement Speed"
            ],
            "explicitMods": [
                "Adds 15 to 25 Physical Damage",
                "+12% increased Attack Speed"
            ]
        }
        """;

        // Act
        var item = JsonSerializer.Deserialize<TradeItem>(originalJson, JsonOptions);
        var serializedJson = JsonSerializer.Serialize(item, JsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<TradeItem>(serializedJson, JsonOptions);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNotNull(deserializedItem);
        
        // Verify implicit mods
        Assert.IsNotNull(item.ImplicitMods);
        Assert.IsNotNull(deserializedItem.ImplicitMods);
        Assert.AreEqual(item.ImplicitMods.Count, deserializedItem.ImplicitMods.Count);
        Assert.AreEqual(item.ImplicitMods[0].RawModText, deserializedItem.ImplicitMods[0].RawModText);
        
        // Verify explicit mods
        Assert.IsNotNull(item.ExplicitMods);
        Assert.IsNotNull(deserializedItem.ExplicitMods);
        Assert.AreEqual(item.ExplicitMods.Count, deserializedItem.ExplicitMods.Count);
        Assert.AreEqual(item.ExplicitMods[0].RawModText, deserializedItem.ExplicitMods[0].RawModText);
        Assert.AreEqual(item.ExplicitMods[1].RawModText, deserializedItem.ExplicitMods[1].RawModText);
    }

    [TestMethod]
    public async Task TradeItem_DeserializeFromRealJsonExamples_ShouldParseModsCorrectly()
    {
        // Arrange - Test with a real example file
        var filePath = Path.Combine("TestData", "Responses", "PoE1", "Weapon.json");
        if (!File.Exists(filePath))
        {
            Assert.Inconclusive("Test data file not found: " + filePath);
        }
        
        var jsonContent = await File.ReadAllTextAsync(filePath);
        Assert.IsFalse(string.IsNullOrWhiteSpace(jsonContent), "JSON file should not be empty");

        // Act
        var response = JsonSerializer.Deserialize<TradeFetchResponse>(jsonContent, JsonOptions);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.IsTrue(response.Result.Count > 0, "Should have search results");
        
        // Find an item with explicit mods to test
        var itemWithMods = response.Result
            .Select(r => r.Item)
            .FirstOrDefault(item => item.ExplicitMods != null && item.ExplicitMods.Count > 0);
        
        if (itemWithMods != null)
        {
            Assert.IsNotNull(itemWithMods.ExplicitMods);
            Assert.IsTrue(itemWithMods.ExplicitMods.Count > 0, "Should have explicit mods");
            
            // Validate the first explicit mod
            var firstMod = itemWithMods.ExplicitMods[0];
            Assert.IsFalse(string.IsNullOrEmpty(firstMod.RawModText), "Mod should have raw text");
            Assert.IsFalse(string.IsNullOrEmpty(firstMod.ModText), "Mod should have processed text");
            
            // If the mod has numeric values, verify they were extracted
            if (firstMod.Values.Count > 0)
            {
                Assert.IsTrue(firstMod.ModText.Contains("#"), "Processed text should contain placeholders");
                Assert.IsNotNull(firstMod.ValueString, "Should have value string if values exist");
            }
        }
    }

    [TestMethod]
    public void TradeItem_WithModsContainingSpecialCharacters_ShouldParseCorrectly()
    {
        // Arrange
        var json = """
        {
            "id": "test-item",
            "league": "Standard",
            "typeLine": "Test Item",
            "baseType": "Test Base",
            "w": 1,
            "h": 1,
            "verified": true,
            "identified": true,
            "ilvl": 75,
            "frameType": 2,
            "icon": "",
            "explicitMods": [
                "Minions deal 15% increased Damage",
                "+25% to Fire Resistance",
                "Cannot be Frozen",
                "Recover 3% of Life on Kill"
            ]
        }
        """;

        // Act
        var item = JsonSerializer.Deserialize<TradeItem>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNotNull(item.ExplicitMods);
        Assert.AreEqual(4, item.ExplicitMods.Count);
        
        // Check mod with percentage at end
        var firstMod = item.ExplicitMods[0];
        Assert.AreEqual("Minions deal 15% increased Damage", firstMod.RawModText);
        Assert.AreEqual("Minions deal #% increased Damage", firstMod.ModText);
        Assert.AreEqual(1, firstMod.Values.Count);
        Assert.AreEqual(15, firstMod.Values[0]);
        
        // Check mod with + prefix
        var secondMod = item.ExplicitMods[1];
        Assert.AreEqual("+25% to Fire Resistance", secondMod.RawModText);
        Assert.AreEqual("+#% to Fire Resistance", secondMod.ModText);
        Assert.AreEqual(1, secondMod.Values.Count);
        Assert.AreEqual(25, secondMod.Values[0]);
        
        // Check mod without numbers
        var thirdMod = item.ExplicitMods[2];
        Assert.AreEqual("Cannot be Frozen", thirdMod.RawModText);
        Assert.AreEqual("Cannot be Frozen", thirdMod.ModText);
        Assert.AreEqual(0, thirdMod.Values.Count);
        
        // Check mod with decimal
        var fourthMod = item.ExplicitMods[3];
        Assert.AreEqual("Recover 3% of Life on Kill", fourthMod.RawModText);
        Assert.AreEqual("Recover #% of Life on Kill", fourthMod.ModText);
        Assert.AreEqual(1, fourthMod.Values.Count);
        Assert.AreEqual(3, fourthMod.Values[0]);
    }
}
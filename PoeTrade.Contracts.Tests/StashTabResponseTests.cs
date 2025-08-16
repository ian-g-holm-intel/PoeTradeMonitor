namespace PoeTrade.Contracts.Tests;

[TestClass]
public class StashTabResponseTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void StashTabResponse_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var request = new StashTabResponse();

        // Assert
        Assert.AreEqual(0, request.NumberOfTabs);
        Assert.IsNotNull(request.Items);
        Assert.AreEqual(0, request.Items.Count);
    }

    [TestMethod]
    public void StashTabResponse_WithInitializer_ShouldSetProperties()
    {
        // Arrange
        var items = new List<TradeItem>
        {
            new() { Id = "test-id", League = "Test League" }
        };

        // Act
        var request = new StashTabResponse
        {
            NumberOfTabs = 5,
            Items = items
        };

        // Assert
        Assert.AreEqual(5, request.NumberOfTabs);
        Assert.AreEqual(1, request.Items.Count);
        Assert.AreEqual("test-id", request.Items[0].Id);
        Assert.AreEqual("Test League", request.Items[0].League);
    }

    [TestMethod]
    public void StashTabResponse_Serialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var request = new StashTabResponse
        {
            NumberOfTabs = 10,
            Items = new List<TradeItem>
            {
                new() { Id = "item1", League = "Standard" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"numTabs\":10"));
        Assert.IsTrue(json.Contains("\"items\":["));
    }

    [TestMethod]
    public void StashTabResponse_Deserialization_ShouldMapFromJson()
    {
        // Arrange
        var json = """
        {
            "numTabs": 15,
            "items": [
                {
                    "id": "test-item-id",
                    "league": "Hardcore",
                    "typeLine": "Test Item",
                    "baseType": "Test Base",
                    "w": 1,
                    "h": 1,
                    "verified": true,
                    "identified": true,
                    "ilvl": 50,
                    "frameType": 1,
                    "icon": ""
                }
            ]
        }
        """;

        // Act
        var request = JsonSerializer.Deserialize<StashTabResponse>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(request);
        Assert.AreEqual(15, request.NumberOfTabs);
        Assert.AreEqual(1, request.Items.Count);
        Assert.AreEqual("test-item-id", request.Items[0].Id);
        Assert.AreEqual("Hardcore", request.Items[0].League);
    }

    [TestMethod]
    public async Task StashTabResponse_DeserializeFromStashItemsJson_ShouldWork()
    {
        // Arrange
        var filePath = Path.Combine("TestData", "Responses", "PoE1", "StashItems.json");
        Assert.IsTrue(File.Exists(filePath), $"Test file not found: {filePath}");
        
        var jsonContent = await File.ReadAllTextAsync(filePath);
        Assert.IsFalse(string.IsNullOrWhiteSpace(jsonContent), "JSON file should not be empty");

        // Act
        var request = JsonSerializer.Deserialize<StashTabResponse>(jsonContent, JsonOptions);

        // Assert
        Assert.IsNotNull(request);
        Assert.IsTrue(request.NumberOfTabs > 0, "Number of tabs should be greater than 0");
        Assert.IsNotNull(request.Items);
        Assert.IsTrue(request.Items.Count > 0, "Items collection should not be empty");
        
        // Validate the first item
        var firstItem = request.Items[0];
        Assert.IsFalse(string.IsNullOrEmpty(firstItem.Id), "First item should have an ID");
        Assert.IsFalse(string.IsNullOrEmpty(firstItem.League), "First item should have a league");
        Assert.IsTrue(firstItem.Width > 0, "First item should have positive width");
        Assert.IsTrue(firstItem.Height > 0, "First item should have positive height");
    }

    [TestMethod]
    public void StashTabResponse_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var items = new List<TradeItem> { new() { Id = "test" } };
        
        var request1 = new StashTabResponse { NumberOfTabs = 5, Items = items };
        var request2 = new StashTabResponse { NumberOfTabs = 5, Items = items };
        var request3 = new StashTabResponse { NumberOfTabs = 10, Items = items };

        // Act & Assert
        Assert.AreEqual(request1, request2);
        Assert.AreNotEqual(request1, request3);
        Assert.AreEqual(request1.GetHashCode(), request2.GetHashCode());
    }

    [TestMethod]
    public void StashTabResponse_ToString_ShouldReturnValidString()
    {
        // Arrange
        var request = new StashTabResponse
        {
            NumberOfTabs = 7,
            Items = new List<TradeItem> { new() { Id = "test" } }
        };

        // Act
        var result = request.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("StashTabResponse"));
        Assert.IsTrue(result.Contains("NumberOfTabs = 7"));
    }

    [TestMethod]
    public void StashTabResponse_WithNullItems_ShouldHandleGracefully()
    {
        // Arrange
        var json = """
        {
            "numTabs": 5,
            "items": null
        }
        """;

        // Act
        var request = JsonSerializer.Deserialize<StashTabResponse>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(request);
        Assert.AreEqual(5, request.NumberOfTabs);
        Assert.IsNull(request.Items);
    }

    [TestMethod]
    public void StashTabResponse_WithEmptyItems_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "numTabs": 3,
            "items": []
        }
        """;

        // Act
        var request = JsonSerializer.Deserialize<StashTabResponse>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(request);
        Assert.AreEqual(3, request.NumberOfTabs);
        Assert.IsNotNull(request.Items);
        Assert.AreEqual(0, request.Items.Count);
    }

    [TestMethod]
    public void StashTabResponse_WithCurrencyLayout_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "numTabs": 5,
            "currencyLayout": {
                "sections": ["general", "influence"],
                "layout": {
                    "0": {
                        "section": "general",
                        "x": 308.519,
                        "y": 48.073,
                        "w": 1,
                        "h": 1,
                        "scale": 0.8077
                    }
                }
            },
            "items": []
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StashTabResponse>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(5, response.NumberOfTabs);
        Assert.IsNotNull(response.CurrencyLayout);
        Assert.AreEqual(2, response.CurrencyLayout.Sections.Count);
        Assert.IsTrue(response.CurrencyLayout.Sections.Contains("general"));
        Assert.IsTrue(response.CurrencyLayout.Sections.Contains("influence"));
        Assert.AreEqual(1, response.CurrencyLayout.Layout.Count);
        Assert.IsTrue(response.CurrencyLayout.Layout.ContainsKey("0"));
        
        var layoutItem = response.CurrencyLayout.Layout["0"];
        Assert.AreEqual("general", layoutItem.Section);
        Assert.AreEqual(308.519, layoutItem.X, 0.001);
        Assert.AreEqual(48.073, layoutItem.Y, 0.001);
        Assert.AreEqual(1, layoutItem.Width);
        Assert.AreEqual(1, layoutItem.Height);
        Assert.AreEqual(0.8077, layoutItem.Scale, 0.0001);
    }

    [TestMethod]
    public void StashTabResponse_WithoutCurrencyLayout_ShouldHaveNullLayout()
    {
        // Arrange
        var json = """
        {
            "numTabs": 3,
            "items": []
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StashTabResponse>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(3, response.NumberOfTabs);
        Assert.IsNull(response.CurrencyLayout);
    }

    [TestMethod]
    public void StashTabResponse_CurrencyLayoutSerialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var response = new StashTabResponse
        {
            NumberOfTabs = 5,
            CurrencyLayout = new CurrencyLayout
            {
                Sections = new List<string> { "general", "influence" },
                Layout = new Dictionary<string, CurrencyLayoutItem>
                {
                    ["0"] = new CurrencyLayoutItem
                    {
                        Section = "general",
                        X = 100.5,
                        Y = 200.25,
                        Width = 2,
                        Height = 1,
                        Scale = 0.75
                    }
                }
            },
            Items = new List<TradeItem>()
        };

        // Act
        var json = JsonSerializer.Serialize(response, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"currencyLayout\""));
        Assert.IsTrue(json.Contains("\"sections\""));
        Assert.IsTrue(json.Contains("\"layout\""));
        Assert.IsTrue(json.Contains("\"section\":\"general\""));
        Assert.IsTrue(json.Contains("\"x\":100.5"));
        Assert.IsTrue(json.Contains("\"y\":200.25"));
        Assert.IsTrue(json.Contains("\"w\":2"));
        Assert.IsTrue(json.Contains("\"h\":1"));
        Assert.IsTrue(json.Contains("\"scale\":0.75"));
    }

    [TestMethod]
    public void CurrencyLayout_DefaultConstructor_ShouldInitializeCollections()
    {
        // Act
        var layout = new CurrencyLayout();

        // Assert
        Assert.IsNotNull(layout.Sections);
        Assert.AreEqual(0, layout.Sections.Count);
        Assert.IsNotNull(layout.Layout);
        Assert.AreEqual(0, layout.Layout.Count);
    }

    [TestMethod]
    public void CurrencyLayoutItem_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var item1 = new CurrencyLayoutItem
        {
            Section = "general",
            X = 100.0,
            Y = 200.0,
            Width = 1,
            Height = 1,
            Scale = 0.8
        };
        
        var item2 = new CurrencyLayoutItem
        {
            Section = "general",
            X = 100.0,
            Y = 200.0,
            Width = 1,
            Height = 1,
            Scale = 0.8
        };
        
        var item3 = new CurrencyLayoutItem
        {
            Section = "influence",
            X = 100.0,
            Y = 200.0,
            Width = 1,
            Height = 1,
            Scale = 0.8
        };

        // Act & Assert
        Assert.AreEqual(item1, item2);
        Assert.AreNotEqual(item1, item3);
        Assert.AreEqual(item1.GetHashCode(), item2.GetHashCode());
    }

    [TestMethod]
    public async Task StashTabResponse_DeserializeFromUpdatedStashItemsJson_ShouldIncludeCurrencyLayout()
    {
        // Arrange
        var filePath = Path.Combine("TestData", "Responses", "PoE1", "StashItems.json");
        Assert.IsTrue(File.Exists(filePath), $"Test file not found: {filePath}");
        
        var jsonContent = await File.ReadAllTextAsync(filePath);
        Assert.IsFalse(string.IsNullOrWhiteSpace(jsonContent), "JSON file should not be empty");

        // Act
        var response = JsonSerializer.Deserialize<StashTabResponse>(jsonContent, JsonOptions);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsTrue(response.NumberOfTabs > 0, "Number of tabs should be greater than 0");
        Assert.IsNotNull(response.CurrencyLayout, "Currency layout should not be null");
        Assert.IsNotNull(response.CurrencyLayout.Sections, "Currency layout sections should not be null");
        Assert.IsTrue(response.CurrencyLayout.Sections.Count > 0, "Currency layout should have sections");
        Assert.IsNotNull(response.CurrencyLayout.Layout, "Currency layout items should not be null");
        Assert.IsTrue(response.CurrencyLayout.Layout.Count > 0, "Currency layout should have layout items");
        
        // Validate first layout item
        var firstLayoutKey = response.CurrencyLayout.Layout.Keys.First();
        var firstLayoutItem = response.CurrencyLayout.Layout[firstLayoutKey];
        Assert.IsFalse(string.IsNullOrEmpty(firstLayoutItem.Section), "Layout item should have a section");
        Assert.IsTrue(firstLayoutItem.Width > 0, "Layout item should have positive width");
        Assert.IsTrue(firstLayoutItem.Height > 0, "Layout item should have positive height");
        Assert.IsTrue(firstLayoutItem.Scale > 0, "Layout item should have positive scale");
    }
}
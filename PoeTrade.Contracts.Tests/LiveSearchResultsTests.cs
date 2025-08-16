using System.Text.Json;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class LiveSearchResultsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void LiveSearchResults_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var results = new LiveSearchResults();

        // Assert
        Assert.IsNull(results.Auth);
        Assert.IsNotNull(results.Ids);
        Assert.AreEqual(0, results.Ids.Count);
    }

    [TestMethod]
    public void LiveSearchResults_WithInitializer_ShouldSetProperties()
    {
        // Arrange & Act
        var results = new LiveSearchResults
        {
            Auth = true,
            Ids = ["item1", "item2", "item3"]
        };

        // Assert
        Assert.IsTrue(results.Auth);
        Assert.AreEqual(3, results.Ids.Count);
        Assert.AreEqual("item1", results.Ids[0]);
    }

    [TestMethod]
    public void LiveSearchResults_JsonDeserialization_ShouldWork()
    {
        // Arrange
        var json = """
        {
            "auth": true,
            "new": ["id1", "id2"]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<LiveSearchResults>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Auth);
        Assert.AreEqual(2, result.Ids.Count);
        Assert.AreEqual("id1", result.Ids[0]);
        Assert.AreEqual("id2", result.Ids[1]);
    }

    [TestMethod]
    public void LiveSearchResults_JsonSerialization_ShouldWork()
    {
        // Arrange
        var results = new LiveSearchResults { Auth = false, Ids = ["test"] };

        // Act
        var json = JsonSerializer.Serialize(results, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<LiveSearchResults>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.IsFalse(deserialized.Auth);
        Assert.AreEqual(1, deserialized.Ids.Count);
    }
}
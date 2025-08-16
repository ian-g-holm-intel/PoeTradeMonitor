using System.Text.Json;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ItemExtendedConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new ItemExtendedConverter() }
    };

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandleValidObject()
    {
        // Arrange
        var json = """
        {
            "base_defence_percentile": 85,
            "ar": 100,
            "es": 200,
            "ev": 150
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(85, result.BaseDefencePercentile);
        Assert.AreEqual(100, result.Armour);
        Assert.AreEqual(200, result.EnergyShield);
        Assert.AreEqual(150, result.Evasion);
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandleEmptyObject()
    {
        // Arrange
        var json = """{}""";

        // Act
        var result = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        // All properties should have default values (0 for ints, null for nullable ints)
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandleEmptyArray()
    {
        // Arrange - POE API sometimes returns [] instead of {} for empty extended data
        var json = """[]""";

        // Act
        var result = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);

        // Assert
        Assert.IsNull(result); // Empty array should result in null
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandleNullValue()
    {
        // Arrange
        var json = """null""";

        // Act
        var result = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandlePartialObject()
    {
        // Arrange
        var json = """
        {
            "base_defence_percentile": 75,
            "ar": 120
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(75, result.BaseDefencePercentile);
        Assert.AreEqual(120, result.Armour);
        Assert.IsNull(result.EnergyShield); // Should be null for missing values
        Assert.IsNull(result.Evasion); // Should be null for missing values
    }

    [TestMethod]
    public void ItemExtendedConverter_Write_ShouldSerializeCorrectly()
    {
        // Arrange
        var extended = new ItemExtended
        {
            BaseDefencePercentile = 90,
            Armour = 250,
            EnergyShield = 180,
            Evasion = 200
        };

        // Act
        var json = JsonSerializer.Serialize(extended, JsonOptions);

        // Assert
        Assert.IsNotNull(json);
        var deserialized = JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(90, deserialized.BaseDefencePercentile);
        Assert.AreEqual(250, deserialized.Armour);
        Assert.AreEqual(180, deserialized.EnergyShield);
        Assert.AreEqual(200, deserialized.Evasion);
    }

    [TestMethod]
    public void ItemExtendedConverter_Write_ShouldHandleNullInput()
    {
        // Arrange
        ItemExtended? extended = null;

        // Act
        var json = JsonSerializer.Serialize(extended, JsonOptions);

        // Assert
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldHandleInvalidJsonType()
    {
        // Arrange
        var json = """"invalid-type"""";

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions));
    }

    [TestMethod]
    public void ItemExtendedConverter_Read_ShouldThrowForStringNumbers()
    {
        // Arrange - String numbers should throw exception since no string-to-number conversion is configured
        var json = """
        {
            "base_defence_percentile": "85",
            "ar": "100"
        }
        """;

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<ItemExtended>(json, JsonOptions));
    }
}
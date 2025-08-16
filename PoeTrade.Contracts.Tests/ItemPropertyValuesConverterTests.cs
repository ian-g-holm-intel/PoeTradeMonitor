using System.Text.Json;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ItemPropertyValuesConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new ItemPropertyValuesConverter() }
    };

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldHandleArrayOfArrays()
    {
        // Arrange
        var json = """[["10", 0], ["20", 1]]""";

        // Act
        var result = JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("10", result[0].Value);
        Assert.AreEqual(0, result[0].Index);
        Assert.AreEqual("20", result[1].Value);
        Assert.AreEqual(1, result[1].Index);
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldHandleNullValue()
    {
        // Arrange
        var json = """[["null", 0], ["test", 1]]""";

        // Act
        var result = JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("null", result[0].Value);
        Assert.AreEqual(0, result[0].Index);
        Assert.AreEqual("test", result[1].Value);
        Assert.AreEqual(1, result[1].Index);
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldHandleEmptyArray()
    {
        // Arrange
        var json = """[]""";

        // Act
        var result = JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldThrowForInvalidFormat()
    {
        // Arrange
        var json = """["invalid", "format"]""";

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions));
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Write_ShouldSerializeCorrectly()
    {
        // Arrange
        var values = new List<ItemPropertyValue>
        {
            new() { Value = "10", Index = 0 },
            new() { Value = "20", Index = 1 }
        };

        // Act
        var json = JsonSerializer.Serialize(values, JsonOptions);

        // Assert
        Assert.IsNotNull(json);
        var deserialized = JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Count);
        Assert.AreEqual("10", deserialized[0].Value);
        Assert.AreEqual("20", deserialized[1].Value);
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldHandleMultipleArrays()
    {
        // Arrange
        var json = """[["first", 0], ["second", 5], ["third", 2]]""";

        // Act
        var result = JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("first", result[0].Value);
        Assert.AreEqual(0, result[0].Index);
        Assert.AreEqual("second", result[1].Value);
        Assert.AreEqual(5, result[1].Index);
        Assert.AreEqual("third", result[2].Value);
        Assert.AreEqual(2, result[2].Index);
    }

    [TestMethod]
    public void ItemPropertyValuesConverter_Read_ShouldHandleInvalidJson()
    {
        // Arrange
        var json = """not-valid-json""";

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<List<ItemPropertyValue>>(json, JsonOptions));
    }
}
using System.Text.Json;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class StringOrNumberConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new StringOrNumberConverter() }
    };

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleStringValue()
    {
        // Arrange
        var json = "\"test-string\"";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("test-string", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleIntegerNumber()
    {
        // Arrange
        var json = """42""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleFloatingPointNumber()
    {
        // Arrange
        var json = """42.5""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("42.5", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleNegativeNumber()
    {
        // Arrange
        var json = """-15""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("-15", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleZero()
    {
        // Arrange
        var json = """0""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleEmptyString()
    {
        // Arrange
        var json = "\"\"";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleNullValue()
    {
        // Arrange
        var json = """null""";

        // Act
        var result = JsonSerializer.Deserialize<string?>(json, JsonOptions);

        // Assert
        Assert.IsNull(result); // Standard JSON deserializer handles null before converter
    }

    [TestMethod]
    public void StringOrNumberConverter_Write_ShouldSerializeString()
    {
        // Arrange
        var value = "test-value";

        // Act
        var json = JsonSerializer.Serialize(value, JsonOptions);

        // Assert
        Assert.AreEqual("\"test-value\"", json);
    }

    [TestMethod]
    public void StringOrNumberConverter_Write_ShouldSerializeNumericString()
    {
        // Arrange
        var value = "123";

        // Act
        var json = JsonSerializer.Serialize(value, JsonOptions);

        // Assert
        Assert.AreEqual("\"123\"", json);
    }

    [TestMethod]
    public void StringOrNumberConverter_Write_ShouldHandleNullValue()
    {
        // Arrange
        string? value = null;

        // Act
        var json = JsonSerializer.Serialize(value, JsonOptions);

        // Assert
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleLargeNumber()
    {
        // Arrange
        var json = """999999999999""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("999999999999", result);
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldHandleScientificNotation()
    {
        // Arrange
        var json = """1.23e10""";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

        // Assert
        Assert.AreEqual("12300000000", result); // Converter uses GetDecimal().ToString() which expands scientific notation
    }

    [TestMethod]
    public void StringOrNumberConverter_Read_ShouldThrowForBoolean()
    {
        // Arrange
        var json = """true""";

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<string>(json, JsonOptions));
    }

    [TestMethod]
    public void StringOrNumberConverter_RoundTrip_ShouldPreserveStringValues()
    {
        // Arrange
        var testValues = new[] { "test", "123", "45.67", "-89", "0", "" };

        foreach (var value in testValues)
        {
            // Act
            var json = JsonSerializer.Serialize(value, JsonOptions);
            var result = JsonSerializer.Deserialize<string>(json, JsonOptions);

            // Assert
            Assert.AreEqual(value, result, $"Round-trip failed for value: {value}");
        }
    }
}
namespace PoeTrade.Contracts.Tests;

[TestClass]
public class TradeCurrencyTypeConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new TradeCurrencyTypeConverter() }
    };

    [TestMethod]
    public void TradeCurrencyTypeConverter_Read_ShouldConvertValidStringToEnum()
    {
        // Arrange
        var json = "\"chaos\"";

        // Act
        var result = JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions);

        // Assert
        Assert.AreEqual(TradeCurrencyType.Chaos, result);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_Write_ShouldConvertEnumToValidString()
    {
        // Arrange
        var currencyType = TradeCurrencyType.Exalted;

        // Act
        var json = JsonSerializer.Serialize(currencyType, JsonOptions);

        // Assert
        Assert.AreEqual("\"exalted\"", json);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_Read_ShouldReturnUnknownForInvalidString()
    {
        // Arrange
        var json = "\"invalid-currency\"";

        // Act
        var result = JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions);

        // Assert
        Assert.AreEqual(TradeCurrencyType.Unknown, result);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_Read_ShouldThrowForNullString()
    {
        // Arrange - When deserializing a nullable TradeCurrencyType, null is handled by the framework
        // But when deserializing a non-nullable TradeCurrencyType, it should throw an exception
        var json = "null";

        // Act & Assert
        var exception = Assert.ThrowsExactly<JsonException>(() =>
            JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions));
        
        // The actual exception message may vary, but it should be a JsonException
        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_Read_ShouldThrowForEmptyString()
    {
        // Arrange
        var json = "\"\"";

        // Act & Assert
        var exception = Assert.ThrowsExactly<JsonException>(() =>
            JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions));
        
        Assert.IsTrue(exception.Message.Contains("TradeCurrencyType value cannot be null or empty"));
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_Read_ShouldThrowForNonStringValue()
    {
        // Arrange
        var json = "123";

        // Act & Assert
        var exception = Assert.ThrowsExactly<JsonException>(() =>
            JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions));
        
        Assert.IsTrue(exception.Message.Contains("Expected string value for TradeCurrencyType"));
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_GetStringValue_ShouldReturnCorrectString()
    {
        // Act
        var result = TradeCurrencyTypeConverter.GetStringValue(TradeCurrencyType.Divine);

        // Assert
        Assert.AreEqual("divine", result);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_GetEnumValue_ShouldReturnCorrectEnum()
    {
        // Act
        var result = TradeCurrencyTypeConverter.GetEnumValue("mirror");

        // Assert
        Assert.AreEqual(TradeCurrencyType.Mirror, result);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_GetEnumValue_ShouldReturnNullForInvalidString()
    {
        // Act
        var result = TradeCurrencyTypeConverter.GetEnumValue("invalid-currency");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_RoundTripConversion_ShouldMaintainValue()
    {
        // Arrange
        var originalValues = new[]
        {
            TradeCurrencyType.Chaos,
            TradeCurrencyType.Exalted,
            TradeCurrencyType.Divine,
            TradeCurrencyType.Mirror,
            TradeCurrencyType.Alt,
            TradeCurrencyType.Fusing,
            TradeCurrencyType.Chrome
        };

        foreach (var originalValue in originalValues)
        {
            // Act - Serialize then deserialize
            var json = JsonSerializer.Serialize(originalValue, JsonOptions);
            var deserializedValue = JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions);

            // Assert
            Assert.AreEqual(originalValue, deserializedValue, $"Round trip failed for {originalValue}");
        }
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_ComplexCurrencyTypes_ShouldWorkCorrectly()
    {
        // Test some of the more complex currency type names
        var testCases = new Dictionary<TradeCurrencyType, string>
        {
            { TradeCurrencyType.InfusedEngineersOrb, "infused-engineers-orb" },
            { TradeCurrencyType.EldritchChaosOrb, "eldritch-chaos-orb" },
            { TradeCurrencyType.TaintedJewellersOrb, "tainted-jewellers-orb" },
            { TradeCurrencyType.MavensChiselOfProcurement, "mavens-chisel-of-procurement" },
            { TradeCurrencyType.ExceptionalEldritchEmber, "exceptional-eldritch-ember" }
        };

        foreach (var testCase in testCases)
        {
            // Test deserialization
            var json = $"\"{testCase.Value}\"";
            var deserializedEnum = JsonSerializer.Deserialize<TradeCurrencyType>(json, JsonOptions);
            Assert.AreEqual(testCase.Key, deserializedEnum, $"Deserialization failed for {testCase.Value}");

            // Test serialization
            var serializedJson = JsonSerializer.Serialize(testCase.Key, JsonOptions);
            Assert.AreEqual(json, serializedJson, $"Serialization failed for {testCase.Key}");
        }
    }

    [TestMethod]
    public void PriceInfo_WithTradeCurrencyType_ShouldSerializeCorrectly()
    {
        // Arrange
        var priceInfo = new PriceInfo
        {
            PriceType = "~price",
            Amount = 50.5m,
            CurrencyType = TradeCurrencyType.Chaos,
            IsRelative = false
        };

        // Act
        var json = JsonSerializer.Serialize(priceInfo, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"type\":\"~price\""));
        Assert.IsTrue(json.Contains("\"amount\":50.5"));
        Assert.IsTrue(json.Contains("\"currency\":\"chaos\""));
        Assert.IsFalse(json.Contains("IsRelative"), "IsRelative should not be serialized due to JsonIgnore");
    }

    [TestMethod]
    public void PriceInfo_WithTradeCurrencyType_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "type": "~exact",
            "amount": 1.5,
            "currency": "exalted"
        }
        """;

        // Act
        var priceInfo = JsonSerializer.Deserialize<PriceInfo>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(priceInfo);
        Assert.AreEqual("~exact", priceInfo.PriceType);
        Assert.AreEqual(1.5m, priceInfo.Amount);
        Assert.AreEqual(TradeCurrencyType.Exalted, priceInfo.CurrencyType);
        Assert.IsFalse(priceInfo.IsRelative); // Default value
    }

    [TestMethod]
    public void PriceInfo_WithInvalidCurrencyType_ShouldReturnUnknown()
    {
        // Arrange
        var json = """
        {
            "type": "~price",
            "amount": 1.0,
            "currency": "invalid-currency"
        }
        """;

        // Act
        var priceInfo = JsonSerializer.Deserialize<PriceInfo>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(priceInfo);
        Assert.AreEqual("~price", priceInfo.PriceType);
        Assert.AreEqual(1.0m, priceInfo.Amount);
        Assert.AreEqual(TradeCurrencyType.Unknown, priceInfo.CurrencyType);
    }

    [TestMethod]
    public void PriceInfo_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var priceInfo = new PriceInfo();

        // Assert
        Assert.AreEqual(string.Empty, priceInfo.PriceType);
        Assert.AreEqual(0m, priceInfo.Amount);
        Assert.AreEqual(default(TradeCurrencyType), priceInfo.CurrencyType);
        Assert.IsFalse(priceInfo.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var priceInfo1 = new PriceInfo
        {
            PriceType = "~exact",
            Amount = 100m,
            CurrencyType = TradeCurrencyType.Divine
        };

        var priceInfo2 = new PriceInfo
        {
            PriceType = "~exact",
            Amount = 100m,
            CurrencyType = TradeCurrencyType.Divine
        };

        var priceInfo3 = new PriceInfo
        {
            PriceType = "~exact",
            Amount = 100m,
            CurrencyType = TradeCurrencyType.Chaos
        };

        // Act & Assert
        Assert.AreEqual(priceInfo1, priceInfo2);
        Assert.AreNotEqual(priceInfo1, priceInfo3);
        Assert.AreEqual(priceInfo1.GetHashCode(), priceInfo2.GetHashCode());
    }

    [TestMethod]
    public void PriceInfo_ToString_ShouldReturnValidString()
    {
        // Arrange
        var priceInfo = new PriceInfo
        {
            PriceType = "~price",
            Amount = 1m,
            CurrencyType = TradeCurrencyType.Mirror
        };

        // Act
        var result = priceInfo.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1 Mirror", result); // Custom ToString returns "Amount CurrencyType"
    }

    [TestMethod]
    public void PriceInfo_ToString_WithRelativePrice_ShouldReturnValidString()
    {
        // Arrange
        var priceInfo = new PriceInfo
        {
            PriceType = "~price",
            Amount = 2.5m,
            CurrencyType = TradeCurrencyType.Chaos,
            IsRelative = true
        };

        // Act
        var result = priceInfo.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("(2.5 Chaos)", result); // Relative prices are wrapped in parentheses
    }

    [TestMethod]
    public void TradeCurrencyTypeConverter_AllEnumValues_ShouldHaveCorrespondingAttributes()
    {
        // Arrange
        var enumValues = Enum.GetValues<TradeCurrencyType>();

        // Act & Assert
        foreach (var enumValue in enumValues)
        {
            var stringValue = TradeCurrencyTypeConverter.GetStringValue(enumValue);
            Assert.IsNotNull(stringValue, $"Enum value {enumValue} should have a corresponding TradeCurrencyTypeAttribute");
            
            var backToEnum = TradeCurrencyTypeConverter.GetEnumValue(stringValue);
            Assert.AreEqual(enumValue, backToEnum, $"Round trip conversion failed for {enumValue}");
        }
    }
}
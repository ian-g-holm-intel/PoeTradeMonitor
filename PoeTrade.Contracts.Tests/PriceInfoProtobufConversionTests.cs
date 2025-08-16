using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PriceInfoProtobufConversionTests
{
    [TestMethod]
    public void PriceInfo_ToProtobuf_ShouldConvertCorrectly()
    {
        // Arrange
        var priceInfo = new PriceInfo
        {
            PriceType = "~price",
            Amount = 50.5m,
            CurrencyType = TradeCurrencyType.Chaos,
            IsRelative = true
        };

        // Act
        var proto = priceInfo.ToProtobuf();

        // Assert
        Assert.IsNotNull(proto);
        Assert.AreEqual("~price", proto.PriceType);
        Assert.AreEqual(50.5, proto.Amount, 0.001);
        Assert.AreEqual("chaos", proto.CurrencyType);
        Assert.IsTrue(proto.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_ToRecord_ShouldConvertCorrectly()
    {
        // Arrange
        var proto = new Proto.PriceInfo
        {
            PriceType = "~exact",
            Amount = 1.0,
            CurrencyType = "exalted",
            IsRelative = false
        };

        // Act
        var record = proto.ToRecord();

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual("~exact", record.PriceType);
        Assert.AreEqual(1.0m, record.Amount);
        Assert.AreEqual(TradeCurrencyType.Exalted, record.CurrencyType);
        Assert.IsFalse(record.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_RoundTripConversion_ShouldMaintainData()
    {
        // Arrange
        var originalPriceInfo = new PriceInfo
        {
            PriceType = "~b/o",
            Amount = 2.75m,
            CurrencyType = TradeCurrencyType.Divine,
            IsRelative = false
        };

        // Act
        var proto = originalPriceInfo.ToProtobuf();
        var convertedBack = proto.ToRecord();

        // Assert
        Assert.AreEqual(originalPriceInfo.PriceType, convertedBack.PriceType);
        Assert.AreEqual(originalPriceInfo.Amount, convertedBack.Amount);
        Assert.AreEqual(originalPriceInfo.CurrencyType, convertedBack.CurrencyType);
        Assert.AreEqual(originalPriceInfo.IsRelative, convertedBack.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_WithUnknownCurrency_ShouldHandleCorrectly()
    {
        // Arrange
        var proto = new Proto.PriceInfo
        {
            PriceType = "~price",
            Amount = 100.0,
            CurrencyType = "unknown-currency",
            IsRelative = false
        };

        // Act
        var record = proto.ToRecord();

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual("~price", record.PriceType);
        Assert.AreEqual(100.0m, record.Amount);
        Assert.AreEqual(TradeCurrencyType.Unknown, record.CurrencyType);
        Assert.IsFalse(record.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_WithEmptyCurrencyString_ShouldHandleCorrectly()
    {
        // Arrange
        var proto = new Proto.PriceInfo
        {
            PriceType = "~price",
            Amount = 10.0,
            CurrencyType = "",
            IsRelative = true
        };

        // Act
        var record = proto.ToRecord();

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual("~price", record.PriceType);
        Assert.AreEqual(10.0m, record.Amount);
        Assert.AreEqual(TradeCurrencyType.Unknown, record.CurrencyType);
        Assert.IsTrue(record.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_WithComplexCurrencyTypes_ShouldConvertCorrectly()
    {
        // Test some complex currency types
        var testCases = new[]
        {
            TradeCurrencyType.EldritchChaosOrb,
            TradeCurrencyType.MavensChiselOfProcurement,
            TradeCurrencyType.TaintedJewellersOrb,
            TradeCurrencyType.ExceptionalEldritchEmber
        };

        foreach (var currencyType in testCases)
        {
            // Arrange
            var priceInfo = new PriceInfo
            {
                PriceType = "~exact",
                Amount = 1.0m,
                CurrencyType = currencyType,
                IsRelative = false
            };

            // Act
            var proto = priceInfo.ToProtobuf();
            var convertedBack = proto.ToRecord();

            // Assert
            Assert.AreEqual(currencyType, convertedBack.CurrencyType, $"Failed for currency type: {currencyType}");
            Assert.AreEqual(priceInfo.PriceType, convertedBack.PriceType);
            Assert.AreEqual(priceInfo.Amount, convertedBack.Amount);
            Assert.AreEqual(priceInfo.IsRelative, convertedBack.IsRelative);
        }
    }

    [TestMethod]
    public void PriceInfo_WithDefaultValues_ShouldConvertCorrectly()
    {
        // Arrange
        var priceInfo = new PriceInfo(); // Default values

        // Act
        var proto = priceInfo.ToProtobuf();
        var convertedBack = proto.ToRecord();

        // Assert
        Assert.AreEqual(string.Empty, convertedBack.PriceType);
        Assert.AreEqual(0m, convertedBack.Amount);
        Assert.AreEqual(TradeCurrencyType.Unknown, convertedBack.CurrencyType); // Default enum value
        Assert.IsFalse(convertedBack.IsRelative);
    }

    [TestMethod]
    public void PriceInfo_LargeCurrencyAmounts_ShouldMaintainPrecision()
    {
        // Arrange
        var testAmounts = new[] { 0.001m, 999999.999m, 1234567.89m };

        foreach (var amount in testAmounts)
        {
            var priceInfo = new PriceInfo
            {
                PriceType = "~price",
                Amount = amount,
                CurrencyType = TradeCurrencyType.Mirror,
                IsRelative = false
            };

            // Act
            var proto = priceInfo.ToProtobuf();
            var convertedBack = proto.ToRecord();

            // Assert - Allow for small floating point precision differences
            Assert.AreEqual(amount, convertedBack.Amount, 0.01m, $"Failed for amount: {amount}");
        }
    }

    [TestMethod]
    public void PriceInfo_WithAllPriceTypes_ShouldConvertCorrectly()
    {
        // Test various price type strings
        var priceTypes = new[] { "~price", "~exact", "~b/o", "~c/o", "" };

        foreach (var priceType in priceTypes)
        {
            // Arrange
            var priceInfo = new PriceInfo
            {
                PriceType = priceType,
                Amount = 42.0m,
                CurrencyType = TradeCurrencyType.Chaos,
                IsRelative = false
            };

            // Act
            var proto = priceInfo.ToProtobuf();
            var convertedBack = proto.ToRecord();

            // Assert
            Assert.AreEqual(priceType, convertedBack.PriceType, $"Failed for price type: '{priceType}'");
            Assert.AreEqual(priceInfo.Amount, convertedBack.Amount);
            Assert.AreEqual(priceInfo.CurrencyType, convertedBack.CurrencyType);
            Assert.AreEqual(priceInfo.IsRelative, convertedBack.IsRelative);
        }
    }

    [TestMethod]
    public void PriceInfo_RelativeVsAbsolute_ShouldMaintainState()
    {
        // Test both relative and absolute pricing
        var isRelativeValues = new[] { true, false };

        foreach (var isRelative in isRelativeValues)
        {
            // Arrange
            var priceInfo = new PriceInfo
            {
                PriceType = "~price",
                Amount = 5.0m,
                CurrencyType = TradeCurrencyType.Chaos,
                IsRelative = isRelative
            };

            // Act
            var proto = priceInfo.ToProtobuf();
            var convertedBack = proto.ToRecord();

            // Assert
            Assert.AreEqual(isRelative, convertedBack.IsRelative, $"Failed for IsRelative: {isRelative}");
        }
    }
}
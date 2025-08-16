using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class RequestProtobufFilterConversionTests
{
    [TestMethod]
    public void RangeFilter_ShouldConvertCorrectly()
    {
        // Arrange
        var rangeFilter = new RangeFilter { Min = 10, Max = 20 };

        // Act
        var proto = rangeFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(rangeFilter.Min, converted.Min);
        Assert.AreEqual(rangeFilter.Max, converted.Max);
    }

    [TestMethod]
    public void RangeFilter_WithNulls_ShouldConvertCorrectly()
    {
        // Arrange - Min only
        var minOnlyFilter = new RangeFilter { Min = 5, Max = null };

        // Act
        var proto = minOnlyFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(minOnlyFilter.Min, converted.Min);
        Assert.AreEqual(minOnlyFilter.Max, converted.Max);

        // Arrange - Max only
        var maxOnlyFilter = new RangeFilter { Min = null, Max = 15 };

        // Act
        proto = maxOnlyFilter.ToProtobuf();
        converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(maxOnlyFilter.Min, converted.Min);
        Assert.AreEqual(maxOnlyFilter.Max, converted.Max);
    }

    [TestMethod]
    public void RangeFilter_WithBothNulls_ShouldConvertCorrectly()
    {
        // Arrange
        var emptyFilter = new RangeFilter { Min = null, Max = null };

        // Act
        var proto = emptyFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.IsNull(converted.Min);
        Assert.IsNull(converted.Max);
    }

    [TestMethod]
    public void OptionFilter_ShouldConvertCorrectly()
    {
        // Arrange
        var optionFilter = new OptionFilter { Option = "unique" };

        // Act
        var proto = optionFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(optionFilter.Option, converted.Option);
    }

    [TestMethod]
    public void OptionFilter_WithNullOption_ShouldConvertCorrectly()
    {
        // Arrange
        var optionFilter = new OptionFilter { Option = null };

        // Act
        var proto = optionFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.IsNull(converted.Option);
    }

    [TestMethod]
    public void PriceFilter_ShouldConvertCorrectly()
    {
        // Arrange
        var priceFilter = new PriceFilter { Option = "divine", Min = 1, Max = 100 };

        // Act
        var proto = priceFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(priceFilter.Option, converted.Option);
        Assert.AreEqual(priceFilter.Min, converted.Min);
        Assert.AreEqual(priceFilter.Max, converted.Max);
    }

    [TestMethod]
    public void PriceFilter_WithPartialData_ShouldConvertCorrectly()
    {
        // Arrange - Only option and min
        var priceFilter = new PriceFilter { Option = "chaos", Min = 10, Max = null };

        // Act
        var proto = priceFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(priceFilter.Option, converted.Option);
        Assert.AreEqual(priceFilter.Min, converted.Min);
        Assert.IsNull(converted.Max);
    }

    [TestMethod]
    public void InputFilter_ShouldConvertCorrectly()
    {
        // Arrange
        var inputFilter = new InputFilter { Input = "testuser" };

        // Act
        var proto = inputFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.AreEqual(inputFilter.Input, converted.Input);
    }

    [TestMethod]
    public void InputFilter_WithNullInput_ShouldConvertCorrectly()
    {
        // Arrange
        var inputFilter = new InputFilter { Input = null };

        // Act
        var proto = inputFilter.ToProtobuf();
        var converted = proto.ToRecord();

        // Assert
        Assert.IsNull(converted.Input);
    }
}
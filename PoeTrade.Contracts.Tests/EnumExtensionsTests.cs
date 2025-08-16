using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class EnumExtensionsTests
{
    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForChaos()
    {
        // Arrange
        var currency = TradeCurrencyType.Chaos;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Chaos Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForDivine()
    {
        // Arrange
        var currency = TradeCurrencyType.Divine;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Divine Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForExalted()
    {
        // Arrange
        var currency = TradeCurrencyType.Exalted;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Exalted Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForChrome()
    {
        // Arrange
        var currency = TradeCurrencyType.Chrome;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Chromatic Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForJewellers()
    {
        // Arrange
        var currency = TradeCurrencyType.Jewellers;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Jeweller's Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForFusing()
    {
        // Arrange
        var currency = TradeCurrencyType.Fusing;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Orb of Fusing", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForAlch()
    {
        // Arrange
        var currency = TradeCurrencyType.Alch;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Orb of Alchemy", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForChisel()
    {
        // Arrange
        var currency = TradeCurrencyType.Chisel;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Cartographer's Chisel", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForVaal()
    {
        // Arrange
        var currency = TradeCurrencyType.Vaal;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Vaal Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForRegret()
    {
        // Arrange
        var currency = TradeCurrencyType.Regret;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Orb of Regret", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForGcp()
    {
        // Arrange
        var currency = TradeCurrencyType.Gcp;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Gemcutter's Prism", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnDescriptionForBlessed()
    {
        // Arrange
        var currency = TradeCurrencyType.Blessed;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Blessed Orb", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldReturnEnumNameForUnknown()
    {
        // Arrange
        var currency = TradeCurrencyType.Unknown;

        // Act
        var result = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual("Unknown Currency Type", result);
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldHandleAllDefinedCurrencies()
    {
        // Arrange & Act & Assert
        foreach (TradeCurrencyType currency in Enum.GetValues<TradeCurrencyType>())
        {
            var description = currency.GetCurrencyDescription();
            
            Assert.IsNotNull(description, $"Description should not be null for {currency}");
            Assert.IsTrue(description.Length > 0, $"Description should not be empty for {currency}");
        }
    }

    [TestMethod]
    public void GetCurrencyDescription_ShouldBeConsistent()
    {
        // Arrange
        var currency = TradeCurrencyType.Chaos;

        // Act
        var result1 = currency.GetCurrencyDescription();
        var result2 = currency.GetCurrencyDescription();

        // Assert
        Assert.AreEqual(result1, result2);
    }
}
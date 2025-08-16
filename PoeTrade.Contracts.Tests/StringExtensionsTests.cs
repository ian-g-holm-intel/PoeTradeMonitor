using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void RemoveDiacritics_ShouldHandleRegularText()
    {
        // Arrange
        var input = "Hello World";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldRemoveAccentsFromFrenchText()
    {
        // Arrange
        var input = "café résumé naïve";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("cafe resume naive", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldRemoveAccentsFromSpanishText()
    {
        // Arrange
        var input = "niño señorita";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("nino senorita", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldRemoveAccentsFromGermanText()
    {
        // Arrange
        var input = "Mädchen Größe";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        // ß is eszett, a distinct German letter, not ß with diacritic
        Assert.AreEqual("Madchen Große", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleWhitespaceOnly()
    {
        // Arrange
        var input = "   \t\n  ";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("   \t\n  ", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleNumbersAndSymbols()
    {
        // Arrange
        var input = "123!@#$%^&*()";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("123!@#$%^&*()", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldRemoveVariousDiacritics()
    {
        // Arrange
        var input = "àáâãäåāăąèéêëēėęìíîïīįòóôõöøōőùúûüūűůũýÿżžź";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        // Note: ø is a distinct letter in Nordic languages, not just o with diacritic
        Assert.AreEqual("aaaaaaaaaeeeeeeeiiiiiioooooøoouuuuuuuuyyzzz", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleUppercaseDiacritics()
    {
        // Arrange
        var input = "ÀÁÂÃÄÅĀĂĄÈÉÊËĒĖĘÌÍÎÏĪĮÒÓÔÕÖØŌŐÙÚÛÜŪŰŮŨÝŸŻŽŹ";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        // Note: Ø is a distinct letter in Nordic languages
        Assert.AreEqual("AAAAAAAAAEEEEEEEIIIIIIOOOOOØOOUUUUUUUUYYZZZ", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleMixedCase()
    {
        // Arrange
        var input = "Café RÉSUMÉ Naïve";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("Cafe RESUME Naive", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleCyrillicText()
    {
        // Arrange
        var input = "Москва";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual("Москва", result); // Cyrillic should remain unchanged
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandlePathOfExileItemNames()
    {
        // Arrange - Common POE item names that might have accents
        var input = "Atziri's Disfavour";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        // Apostrophe is not a diacritic so it should remain
        Assert.AreEqual("Atziri's Disfavour", result);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleComplexDiacritics()
    {
        // Arrange
        var input = "ñç₡₢₣₤₥₦₧₨₩₪₫€₭₮₯₰₱₲₳₴₵₶₷₸₹₺₻₼₽₾₿";

        // Act
        var result = input.RemoveDiacritics();

        // Assert
        // Should remove combining marks but preserve base characters and currency symbols
        Assert.IsTrue(result.Contains("nc"));
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldBeConsistent()
    {
        // Arrange
        var input = "café";

        // Act
        var result1 = input.RemoveDiacritics();
        var result2 = input.RemoveDiacritics();

        // Assert
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void RemoveDiacritics_ShouldHandleNormalizedInput()
    {
        // Arrange
        var input1 = "é"; // Single character with accent
        var input2 = "e\u0301"; // e + combining acute accent

        // Act
        var result1 = input1.RemoveDiacritics();
        var result2 = input2.RemoveDiacritics();

        // Assert
        Assert.AreEqual("e", result1);
        Assert.AreEqual("e", result2);
        Assert.AreEqual(result1, result2);
    }
}
namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ItemPropertyValueTests
{
    [TestMethod]
    public void ItemPropertyValue_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var propertyValue = new ItemPropertyValue();

        // Assert
        Assert.IsNull(propertyValue.Value);
        Assert.AreEqual(0, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_WithInitializer_ShouldSetProperties()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "test-value",
            Index = 5
        };

        // Assert
        Assert.AreEqual("test-value", propertyValue.Value);
        Assert.AreEqual(5, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldAllowNullValue()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = null,
            Index = 1
        };

        // Assert
        Assert.IsNull(propertyValue.Value);
        Assert.AreEqual(1, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldAllowEmptyStringValue()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = string.Empty,
            Index = 2
        };

        // Assert
        Assert.AreEqual(string.Empty, propertyValue.Value);
        Assert.AreEqual(2, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldAllowNegativeIndex()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "test",
            Index = -1
        };

        // Assert
        Assert.AreEqual("test", propertyValue.Value);
        Assert.AreEqual(-1, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandleNumericStringValues()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "123",
            Index = 0
        };

        // Assert
        Assert.AreEqual("123", propertyValue.Value);
        Assert.AreEqual(0, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandleComplexStringValues()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "12-24 Physical Damage",
            Index = 0
        };

        // Assert
        Assert.AreEqual("12-24 Physical Damage", propertyValue.Value);
        Assert.AreEqual(0, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandleSpecialCharacters()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "+15% to Fire Resistance",
            Index = 1
        };

        // Assert
        Assert.AreEqual("+15% to Fire Resistance", propertyValue.Value);
        Assert.AreEqual(1, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandlePathOfExileItemNames()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "Atziri's Disfavour",
            Index = 0
        };

        // Assert
        Assert.AreEqual("Atziri's Disfavour", propertyValue.Value);
        Assert.AreEqual(0, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var propertyValue1 = new ItemPropertyValue { Value = "test", Index = 1 };
        var propertyValue2 = new ItemPropertyValue { Value = "test", Index = 1 };
        var propertyValue3 = new ItemPropertyValue { Value = "different", Index = 1 };

        // Act & Assert
        Assert.AreEqual(propertyValue1, propertyValue2);
        Assert.AreNotEqual(propertyValue1, propertyValue3);
        Assert.IsTrue(propertyValue1.Equals(propertyValue2));
        Assert.IsFalse(propertyValue1.Equals(propertyValue3));
    }

    [TestMethod]
    public void ItemPropertyValue_RecordHashCode_ShouldBeConsistent()
    {
        // Arrange
        var propertyValue1 = new ItemPropertyValue { Value = "test", Index = 1 };
        var propertyValue2 = new ItemPropertyValue { Value = "test", Index = 1 };

        // Act & Assert
        Assert.AreEqual(propertyValue1.GetHashCode(), propertyValue2.GetHashCode());
    }

    [TestMethod]
    public void ItemPropertyValue_ToString_ShouldReturnExpectedFormat()
    {
        // Arrange
        var propertyValue = new ItemPropertyValue { Value = "test", Index = 1 };

        // Act
        var result = propertyValue.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("test"));
        Assert.IsTrue(result.Contains("1"));
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldAllowPropertyModification()
    {
        // Arrange
        var propertyValue = new ItemPropertyValue { Value = "initial", Index = 0 };

        // Act
        propertyValue.Value = "modified";
        propertyValue.Index = 5;

        // Assert
        Assert.AreEqual("modified", propertyValue.Value);
        Assert.AreEqual(5, propertyValue.Index);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandleWhitespaceValues()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "   whitespace   ",
            Index = 0
        };

        // Assert
        Assert.AreEqual("   whitespace   ", propertyValue.Value);
    }

    [TestMethod]
    public void ItemPropertyValue_ShouldHandleUnicodeValues()
    {
        // Arrange & Act
        var propertyValue = new ItemPropertyValue
        {
            Value = "café résumé ñoño",
            Index = 0
        };

        // Assert
        Assert.AreEqual("café résumé ñoño", propertyValue.Value);
    }
}
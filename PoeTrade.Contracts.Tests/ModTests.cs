namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ModTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void Mod_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var mod = new Mod();

        // Assert
        Assert.AreEqual(string.Empty, mod.RawModText);
        Assert.AreEqual(string.Empty, mod.ModText);
        Assert.IsNotNull(mod.Values);
        Assert.AreEqual(0, mod.Values.Count);
        Assert.IsNull(mod.ValueString);
    }

    [TestMethod]
    public void Mod_WithInitializer_ShouldSetProperties()
    {
        // Arrange
        var values = new List<double> { 10.5, 20.0 };
        
        // Act
        var mod = new Mod
        {
            RawModText = "+15% increased Attack Speed",
            ModText = "+#% increased Attack Speed",
            Values = values,
            ValueString = "15"
        };

        // Assert
        Assert.AreEqual("+15% increased Attack Speed", mod.RawModText);
        Assert.AreEqual("+#% increased Attack Speed", mod.ModText);
        Assert.AreEqual(2, mod.Values.Count);
        Assert.AreEqual(10.5, mod.Values[0]);
        Assert.AreEqual(20.0, mod.Values[1]);
        Assert.AreEqual("15", mod.ValueString);
    }

    [TestMethod]
    public void Mod_ToString_ShouldReturnModText()
    {
        // Arrange
        var mod = new Mod { ModText = "+#% increased Damage" };

        // Act
        var result = mod.ToString();

        // Assert
        Assert.AreEqual("+#% increased Damage", result);
    }

    [TestMethod]
    public void Mod_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var values1 = new List<double> { 15 };
        var values2 = new List<double> { 15 };
        
        var mod1 = new Mod
        {
            RawModText = "+15% increased Attack Speed",
            ModText = "+#% increased Attack Speed",
            Values = values1
        };
        
        var mod2 = new Mod
        {
            RawModText = "+15% increased Attack Speed",
            ModText = "+#% increased Attack Speed",
            Values = values2
        };
        
        var mod3 = new Mod
        {
            RawModText = "+20% increased Attack Speed",
            ModText = "+#% increased Attack Speed",
            Values = new List<double> { 20 }
        };

        // Act & Assert
        // Note: Records with different list instances are not equal even if content is the same
        Assert.AreNotEqual(mod1, mod2); // Different list instances
        Assert.AreNotEqual(mod1, mod3);
        
        // Test content equality instead
        Assert.AreEqual(mod1.RawModText, mod2.RawModText);
        Assert.AreEqual(mod1.ModText, mod2.ModText);
        Assert.IsTrue(mod1.Values.SequenceEqual(mod2.Values));
    }
}

[TestClass]
public class ModListConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new ModListConverter() }
    };

    [TestMethod]
    public void ModListConverter_Read_ShouldParseSimpleMod()
    {
        // Arrange
        var json = """["Adds 15 to 25 Physical Damage"]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        
        var mod = mods[0];
        Assert.AreEqual("Adds 15 to 25 Physical Damage", mod.RawModText);
        Assert.AreEqual("Adds # to # Physical Damage", mod.ModText);
        Assert.AreEqual(2, mod.Values.Count);
        Assert.AreEqual(15, mod.Values[0]);
        Assert.AreEqual(25, mod.Values[1]);
        Assert.AreEqual("15 25", mod.ValueString);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldParseModWithDecimals()
    {
        // Arrange
        var json = """["+12.5% increased Critical Strike Chance"]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        
        var mod = mods[0];
        Assert.AreEqual("+12.5% increased Critical Strike Chance", mod.RawModText);
        Assert.AreEqual("+#% increased Critical Strike Chance", mod.ModText);
        Assert.AreEqual(1, mod.Values.Count);
        Assert.AreEqual(12.5, mod.Values[0]);
        Assert.AreEqual("12.5", mod.ValueString);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldParseModWithoutNumbers()
    {
        // Arrange
        var json = """["Cannot be Frozen"]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        
        var mod = mods[0];
        Assert.AreEqual("Cannot be Frozen", mod.RawModText);
        Assert.AreEqual("Cannot be Frozen", mod.ModText);
        Assert.AreEqual(0, mod.Values.Count);
        Assert.IsNull(mod.ValueString);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldParseMultipleMods()
    {
        // Arrange
        var json = """
        [
            "Adds 15 to 25 Physical Damage",
            "+10% increased Attack Speed",
            "Cannot be Frozen"
        ]
        """;

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(3, mods.Count);
        
        // First mod
        Assert.AreEqual("Adds 15 to 25 Physical Damage", mods[0].RawModText);
        Assert.AreEqual("Adds # to # Physical Damage", mods[0].ModText);
        Assert.AreEqual(2, mods[0].Values.Count);
        
        // Second mod
        Assert.AreEqual("+10% increased Attack Speed", mods[1].RawModText);
        Assert.AreEqual("+#% increased Attack Speed", mods[1].ModText);
        Assert.AreEqual(1, mods[1].Values.Count);
        Assert.AreEqual(10, mods[1].Values[0]);
        
        // Third mod
        Assert.AreEqual("Cannot be Frozen", mods[2].RawModText);
        Assert.AreEqual("Cannot be Frozen", mods[2].ModText);
        Assert.AreEqual(0, mods[2].Values.Count);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldHandleNullValue()
    {
        // Arrange
        var json = "null";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNull(mods);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldHandleEmptyArray()
    {
        // Arrange
        var json = "[]";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(0, mods.Count);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldIgnoreEmptyStrings()
    {
        // Arrange
        var json = """["", "+10% increased Damage", ""]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        Assert.AreEqual("+10% increased Damage", mods[0].RawModText);
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldHandleQuestionMarks()
    {
        // Arrange
        var json = """["+10% increased Damage?"]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        
        var mod = mods[0];
        Assert.AreEqual("+10% increased Damage?", mod.RawModText);
        Assert.AreEqual("+#% increased Damage", mod.ModText); // Should have question mark removed
    }

    [TestMethod]
    public void ModListConverter_Write_ShouldSerializeToStringArray()
    {
        // Arrange
        var mods = new List<Mod>
        {
            new() { RawModText = "Adds 15 to 25 Physical Damage", ModText = "Adds # to # Physical Damage", Values = new List<double> { 15, 25 } },
            new() { RawModText = "+10% increased Attack Speed", ModText = "+#% increased Attack Speed", Values = new List<double> { 10 } }
        };

        // Act
        var json = JsonSerializer.Serialize(mods, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"Adds 15 to 25 Physical Damage\""), $"JSON should contain mod string: {json}");
        // The + character is escaped as \u002B in JSON
        Assert.IsTrue(json.Contains("\"\\u002B10% increased Attack Speed\"") || json.Contains("\"+10% increased Attack Speed\""), $"JSON should contain mod string: {json}");
        Assert.IsFalse(json.Contains("modText")); // Should not serialize internal properties
        Assert.IsFalse(json.Contains("values"));
    }

    [TestMethod]
    public void ModListConverter_Write_ShouldHandleNullValue()
    {
        // Arrange
        List<Mod>? mods = null;

        // Act
        var json = JsonSerializer.Serialize(mods, JsonOptions);

        // Assert
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void ModListConverter_Write_ShouldHandleEmptyList()
    {
        // Arrange
        var mods = new List<Mod>();

        // Act
        var json = JsonSerializer.Serialize(mods, JsonOptions);

        // Assert
        Assert.AreEqual("[]", json);
    }

    [TestMethod]
    public void ModListConverter_RoundTrip_ShouldMaintainData()
    {
        // Arrange
        var originalJson = """
        [
            "Adds 15 to 25 Physical Damage",
            "+12.5% increased Critical Strike Chance",
            "Cannot be Frozen"
        ]
        """;

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(originalJson, JsonOptions);
        var serializedJson = JsonSerializer.Serialize(mods, JsonOptions);
        var deserializedMods = JsonSerializer.Deserialize<List<Mod>?>(serializedJson, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.IsNotNull(deserializedMods);
        Assert.AreEqual(mods.Count, deserializedMods.Count);
        
        for (int i = 0; i < mods.Count; i++)
        {
            Assert.AreEqual(mods[i].RawModText, deserializedMods[i].RawModText);
            // Note: ModText and Values are computed during parsing, so they should match after round trip
        }
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldThrowOnInvalidJson()
    {
        // Arrange
        var json = """{"invalid": "structure"}""";

        // Act & Assert
        Assert.ThrowsExactly<JsonException>(() => 
            JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions));
    }

    [TestMethod]
    public void ModListConverter_Read_ShouldParseComplexMod()
    {
        // Arrange
        var json = """["+(20-30) to maximum Life and +(10-15) to maximum Energy Shield"]""";

        // Act
        var mods = JsonSerializer.Deserialize<List<Mod>?>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(mods);
        Assert.AreEqual(1, mods.Count);
        
        var mod = mods[0];
        Assert.AreEqual("+(20-30) to maximum Life and +(10-15) to maximum Energy Shield", mod.RawModText);
        Assert.AreEqual("+(#-#) to maximum Life and +(#-#) to maximum Energy Shield", mod.ModText);
        Assert.AreEqual(4, mod.Values.Count);
        Assert.AreEqual(20, mod.Values[0]);
        Assert.AreEqual(30, mod.Values[1]);
        Assert.AreEqual(10, mod.Values[2]);
        Assert.AreEqual(15, mod.Values[3]);
    }
}
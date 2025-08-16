namespace PoeTrade.Contracts.Tests;

[TestClass]
public class LeagueListResponseTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void League_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var league = new League();

        // Assert
        Assert.AreEqual(string.Empty, league.Id);
        Assert.AreEqual(string.Empty, league.Name);
        Assert.AreEqual(string.Empty, league.Realm);
        Assert.AreEqual(string.Empty, league.Url);
        Assert.IsNull(league.StartAt);
        Assert.IsNull(league.EndAt);
    }

    [TestMethod]
    public void League_WithInitializer_ShouldSetProperties()
    {
        // Act
        var league = new League
        {
            Id = "Standard",
            Name = "Standard",
            Realm = "pc",
            Url = "https://www.pathofexile.com/ladders/league/Standard",
            StartAt = "2013-01-23T21:00:00Z",
            EndAt = null
        };

        // Assert
        Assert.AreEqual("Standard", league.Id);
        Assert.AreEqual("Standard", league.Name);
        Assert.AreEqual("pc", league.Realm);
        Assert.AreEqual("https://www.pathofexile.com/ladders/league/Standard", league.Url);
        Assert.AreEqual("2013-01-23T21:00:00Z", league.StartAt);
        Assert.IsNull(league.EndAt);
    }

    [TestMethod]
    public void League_Serialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var league = new League
        {
            Id = "Hardcore",
            Name = "Hardcore",
            Realm = "pc",
            Url = "https://www.pathofexile.com/ladders/league/Hardcore",
            StartAt = "2013-01-23T21:00:00Z",
            EndAt = null
        };

        // Act
        var json = JsonSerializer.Serialize(league, JsonOptions);

        // Assert
        Assert.IsTrue(json.Contains("\"id\":\"Hardcore\""));
        Assert.IsTrue(json.Contains("\"name\":\"Hardcore\""));
        Assert.IsTrue(json.Contains("\"realm\":\"pc\""));
        Assert.IsTrue(json.Contains("\"url\":\"https://www.pathofexile.com/ladders/league/Hardcore\""));
        Assert.IsTrue(json.Contains("\"startAt\":\"2013-01-23T21:00:00Z\""));
        Assert.IsTrue(json.Contains("\"endAt\":null"));
    }

    [TestMethod]
    public void League_Deserialization_ShouldMapFromJson()
    {
        // Arrange
        var json = """
        {
            "id": "Mercenaries",
            "name": "Mercenaries",
            "realm": "pc",
            "url": "https://www.pathofexile.com/ladders/league/Mercenaries",
            "startAt": "2025-06-13T20:00:00Z",
            "endAt": null
        }
        """;

        // Act
        var league = JsonSerializer.Deserialize<League>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(league);
        Assert.AreEqual("Mercenaries", league.Id);
        Assert.AreEqual("Mercenaries", league.Name);
        Assert.AreEqual("pc", league.Realm);
        Assert.AreEqual("https://www.pathofexile.com/ladders/league/Mercenaries", league.Url);
        Assert.AreEqual("2025-06-13T20:00:00Z", league.StartAt);
        Assert.IsNull(league.EndAt);
    }

    [TestMethod]
    public void League_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var league1 = new League { Id = "Standard", Name = "Standard", Realm = "pc" };
        var league2 = new League { Id = "Standard", Name = "Standard", Realm = "pc" };
        var league3 = new League { Id = "Hardcore", Name = "Hardcore", Realm = "pc" };

        // Act & Assert
        Assert.AreEqual(league1, league2);
        Assert.AreNotEqual(league1, league3);
        Assert.AreEqual(league1.GetHashCode(), league2.GetHashCode());
    }

    [TestMethod]
    public void League_ToString_ShouldReturnValidString()
    {
        // Arrange
        var league = new League
        {
            Id = "Standard",
            Name = "Standard",
            Realm = "pc"
        };

        // Act
        var result = league.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("League"));
        Assert.IsTrue(result.Contains("Id = Standard"));
    }

    [TestMethod]
    public void League_WithNullDates_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "id": "Ruthless",
            "name": "Ruthless",
            "realm": "pc",
            "url": "https://www.pathofexile.com/ladders/league/Ruthless",
            "startAt": null,
            "endAt": null
        }
        """;

        // Act
        var league = JsonSerializer.Deserialize<League>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(league);
        Assert.AreEqual("Ruthless", league.Id);
        Assert.AreEqual("Ruthless", league.Name);
        Assert.IsNull(league.StartAt);
        Assert.IsNull(league.EndAt);
    }
}
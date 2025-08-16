using System.Text.Json;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ItemWhisperRequestTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [TestMethod]
    public void ItemWhisperRequest_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var request = new ItemWhisperRequest();

        // Assert
        Assert.AreEqual(string.Empty, request.Token);
        Assert.IsNotNull(request.Values);
        Assert.AreEqual(0, request.Values.Count);
    }

    [TestMethod]
    public void ItemWhisperRequest_WithInitializer_ShouldSetProperties()
    {
        // Arrange & Act
        var request = new ItemWhisperRequest
        {
            Token = "test-token-12345",
            Values = [1, 2, 3]
        };

        // Assert
        Assert.AreEqual("test-token-12345", request.Token);
        Assert.AreEqual(3, request.Values.Count);
        Assert.AreEqual(1, request.Values[0]);
    }

    [TestMethod]
    public void ItemWhisperRequest_JsonSerialization_ShouldWork()
    {
        // Arrange
        var request = new ItemWhisperRequest
        {
            Token = "test-token",
            Values = [1, 2]
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ItemWhisperRequest>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual("test-token", deserialized.Token);
        Assert.AreEqual(2, deserialized.Values.Count);
    }

    [TestMethod]
    public void ItemWhisperRequest_ShouldAllowPropertyModification()
    {
        // Arrange
        var request = new ItemWhisperRequest { Token = "initial" };

        // Act
        request.Token = "modified";
        request.Values.Add(5);

        // Assert
        Assert.AreEqual("modified", request.Token);
        Assert.AreEqual(1, request.Values.Count);
        Assert.AreEqual(5, request.Values[0]);
    }
}
namespace PoeTrade.Contracts.Tests;

[TestClass]
public class TradeSearchResponseTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task TradeSearchResponse_DeserializeFromSearchResponseJson_ShouldWork()
    {
        // Arrange & Act
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SearchResponse.json");

        // Assert
        ValidateBasicTradeSearchResponse(response, "SearchResponse.json");
        
        // Verify we have exactly 2 results as per our test data
        Assert.AreEqual(2, response.Result.Count, "Should have exactly 2 results");
        
        // Validate each result
        foreach (var result in response.Result)
        {
            ValidateTradeSearchResult(result, "SearchResponse.json");
            ValidateTradeListing(result.Listing, "SearchResponse.json");
            ValidateTradeItem(result.Item, "SearchResponse.json");
            
            // Verify that Indexed is properly deserialized as DateTime
            Assert.IsTrue(result.Listing.Indexed > DateTime.MinValue, "Indexed should be a valid DateTime");
            Assert.AreEqual(DateTimeKind.Utc, result.Listing.Indexed.Kind, "Indexed should be UTC");
        }
    }

    [TestMethod]
    public async Task TradeListing_IndexedProperty_ShouldDeserializeAsDateTime()
    {
        // Arrange & Act
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SearchResponse.json");
        var firstListing = response.Result.First().Listing;

        // Assert
        Assert.IsInstanceOfType<DateTime>(firstListing.Indexed, "Indexed should be of type DateTime");
        Assert.IsTrue(firstListing.Indexed > DateTime.MinValue, "Indexed should have a valid DateTime value");
        Assert.AreEqual(DateTimeKind.Utc, firstListing.Indexed.Kind, "Indexed DateTime should be UTC");
        
        // Test specific expected date from our test JSON (2025-08-12T17:37:23Z)
        var expectedDate = new DateTime(2025, 8, 12, 17, 37, 23, DateTimeKind.Utc);
        Assert.AreEqual(expectedDate, firstListing.Indexed, "Should match the expected DateTime from JSON");
    }

    [TestMethod]
    public async Task TradeListing_IndexedProperty_ShouldHandleMultipleDateFormats()
    {
        // Arrange & Act  
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SearchResponse.json");
        var listings = response.Result.Select(r => r.Listing).ToList();

        // Assert - Test that both dates in our test file are parsed correctly
        Assert.AreEqual(2, listings.Count, "Should have 2 listings to test");
        
        // First listing: 2025-08-12T17:37:23Z
        var firstExpected = new DateTime(2025, 8, 12, 17, 37, 23, DateTimeKind.Utc);
        Assert.AreEqual(firstExpected, listings[0].Indexed, "First listing should have correct DateTime");
        
        // Second listing: 2025-08-12T15:22:18Z
        var secondExpected = new DateTime(2025, 8, 12, 15, 22, 18, DateTimeKind.Utc);
        Assert.AreEqual(secondExpected, listings[1].Indexed, "Second listing should have correct DateTime");
        
        // Verify both are UTC
        Assert.AreEqual(DateTimeKind.Utc, listings[0].Indexed.Kind, "First date should be UTC");
        Assert.AreEqual(DateTimeKind.Utc, listings[1].Indexed.Kind, "Second date should be UTC");
    }

    [TestMethod]
    public async Task TradeListing_IndexedProperty_ShouldPreserveOriginalTimezone()
    {
        // Arrange & Act
        var response = await DeserializeJsonFileAsync("Responses/PoE1/SearchResponse.json");
        var listing = response.Result.First().Listing;

        // Assert - Test that UTC timezone is preserved
        Assert.AreEqual(DateTimeKind.Utc, listing.Indexed.Kind, "DateTime should maintain UTC kind");
        
        // Test that the time value matches what's expected from the JSON
        var expectedUtcTime = new DateTime(2025, 8, 12, 17, 37, 23, DateTimeKind.Utc);
        Assert.AreEqual(expectedUtcTime, listing.Indexed, "UTC time should match expected value");
    }

    [TestMethod]
    public async Task TradeListing_SerializationRoundTrip_ShouldMaintainDateTime()
    {
        // Arrange
        var originalResponse = await DeserializeJsonFileAsync("Responses/PoE1/SearchResponse.json");
        var originalListing = originalResponse.Result.First().Listing;
        var originalIndexed = originalListing.Indexed;

        // Act - Serialize and deserialize
        var json = JsonSerializer.Serialize(originalListing, JsonOptions);
        var deserializedListing = JsonSerializer.Deserialize<TradeListing>(json, JsonOptions);

        // Assert
        Assert.IsNotNull(deserializedListing, "Deserialized listing should not be null");
        Assert.AreEqual(originalIndexed, deserializedListing.Indexed, "Indexed DateTime should be preserved during round-trip serialization");
        Assert.AreEqual(originalIndexed.Kind, deserializedListing.Indexed.Kind, "DateTime Kind should be preserved during round-trip serialization");
    }
}
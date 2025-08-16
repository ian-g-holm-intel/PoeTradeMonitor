using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE2RequestProtobufConversionTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task PoE2_FullYes_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE2/Full_Yes.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Basic structure should match
        Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option);
        Assert.AreEqual(originalRequest.Query.Stats.Count, convertedRequest.Query.Stats.Count);
        Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price);

        // PoE2-specific type filter properties should match
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.ItemLevel?.Min,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.ItemLevel?.Min);
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.ItemLevel?.Max,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.ItemLevel?.Max);
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.Quality?.Min,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.Quality?.Min);
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.Quality?.Max,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.Quality?.Max);
    }

    [TestMethod]
    public async Task PoE2_MinNo_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE2/Min_No.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Basic structure should be preserved
        Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option);
        Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price);

        // Verify PoE2-specific properties are preserved
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.Category?.Option,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.Category?.Option);
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Option,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Option);
    }

    [TestMethod]
    public async Task PoE2_MaxAny_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE2/Max_Any.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Basic structure should be preserved
        Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option);
        Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price);
    }

    [TestMethod]
    public async Task AllPoE2Requests_ShouldConvertToProtobufAndBack()
    {
        var requestFiles = new[]
        {
            "Requests/PoE2/Full_Yes.json",
            "Requests/PoE2/Min_No.json",
            "Requests/PoE2/Max_Any.json"
        };

        foreach (var fileName in requestFiles)
        {
            // Arrange
            var originalRequest = await DeserializeRequestFileAsync(fileName);

            // Act - Convert to protobuf and back
            var protoRequest = originalRequest.ToProtobuf();
            var convertedRequest = protoRequest.ToRecord();

            // Assert - Basic structure should be preserved
            Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option, 
                           $"Status option should match for {fileName}");
            Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price, 
                           $"Sort price should match for {fileName}");
            
            // Type filters should be preserved
            if (originalRequest.Query.Filters.TypeFilters?.Filters.Category != null)
            {
                Assert.AreEqual(originalRequest.Query.Filters.TypeFilters!.Filters.Category!.Option,
                               convertedRequest.Query.Filters.TypeFilters?.Filters.Category?.Option,
                               $"Category should match for {fileName}");
            }

            // PoE2-specific filters should not have PoE1 data
            Assert.IsNull(convertedRequest.Query.Filters.WeaponFilters, 
                         $"PoE2 converted request {fileName} should not have weapon filters");
            Assert.IsNull(convertedRequest.Query.Filters.ArmourFilters, 
                         $"PoE2 converted request {fileName} should not have armour filters");
            Assert.IsNull(convertedRequest.Query.Filters.SocketFilters, 
                         $"PoE2 converted request {fileName} should not have socket filters");
        }
    }

    #region Helper Methods

    private async Task<TradeSearchRequest> DeserializeRequestFileAsync(string fileName)
    {
        var jsonContent = await File.ReadAllTextAsync(Path.Combine("TestData", fileName));
        var request = JsonSerializer.Deserialize<TradeSearchRequest>(jsonContent, JsonOptions);
        
        Assert.IsNotNull(request, $"Failed to deserialize {fileName}");
        return request;
    }

    #endregion
}
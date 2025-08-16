using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE1RequestProtobufConversionTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task PoE1_FullYes_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE1/Full_Yes.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Basic structure should match
        Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option);
        Assert.AreEqual(originalRequest.Query.Stats.Count, convertedRequest.Query.Stats.Count);
        Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price);

        // Type filters should match
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.Category?.Option,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.Category?.Option);
        Assert.AreEqual(originalRequest.Query.Filters.TypeFilters?.Filters.Rarity?.Option,
                       convertedRequest.Query.Filters.TypeFilters?.Filters.Rarity?.Option);

        // Trade filters should match
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Option,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Option);
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Min,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Min);
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Max,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Max);

        // Account filter should match
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Account?.Input,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Account?.Input);
    }

    [TestMethod]
    public async Task PoE1_MinNo_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE1/Min_No.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Should handle min-only price filter
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Min,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Min);
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Max,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Max);
        Assert.AreEqual(originalRequest.Query.Filters.TradeFilters?.Filters.Price?.Option,
                       convertedRequest.Query.Filters.TradeFilters?.Filters.Price?.Option);
    }

    [TestMethod]
    public async Task PoE1_MaxAny_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalRequest = await DeserializeRequestFileAsync("Requests/PoE1/Max_Any.json");

        // Act - Convert to protobuf and back
        var protoRequest = originalRequest.ToProtobuf();
        var convertedRequest = protoRequest.ToRecord();

        // Assert - Basic structure should be preserved
        Assert.AreEqual(originalRequest.Query.Status.Option, convertedRequest.Query.Status.Option);
        Assert.AreEqual(originalRequest.Sort.Price, convertedRequest.Sort.Price);
    }

    [TestMethod]
    public async Task AllPoE1Requests_ShouldConvertToProtobufAndBack()
    {
        var requestFiles = new[]
        {
            "Requests/PoE1/Full_Yes.json",
            "Requests/PoE1/Min_No.json", 
            "Requests/PoE1/Max_Any.json"
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

            // PoE1-specific filters should not have PoE2 data
            Assert.IsNull(convertedRequest.Query.Filters.EquipmentFilters, 
                         $"PoE1 converted request {fileName} should not have equipment filters");
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
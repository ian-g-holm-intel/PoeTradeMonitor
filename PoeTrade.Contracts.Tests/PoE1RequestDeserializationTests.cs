namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE1RequestDeserializationTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task PoE1_FullYes_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE1/Full_Yes.json");
        ValidateBasicTradeSearchRequest(request, "Full_Yes.json");
        
        // Verify PoE1-specific properties
        Assert.IsNotNull(request.Query.Filters.WeaponFilters, "PoE1 should have weapon filters");
        Assert.IsNotNull(request.Query.Filters.ArmourFilters, "PoE1 should have armour filters");
        Assert.IsNotNull(request.Query.Filters.SocketFilters, "PoE1 should have socket filters");
        Assert.IsNotNull(request.Query.Filters.HeistFilters, "PoE1 should have heist filters");
        Assert.IsNotNull(request.Query.Filters.SanctumFilters, "PoE1 should have sanctum filters");
        Assert.IsNotNull(request.Query.Filters.UltimatumFilters, "PoE1 should have ultimatum filters");
        
        // PoE1 should not have equipment filters (PoE2-specific)
        Assert.IsNull(request.Query.Filters.EquipmentFilters, "PoE1 should not have equipment filters");
        
        // Verify type filters
        Assert.IsNotNull(request.Query.Filters.TypeFilters?.Filters.Category);
        Assert.AreEqual("weapon", request.Query.Filters.TypeFilters!.Filters.Category!.Option);
        Assert.AreEqual("unique", request.Query.Filters.TypeFilters?.Filters.Rarity?.Option);
        
        // Verify trade filters
        Assert.IsNotNull(request.Query.Filters.TradeFilters?.Filters.Price);
        Assert.AreEqual("divine", request.Query.Filters.TradeFilters!.Filters.Price!.Option);
        Assert.AreEqual(1, request.Query.Filters.TradeFilters!.Filters.Price!.Min);
        Assert.AreEqual(1, request.Query.Filters.TradeFilters!.Filters.Price!.Max);
    }

    [TestMethod]
    public async Task PoE1_MinNo_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE1/Min_No.json");
        ValidateBasicTradeSearchRequest(request, "Min_No.json");
        
        // Verify minimal properties
        Assert.AreEqual("weapon.one", request.Query.Filters.TypeFilters?.Filters.Category?.Option);
        Assert.AreEqual("nonunique", request.Query.Filters.TypeFilters?.Filters.Rarity?.Option);
        
        // Verify price filter without option (min only)
        Assert.IsNotNull(request.Query.Filters.TradeFilters?.Filters.Price);
        Assert.IsNull(request.Query.Filters.TradeFilters!.Filters.Price!.Option);
        Assert.AreEqual(1, request.Query.Filters.TradeFilters!.Filters.Price!.Min);
        Assert.IsNull(request.Query.Filters.TradeFilters!.Filters.Price!.Max);
    }

    [TestMethod]
    public async Task PoE1_MaxAny_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE1/Max_Any.json");
        ValidateBasicTradeSearchRequest(request, "Max_Any.json");
    }

    [TestMethod]
    public async Task AllPoE1RequestExamples_ShouldDeserializeSuccessfully()
    {
        var poe1Files = new[]
        {
            "Requests/PoE1/Full_Yes.json",
            "Requests/PoE1/Min_No.json",
            "Requests/PoE1/Max_Any.json"
        };

        foreach (var fileName in poe1Files)
        {
            var request = await DeserializeRequestFileAsync(fileName);
            ValidateBasicTradeSearchRequest(request, fileName);
            
            // Verify all PoE1 requests don't have PoE2-specific filters
            Assert.IsNull(request.Query.Filters.EquipmentFilters, $"PoE1 request {fileName} should not have equipment filters");
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

    private static void ValidateBasicTradeSearchRequest(TradeSearchRequest request, string fileName)
    {
        Assert.IsNotNull(request, $"Request should not be null: {fileName}");
        Assert.IsNotNull(request.Query, $"Query should not be null: {fileName}");
        Assert.IsNotNull(request.Sort, $"Sort should not be null: {fileName}");
        
        // Validate status
        Assert.IsNotNull(request.Query.Status, $"Status should not be null: {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(request.Query.Status.Option), $"Status option should not be empty: {fileName}");
        
        // Validate stats
        Assert.IsNotNull(request.Query.Stats, $"Stats should not be null: {fileName}");
        Assert.IsTrue(request.Query.Stats.Count > 0, $"Stats should have at least one entry: {fileName}");
        
        // Validate filters
        Assert.IsNotNull(request.Query.Filters, $"Filters should not be null: {fileName}");
        
        // Sort should be "asc" for all test examples
        Assert.AreEqual("asc", request.Sort.Price, $"Sort price should be 'asc': {fileName}");
    }

    #endregion
}
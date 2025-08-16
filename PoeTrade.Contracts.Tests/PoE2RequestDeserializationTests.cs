namespace PoeTrade.Contracts.Tests;

[TestClass]
public class PoE2RequestDeserializationTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task PoE2_FullYes_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE2/Full_Yes.json");
        ValidateBasicTradeSearchRequest(request, "Full_Yes.json");
        
        // Verify PoE2-specific properties
        Assert.IsNotNull(request.Query.Filters.EquipmentFilters, "PoE2 should have equipment filters");
        
        // PoE2 should not have PoE1-specific filters
        Assert.IsNull(request.Query.Filters.WeaponFilters, "PoE2 should not have weapon filters");
        Assert.IsNull(request.Query.Filters.ArmourFilters, "PoE2 should not have armour filters");
        Assert.IsNull(request.Query.Filters.SocketFilters, "PoE2 should not have socket filters");
        Assert.IsNull(request.Query.Filters.HeistFilters, "PoE2 should not have heist filters");
        Assert.IsNull(request.Query.Filters.SanctumFilters, "PoE2 should not have sanctum filters");
        Assert.IsNull(request.Query.Filters.UltimatumFilters, "PoE2 should not have ultimatum filters");
        
        // Verify PoE2-specific type filter properties
        Assert.IsNotNull(request.Query.Filters.TypeFilters?.Filters.ItemLevel, "PoE2 should have ilvl in type filters");
        Assert.IsNotNull(request.Query.Filters.TypeFilters?.Filters.Quality, "PoE2 should have quality in type filters");
        Assert.AreEqual(1, request.Query.Filters.TypeFilters.Filters.ItemLevel?.Min);
        Assert.AreEqual(1, request.Query.Filters.TypeFilters.Filters.ItemLevel?.Max);
        
        // Verify equipment filters
        Assert.IsNotNull(request.Query.Filters.EquipmentFilters?.Filters.RuneSockets, "PoE2 should have rune sockets");
        Assert.IsNotNull(request.Query.Filters.EquipmentFilters?.Filters.Spirit, "PoE2 should have spirit");
        Assert.IsNotNull(request.Query.Filters.EquipmentFilters?.Filters.ReloadTime, "PoE2 should have reload time");
    }

    [TestMethod]
    public async Task PoE2_MinNo_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE2/Min_No.json");
        ValidateBasicTradeSearchRequest(request, "Min_No.json");
        
        // Verify PoE2-specific category
        Assert.AreEqual("armour.buckler", request.Query.Filters.TypeFilters?.Filters.Category?.Option);
        Assert.AreEqual("magic", request.Query.Filters.TypeFilters?.Filters.Rarity?.Option);
        
        // Verify PoE2-specific price currency
        Assert.AreEqual("exalted", request.Query.Filters.TradeFilters?.Filters.Price?.Option);
    }

    [TestMethod]
    public async Task PoE2_MaxAny_ShouldDeserializeSuccessfully()
    {
        var request = await DeserializeRequestFileAsync("Requests/PoE2/Max_Any.json");
        ValidateBasicTradeSearchRequest(request, "Max_Any.json");
    }

    [TestMethod]
    public async Task AllPoE2RequestExamples_ShouldDeserializeSuccessfully()
    {
        var poe2Files = new[]
        {
            "Requests/PoE2/Full_Yes.json",
            "Requests/PoE2/Min_No.json",
            "Requests/PoE2/Max_Any.json"
        };

        foreach (var fileName in poe2Files)
        {
            var request = await DeserializeRequestFileAsync(fileName);
            ValidateBasicTradeSearchRequest(request, fileName);
            
            // Verify all PoE2 requests don't have PoE1-specific filters
            Assert.IsNull(request.Query.Filters.WeaponFilters, $"PoE2 request {fileName} should not have weapon filters");
            Assert.IsNull(request.Query.Filters.ArmourFilters, $"PoE2 request {fileName} should not have armour filters");
            Assert.IsNull(request.Query.Filters.SocketFilters, $"PoE2 request {fileName} should not have socket filters");
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
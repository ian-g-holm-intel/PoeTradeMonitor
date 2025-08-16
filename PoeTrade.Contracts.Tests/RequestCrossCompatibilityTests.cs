namespace PoeTrade.Contracts.Tests;

[TestClass]
public class RequestCrossCompatibilityTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task AllRequestExamples_ShouldDeserializeSuccessfully()
    {
        var poe1Files = new[]
        {
            "Requests/PoE1/Full_Yes.json",
            "Requests/PoE1/Min_No.json",
            "Requests/PoE1/Max_Any.json"
        };

        var poe2Files = new[]
        {
            "Requests/PoE2/Full_Yes.json",
            "Requests/PoE2/Min_No.json",
            "Requests/PoE2/Max_Any.json"
        };

        foreach (var fileName in poe1Files.Concat(poe2Files))
        {
            var request = await DeserializeRequestFileAsync(fileName);
            ValidateBasicTradeSearchRequest(request, fileName);
        }
    }

    [TestMethod]
    public async Task RequestStructure_ShouldBeConsistent()
    {
        var poe1Request = await DeserializeRequestFileAsync("Requests/PoE1/Full_Yes.json");
        var poe2Request = await DeserializeRequestFileAsync("Requests/PoE2/Full_Yes.json");

        // Both should have basic structure
        Assert.IsNotNull(poe1Request.Query);
        Assert.IsNotNull(poe2Request.Query);
        Assert.IsNotNull(poe1Request.Sort);
        Assert.IsNotNull(poe2Request.Sort);

        // Both should have common filters
        Assert.IsNotNull(poe1Request.Query.Filters.TypeFilters);
        Assert.IsNotNull(poe2Request.Query.Filters.TypeFilters);
        Assert.IsNotNull(poe1Request.Query.Filters.TradeFilters);
        Assert.IsNotNull(poe2Request.Query.Filters.TradeFilters);

        // Status and stats should be similar
        Assert.AreEqual("online", poe1Request.Query.Status.Option);
        Assert.AreEqual("online", poe2Request.Query.Status.Option);
        Assert.IsTrue(poe1Request.Query.Stats.Count > 0);
        Assert.IsTrue(poe2Request.Query.Stats.Count > 0);
    }

    [TestMethod]
    public async Task VersionSpecificFilters_ShouldBeExclusive()
    {
        var poe1Request = await DeserializeRequestFileAsync("Requests/PoE1/Full_Yes.json");
        var poe2Request = await DeserializeRequestFileAsync("Requests/PoE2/Full_Yes.json");

        // PoE1 specific filters should only exist in PoE1
        Assert.IsNotNull(poe1Request.Query.Filters.WeaponFilters, "PoE1 should have weapon filters");
        Assert.IsNotNull(poe1Request.Query.Filters.ArmourFilters, "PoE1 should have armour filters");
        Assert.IsNotNull(poe1Request.Query.Filters.SocketFilters, "PoE1 should have socket filters");
        Assert.IsNull(poe2Request.Query.Filters.WeaponFilters, "PoE2 should not have weapon filters");
        Assert.IsNull(poe2Request.Query.Filters.ArmourFilters, "PoE2 should not have armour filters");
        Assert.IsNull(poe2Request.Query.Filters.SocketFilters, "PoE2 should not have socket filters");

        // PoE2 specific filters should only exist in PoE2
        Assert.IsNotNull(poe2Request.Query.Filters.EquipmentFilters, "PoE2 should have equipment filters");
        Assert.IsNull(poe1Request.Query.Filters.EquipmentFilters, "PoE1 should not have equipment filters");

        // PoE1 league-specific filters should only exist in PoE1
        Assert.IsNotNull(poe1Request.Query.Filters.HeistFilters, "PoE1 should have heist filters");
        Assert.IsNotNull(poe1Request.Query.Filters.SanctumFilters, "PoE1 should have sanctum filters");
        Assert.IsNotNull(poe1Request.Query.Filters.UltimatumFilters, "PoE1 should have ultimatum filters");
        Assert.IsNull(poe2Request.Query.Filters.HeistFilters, "PoE2 should not have heist filters");
        Assert.IsNull(poe2Request.Query.Filters.SanctumFilters, "PoE2 should not have sanctum filters");
        Assert.IsNull(poe2Request.Query.Filters.UltimatumFilters, "PoE2 should not have ultimatum filters");
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
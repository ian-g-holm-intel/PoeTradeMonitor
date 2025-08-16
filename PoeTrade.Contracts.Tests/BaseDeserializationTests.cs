namespace PoeTrade.Contracts.Tests;

[TestClass]
public abstract class BaseDeserializationTests
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    protected static async Task<TradeFetchResponse> DeserializeJsonFileAsync(string fileName)
    {
        var filePath = Path.Combine("TestData", fileName);
        Assert.IsTrue(File.Exists(filePath), $"Test file not found: {filePath}");
        
        var jsonContent = await File.ReadAllTextAsync(filePath);
        Assert.IsFalse(string.IsNullOrWhiteSpace(jsonContent), $"JSON file is empty: {fileName}");
        
        var result = JsonSerializer.Deserialize<TradeFetchResponse>(jsonContent, JsonOptions);
        Assert.IsNotNull(result, $"Failed to deserialize JSON from {fileName}");
        
        return result;
    }

    protected static void ValidateBasicTradeSearchResponse(TradeFetchResponse response, string fileName)
    {
        Assert.IsNotNull(response, $"Response should not be null for {fileName}");
        Assert.IsNotNull(response.Result, $"Result should not be null for {fileName}");
        Assert.IsTrue(response.Result.Count > 0, $"Result should contain items for {fileName}");
    }

    protected static void ValidateTradeSearchResult(TradeSearchResult result, string fileName)
    {
        Assert.IsNotNull(result, $"Trade search result should not be null for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(result.Id), $"Id should not be empty for {fileName}");
        Assert.IsNotNull(result.Listing, $"Listing should not be null for {fileName}");
        Assert.IsNotNull(result.Item, $"Item should not be null for {fileName}");
    }

    protected static void ValidateTradeListing(TradeListing listing, string fileName)
    {
        Assert.IsNotNull(listing, $"Listing should not be null for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(listing.Method), $"Method should not be empty for {fileName}");
        Assert.IsNotNull(listing.Account, $"Account should not be null for {fileName}");
        Assert.IsNotNull(listing.Stash, $"Stash should not be null for {fileName}");
    }

    protected static void ValidateTradeItem(TradeItem item, string fileName)
    {
        Assert.IsNotNull(item, $"Item should not be null for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(item.Id), $"Item Id should not be empty for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(item.TypeLine), $"TypeLine should not be empty for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(item.BaseType), $"BaseType should not be empty for {fileName}");
        Assert.IsFalse(string.IsNullOrEmpty(item.League), $"League should not be empty for {fileName}");
        Assert.IsTrue(item.Width > 0, $"Width should be positive for {fileName}");
        Assert.IsTrue(item.Height > 0, $"Height should be positive for {fileName}");
    }
}
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the response from a Path of Exile trade search API request.
/// Contains search results, pagination information, and a unique identifier for the search query.
/// </summary>
public record TradeSearchResponse
{
    /// <summary>
    /// Gets or sets the list of item listing IDs returned from the search.
    /// These IDs can be used to fetch detailed item information in subsequent API calls.
    /// The list may be paginated and represent only a subset of the total results.
    /// </summary>
    [JsonPropertyName("result")]
    public List<string> Result { get; set; } = [];

    /// <summary>
    /// Gets or sets the unique identifier for this search query.
    /// This ID can be used to fetch additional pages of results or reference the search later.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of items that match the search criteria.
    /// This represents the complete result count, which may be larger than the number of items
    /// returned in the current Result list due to pagination limits.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}
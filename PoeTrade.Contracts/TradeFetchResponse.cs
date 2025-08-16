using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the response from the Path of Exile trade fetch API.
/// Contains a list of matching trade listings with items and listing details.
/// </summary>
public record TradeFetchResponse
{
    /// <summary>
    /// List of trade search results matching the query criteria.
    /// Each result contains an item and its associated listing information.
    /// </summary>
    [JsonPropertyName("result")]
    public List<TradeSearchResult> Result { get; init; } = [];
}
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a single trade search result containing an item and its listing details.
/// Each result represents one item available for trade on the Path of Exile trade market.
/// </summary>
public record TradeSearchResult
{
    /// <summary>
    /// Gets the unique identifier for this trade result.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the listing information including price, seller account, and stash details.
    /// </summary>
    [JsonPropertyName("listing")]
    public TradeListing Listing { get; init; } = new();
    
    /// <summary>
    /// Gets the item details including properties, modifiers, and characteristics.
    /// </summary>
    [JsonPropertyName("item")]
    public TradeItem Item { get; init; } = new();
}
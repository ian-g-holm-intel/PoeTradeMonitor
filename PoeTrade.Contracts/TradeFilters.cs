using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents trade-specific filters for price, account, and listing settings.
/// Used to filter results based on trading preferences and seller criteria.
/// </summary>
public record TradeFilters
{
    /// <summary>
    /// Gets whether this filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific trade filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public TradeFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific options for trade-based filtering.
/// </summary>
public record TradeFilterOptions
{
    /// <summary>
    /// Gets the account name filter to search for items from specific sellers.
    /// </summary>
    [JsonPropertyName("account")]
    public InputFilter? Account { get; init; }
    
    /// <summary>
    /// Gets the collapse filter option for grouping similar listings.
    /// </summary>
    [JsonPropertyName("collapse")]
    public OptionFilter? Collapse { get; init; }
    
    /// <summary>
    /// Gets the price filter for specifying currency type and price range.
    /// </summary>
    [JsonPropertyName("price")]
    public PriceFilter? Price { get; init; }
}
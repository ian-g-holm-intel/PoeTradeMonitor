using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents type-based filters for item category, rarity, and level.
/// Common to both PoE1 and PoE2 with game-specific properties.
/// </summary>
public record TypeFilters
{
    /// <summary>
    /// Gets whether this filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific type filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public TypeFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific options for type-based filtering.
/// </summary>
public record TypeFilterOptions
{
    /// <summary>
    /// Gets the item category filter (e.g., "weapon", "armour", "flask").
    /// </summary>
    [JsonPropertyName("category")]
    public OptionFilter? Category { get; init; }
    
    /// <summary>
    /// Gets the item rarity filter (e.g., "normal", "magic", "rare", "unique").
    /// </summary>
    [JsonPropertyName("rarity")]
    public OptionFilter? Rarity { get; init; }
    
    /// <summary>
    /// Gets the item level filter (PoE2-specific). Filters items by their level requirement.
    /// </summary>
    [JsonPropertyName("ilvl")]
    public RangeFilter? ItemLevel { get; init; }
    
    /// <summary>
    /// Gets the quality filter (PoE2-specific). Filters items by their quality percentage.
    /// </summary>
    [JsonPropertyName("quality")]
    public RangeFilter? Quality { get; init; }
}

/// <summary>
/// Represents a filter that accepts a single option value.
/// Used for categorical filters like rarity, category, etc.
/// </summary>
public record OptionFilter
{
    /// <summary>
    /// Gets the selected option value.
    /// </summary>
    [JsonPropertyName("option")]
    public string? Option { get; init; }
}

/// <summary>
/// Represents a filter that accepts a numeric range with minimum and maximum values.
/// Used for quantitative filters like level, damage, price, etc.
/// </summary>
public record RangeFilter
{
    /// <summary>
    /// Gets the minimum value for the range (inclusive).
    /// </summary>
    [JsonPropertyName("min")]
    public int? Min { get; init; }
    
    /// <summary>
    /// Gets the maximum value for the range (inclusive).
    /// </summary>
    [JsonPropertyName("max")]
    public int? Max { get; init; }
}

/// <summary>
/// Represents a filter that accepts text input.
/// Used for filters like account names or custom search terms.
/// </summary>
public record InputFilter
{
    /// <summary>
    /// Gets the input text value.
    /// </summary>
    [JsonPropertyName("input")]
    public string? Input { get; init; }
}

/// <summary>
/// Represents a price filter with currency type and value range.
/// Used for filtering items by their listed price.
/// </summary>
public record PriceFilter
{
    /// <summary>
    /// Gets the currency type (e.g., "divine", "chaos", "exalted").
    /// </summary>
    [JsonPropertyName("option")]
    public string? Option { get; init; }
    
    /// <summary>
    /// Gets the minimum price value.
    /// </summary>
    [JsonPropertyName("min")]
    public int? Min { get; init; }
    
    /// <summary>
    /// Gets the maximum price value.
    /// </summary>
    [JsonPropertyName("max")]
    public int? Max { get; init; }
}
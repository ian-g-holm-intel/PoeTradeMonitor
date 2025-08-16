using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents map-specific filters for Path of Exile trade searches.
/// Contains filters for map tier, area level, item quantity, and other map properties.
/// </summary>
public record MapFilters
{
    /// <summary>
    /// Gets whether this map filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific map filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public MapFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for map properties in trade searches.
/// </summary>
public record MapFilterOptions
{
    /// <summary>
    /// Gets the map tier range filter.
    /// </summary>
    [JsonPropertyName("map_tier")]
    public RangeFilter? MapTier { get; init; }
    
    /// <summary>
    /// Gets the area level range filter for maps.
    /// </summary>
    [JsonPropertyName("area_level")]
    public RangeFilter? AreaLevel { get; init; }
    
    // PoE1-specific
    /// <summary>
    /// Gets the item quantity percentage range filter for maps (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_iiq")]
    public RangeFilter? ItemQuantity { get; init; }
    
    /// <summary>
    /// Gets the item rarity percentage range filter for maps (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_iir")]
    public RangeFilter? ItemRarity { get; init; }
    
    /// <summary>
    /// Gets the pack size percentage range filter for maps (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_packsize")]
    public RangeFilter? PackSize { get; init; }
    
    /// <summary>
    /// Gets the blighted map filter for maps affected by Blight mechanics (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_blighted")]
    public OptionFilter? MapBlighted { get; init; }
    
    /// <summary>
    /// Gets the completion reward filter for Atlas completion bonuses (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_completion_reward")]
    public OptionFilter? CompletionReward { get; init; }
    
    /// <summary>
    /// Gets the uber blighted map filter for enhanced Blight encounters (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_uberblighted")]
    public OptionFilter? MapUberBlighted { get; init; }
    
    /// <summary>
    /// Gets the map series filter for different Atlas map series (PoE1 only).
    /// </summary>
    [JsonPropertyName("map_series")]
    public OptionFilter? MapSeries { get; init; }
    
    // PoE2-specific
    /// <summary>
    /// Gets the map bonus range filter for additional map rewards (PoE2 only).
    /// </summary>
    [JsonPropertyName("map_bonus")]
    public RangeFilter? MapBonus { get; init; }
    
    /// <summary>
    /// Gets the ultimatum hint filter for maps with Ultimatum encounters (PoE2 only).
    /// </summary>
    [JsonPropertyName("ultimatum_hint")]
    public OptionFilter? UltimatumHint { get; init; }
}
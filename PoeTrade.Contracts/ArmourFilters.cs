using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents armor-specific filters for Path of Exile trade searches.
/// Contains filters for defensive properties like armor, energy shield, evasion, and ward.
/// </summary>
public record ArmourFilters
{
    /// <summary>
    /// Gets whether this armor filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific armor filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public ArmourFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for armor properties in trade searches.
/// </summary>
public record ArmourFilterOptions
{
    /// <summary>
    /// Gets the armor rating range filter.
    /// </summary>
    [JsonPropertyName("ar")]
    public RangeFilter? Armour { get; init; }
    
    /// <summary>
    /// Gets the energy shield range filter.
    /// </summary>
    [JsonPropertyName("es")]
    public RangeFilter? EnergyShield { get; init; }
    
    /// <summary>
    /// Gets the evasion rating range filter.
    /// </summary>
    [JsonPropertyName("ev")]
    public RangeFilter? Evasion { get; init; }
    
    /// <summary>
    /// Gets the block chance range filter for shields.
    /// </summary>
    [JsonPropertyName("block")]
    public RangeFilter? Block { get; init; }
    
    /// <summary>
    /// Gets the base defense percentile range filter.
    /// </summary>
    [JsonPropertyName("base_defence_percentile")]
    public RangeFilter? BaseDefencePercentile { get; init; }
    
    /// <summary>
    /// Gets the ward range filter.
    /// </summary>
    [JsonPropertyName("ward")]
    public RangeFilter? Ward { get; init; }
}
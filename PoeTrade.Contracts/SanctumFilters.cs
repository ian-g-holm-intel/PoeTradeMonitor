using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents Sanctum league-specific filters for Path of Exile trade searches.
/// Contains filters for Sanctum room properties and resources.
/// </summary>
public record SanctumFilters
{
    /// <summary>
    /// Gets whether this Sanctum filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific Sanctum filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public SanctumFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for Sanctum league content in trade searches.
/// Includes filters for resolve, inspiration, and gold resources.
/// </summary>
public record SanctumFilterOptions
{
    /// <summary>
    /// Gets the resolve range filter for Sanctum rooms.
    /// </summary>
    [JsonPropertyName("sanctum_resolve")]
    public RangeFilter? Resolve { get; init; }
    
    /// <summary>
    /// Gets the inspiration range filter for Sanctum rooms.
    /// </summary>
    [JsonPropertyName("sanctum_inspiration")]
    public RangeFilter? Inspiration { get; init; }
    
    /// <summary>
    /// Gets the maximum resolve range filter for Sanctum rooms.
    /// </summary>
    [JsonPropertyName("sanctum_max_resolve")]
    public RangeFilter? MaxResolve { get; init; }
    
    /// <summary>
    /// Gets the gold range filter for Sanctum rooms.
    /// </summary>
    [JsonPropertyName("sanctum_gold")]
    public RangeFilter? Gold { get; init; }
}
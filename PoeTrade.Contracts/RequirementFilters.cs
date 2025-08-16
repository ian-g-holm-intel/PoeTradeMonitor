using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents requirement filters for Path of Exile trade searches.
/// Contains filters for level and attribute requirements.
/// </summary>
public record RequirementFilters
{
    /// <summary>
    /// Gets whether this requirement filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific requirement filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public RequirementFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for item requirements in trade searches.
/// </summary>
public record RequirementFilterOptions
{
    /// <summary>
    /// Gets the level requirement range filter.
    /// </summary>
    [JsonPropertyName("lvl")]
    public RangeFilter? Level { get; init; }
    
    /// <summary>
    /// Gets the strength requirement range filter.
    /// </summary>
    [JsonPropertyName("str")]
    public RangeFilter? Strength { get; init; }
    
    /// <summary>
    /// Gets the intelligence requirement range filter.
    /// </summary>
    [JsonPropertyName("int")]
    public RangeFilter? Intelligence { get; init; }
    
    /// <summary>
    /// Gets the dexterity requirement range filter.
    /// </summary>
    [JsonPropertyName("dex")]
    public RangeFilter? Dexterity { get; init; }
    
    // PoE1-specific
    /// <summary>
    /// Gets the character class requirement filter (PoE1 only).
    /// </summary>
    [JsonPropertyName("class")]
    public OptionFilter? Class { get; init; }
}
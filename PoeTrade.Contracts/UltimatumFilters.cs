using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents Ultimatum league-specific filters for Path of Exile trade searches.
/// Contains filters for Ultimatum encounter challenges and rewards.
/// </summary>
public record UltimatumFilters
{
    /// <summary>
    /// Gets whether this Ultimatum filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific Ultimatum filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public UltimatumFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for Ultimatum league content in trade searches.
/// Includes filters for challenges, rewards, inputs, and outputs.
/// </summary>
public record UltimatumFilterOptions
{
    /// <summary>
    /// Gets the challenge type filter for Ultimatum encounters.
    /// </summary>
    [JsonPropertyName("ultimatum_challenge")]
    public OptionFilter? Challenge { get; init; }
    
    /// <summary>
    /// Gets the reward type filter for Ultimatum encounters.
    /// </summary>
    [JsonPropertyName("ultimatum_reward")]
    public OptionFilter? Reward { get; init; }
    
    /// <summary>
    /// Gets the input type filter for Ultimatum encounters.
    /// </summary>
    [JsonPropertyName("ultimatum_input")]
    public OptionFilter? Input { get; init; }
    
    /// <summary>
    /// Gets the output type filter for Ultimatum encounters.
    /// </summary>
    [JsonPropertyName("ultimatum_output")]
    public OptionFilter? Output { get; init; }
}
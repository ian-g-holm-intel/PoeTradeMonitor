using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents detailed information about a modifier, including its name, tier, level, and magnitudes.
/// Used for understanding the characteristics and power level of item modifiers.
/// </summary>
public record ModInfo
{
    /// <summary>
    /// Gets the name of the modifier.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the tier classification of the modifier (e.g., "T1", "T2", etc.).
    /// </summary>
    [JsonPropertyName("tier")]
    public string Tier { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the level requirement or power level of the modifier.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; init; }
    
    /// <summary>
    /// Gets the magnitude ranges and values for this modifier.
    /// </summary>
    [JsonPropertyName("magnitudes")]
    public List<ModMagnitude>? Magnitudes { get; init; }
}
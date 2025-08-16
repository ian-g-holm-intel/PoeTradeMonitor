using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the magnitude range for a modifier, containing hash identifier and min/max values.
/// Used to define the possible value ranges for item modifiers.
/// </summary>
public record ModMagnitude
{
    /// <summary>
    /// Gets the hash identifier for this magnitude range.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the minimum value for this magnitude range.
    /// Uses a custom converter to handle both string and numeric JSON values.
    /// </summary>
    [JsonPropertyName("min")]
    [JsonConverter(typeof(StringOrNumberConverter))]
    public string Min { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the maximum value for this magnitude range.
    /// Uses a custom converter to handle both string and numeric JSON values.
    /// </summary>
    [JsonPropertyName("max")]
    [JsonConverter(typeof(StringOrNumberConverter))]
    public string Max { get; init; } = string.Empty;
}
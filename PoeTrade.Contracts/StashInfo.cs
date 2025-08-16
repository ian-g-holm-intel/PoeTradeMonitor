using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents information about a stash tab location and identification.
/// Contains the name and coordinates for stash tab positioning.
/// </summary>
public record StashInfo
{
    /// <summary>
    /// Gets the name of the stash tab.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the X coordinate position of the item in the stash tab.
    /// </summary>
    [JsonPropertyName("x")]
    public int X { get; init; }
    
    /// <summary>
    /// Gets the Y coordinate position of the item in the stash tab.
    /// </summary>
    [JsonPropertyName("y")]
    public int Y { get; init; }
}
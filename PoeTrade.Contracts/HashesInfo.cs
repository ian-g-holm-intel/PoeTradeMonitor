using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents hash information for different types of item modifiers.
/// Contains hash arrays for explicit, implicit, enchant, crafted, fractured, and rune modifiers.
/// </summary>
public record HashesInfo
{
    /// <summary>
    /// Gets the hash information for explicit modifiers.
    /// </summary>
    [JsonPropertyName("explicit")]
    public List<List<object>>? Explicit { get; init; }
    
    /// <summary>
    /// Gets the hash information for implicit modifiers.
    /// </summary>
    [JsonPropertyName("implicit")]
    public List<List<object>>? Implicit { get; init; }
    
    /// <summary>
    /// Gets the hash information for enchantment modifiers.
    /// </summary>
    [JsonPropertyName("enchant")]
    public List<List<object>>? Enchant { get; init; }
    
    /// <summary>
    /// Gets the hash information for crafted modifiers.
    /// </summary>
    [JsonPropertyName("crafted")]
    public List<List<object>>? Crafted { get; init; }
    
    /// <summary>
    /// Gets the hash information for fractured modifiers.
    /// </summary>
    [JsonPropertyName("fractured")]
    public List<List<object>>? Fractured { get; init; }
    
    /// <summary>
    /// Gets the hash information for rune modifiers (PoE2 specific).
    /// </summary>
    [JsonPropertyName("rune")]
    public List<List<object>>? Rune { get; init; }
}
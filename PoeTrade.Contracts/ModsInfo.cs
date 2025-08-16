using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents collections of modifier information organized by modifier type.
/// Contains separate lists for explicit, implicit, enchant, crafted, and fractured modifiers.
/// </summary>
public record ModsInfo
{
    /// <summary>
    /// Gets the list of explicit modifier information.
    /// </summary>
    [JsonPropertyName("explicit")]
    public List<ModInfo>? Explicit { get; init; }
    
    /// <summary>
    /// Gets the list of implicit modifier information.
    /// </summary>
    [JsonPropertyName("implicit")]
    public List<ModInfo>? Implicit { get; init; }
    
    /// <summary>
    /// Gets the list of enchantment modifier information.
    /// </summary>
    [JsonPropertyName("enchant")]
    public List<ModInfo>? Enchant { get; init; }
    
    /// <summary>
    /// Gets the list of crafted modifier information.
    /// </summary>
    [JsonPropertyName("crafted")]
    public List<ModInfo>? Crafted { get; init; }
    
    /// <summary>
    /// Gets the list of fractured modifier information.
    /// </summary>
    [JsonPropertyName("fractured")]
    public List<ModInfo>? Fractured { get; init; }
}
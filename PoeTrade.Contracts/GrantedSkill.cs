using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a skill granted by an item in Path of Exile.
/// Contains information about the skill name, values, display mode, and icon.
/// </summary>
public record GrantedSkill
{
    /// <summary>
    /// Gets the name of the granted skill.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the values associated with this skill as nested lists.
    /// The outer list represents multiple value groups, inner lists contain value and type information.
    /// </summary>
    [JsonPropertyName("values")]
    public List<List<object>>? Values { get; init; }
    
    /// <summary>
    /// Gets the display mode for how this skill should be rendered in the UI.
    /// </summary>
    [JsonPropertyName("displayMode")]
    public int DisplayMode { get; init; }
    
    /// <summary>
    /// Gets the URL to the icon image for this skill.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }
}
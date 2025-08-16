using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a property of a Path of Exile item such as damage, armor values, or requirements.
/// Contains the property name, values, and display formatting information.
/// </summary>
public record ItemProperty
{
    /// <summary>
    /// Gets the name of the property (e.g., "Physical Damage", "Armour", "Level").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the values associated with this property as nested lists.
    /// The outer list represents multiple value groups, inner lists contain value and type information.
    /// </summary>
    [JsonPropertyName("values")]
    [JsonConverter(typeof(ItemPropertyValuesConverter))]
    public List<ItemPropertyValue>? Values { get; init; }
    
    /// <summary>
    /// Gets the display mode for how this property should be rendered in the UI.
    /// </summary>
    [JsonPropertyName("displayMode")]
    public int DisplayMode { get; init; }
    
    /// <summary>
    /// Gets the type identifier for this property, used for categorization and formatting.
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; init; }
}
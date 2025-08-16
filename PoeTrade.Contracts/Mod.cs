using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a modifier on a Path of Exile item.
/// Contains the original text, processed text with placeholders, and extracted numeric values.
/// </summary>
public record Mod
{
    /// <summary>
    /// Gets the original, unprocessed modifier text as it appears in the game.
    /// </summary>
    [JsonPropertyName("rawModText")]
    public string RawModText { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the processed modifier text with numeric values replaced by placeholders (e.g., "#").
    /// This is useful for pattern matching and categorization.
    /// </summary>
    [JsonPropertyName("modText")]
    public string ModText { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the numeric values extracted from the modifier text.
    /// These correspond to the placeholders in the ModText property.
    /// </summary>
    [JsonPropertyName("values")]
    public List<double> Values { get; init; } = new();
    
    /// <summary>
    /// Gets a string representation of values for serialization purposes.
    /// This property is primarily used for compatibility with existing systems.
    /// </summary>
    [JsonPropertyName("valueString")]
    public string? ValueString { get; init; }
    
    /// <summary>
    /// Returns the processed modifier text for display purposes.
    /// </summary>
    /// <returns>The ModText property value.</returns>
    public override string ToString() => ModText;
}
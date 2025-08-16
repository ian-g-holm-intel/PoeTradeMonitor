using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a single currency item's positioning and appearance in the currency layout.
/// Contains section, coordinates, dimensions, and scaling information.
/// </summary>
public record CurrencyLayoutItem
{
    /// <summary>
    /// Gets the section this currency item belongs to (e.g., "general", "influence").
    /// </summary>
    [JsonPropertyName("section")]
    public string Section { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the X coordinate position of the currency item.
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; init; }
    
    /// <summary>
    /// Gets the Y coordinate position of the currency item.
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; init; }
    
    /// <summary>
    /// Gets the width of the currency item.
    /// </summary>
    [JsonPropertyName("w")]
    public int Width { get; init; }
    
    /// <summary>
    /// Gets the height of the currency item.
    /// </summary>
    [JsonPropertyName("h")]
    public int Height { get; init; }
    
    /// <summary>
    /// Gets the scale factor for displaying the currency item.
    /// </summary>
    [JsonPropertyName("scale")]
    public double Scale { get; init; }
}
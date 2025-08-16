namespace PoeTrade.Contracts;

/// <summary>
/// Represents a single value within an item property, containing both the display value and its position index.
/// Used in properties that have multiple values, such as damage ranges or requirement values.
/// </summary>
public record ItemPropertyValue
{
    /// <summary>
    /// Gets or sets the display value of the property (e.g., "12", "50-75", "Intelligence").
    /// Can be null if the property value is not available.
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Gets or sets the index position of this value within the property's value collection.
    /// Used to maintain the correct order of multiple values within a single property.
    /// </summary>
    public int Index { get; set; }
}
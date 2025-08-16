using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the currency layout configuration in a stash tab response.
/// Contains sections and layout positioning information for currency items.
/// </summary>
public record CurrencyLayout
{
    /// <summary>
    /// Gets the list of sections in the currency layout.
    /// </summary>
    [JsonPropertyName("sections")]
    public List<string> Sections { get; init; } = new();
    
    /// <summary>
    /// Gets the layout positioning information for currency items.
    /// The key is the item index as a string, and the value contains positioning data.
    /// </summary>
    [JsonPropertyName("layout")]
    public Dictionary<string, CurrencyLayoutItem> Layout { get; init; } = new();
}
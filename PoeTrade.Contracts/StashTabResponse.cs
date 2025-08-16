using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a response for stash tab data from the Path of Exile API.
/// Contains information about the number of stash tabs, items within them, and currency layout configuration.
/// </summary>
public record StashTabResponse
{
    /// <summary>
    /// Gets the total number of stash tabs available.
    /// </summary>
    [JsonPropertyName("numTabs")]
    public int NumberOfTabs { get; init; }
    
    /// <summary>
    /// Gets the currency layout configuration containing sections and positioning data.
    /// This property may be null if the stash tab does not contain currency layout information.
    /// </summary>
    [JsonPropertyName("currencyLayout")]
    public CurrencyLayout? CurrencyLayout { get; init; }
    
    /// <summary>
    /// Gets the collection of items contained in the stash tabs.
    /// </summary>
    [JsonPropertyName("items")]
    public List<TradeItem> Items { get; init; } = new();
}
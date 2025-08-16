using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents pricing information for items in Path of Exile trading.
/// Contains the listing type, currency type, amount, and additional metadata about the price.
/// </summary>
public record PriceInfo
{
    /// <summary>
    /// Gets the listing type (e.g., "~price" for negotiable, "~exact" for non-negotiable).
    /// </summary>
    [JsonPropertyName("type")]
    public string PriceType { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the amount of currency required for the item.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Gets the currency type used for pricing.
    /// Uses a custom converter to map JSON string values to the TradeCurrencyType enum.
    /// </summary>
    [JsonPropertyName("currency")]
    [JsonConverter(typeof(TradeCurrencyTypeConverter))]
    public TradeCurrencyType CurrencyType { get; init; }

    /// <summary>
    /// Gets or sets whether this price is relative to another price or absolute.
    /// This property is not serialized to JSON.
    /// </summary>
    [JsonIgnore]
    public bool IsRelative { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        string output = $"{Amount} {CurrencyType}";
        return IsRelative ? $"({output.TrimStart()})" : output.TrimStart();
    }
}
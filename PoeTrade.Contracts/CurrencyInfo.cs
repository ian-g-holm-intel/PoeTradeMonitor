using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents currency information for items in Path of Exile trading.
/// Contains the currency type and amount.
/// </summary>
public record CurrencyInfo
{
    /// <summary>
    /// Type of currency.
    /// Uses a custom converter to map JSON string values to the TradeCurrencyType enum.
    /// </summary>
    [JsonPropertyName("currency")]
    [JsonConverter(typeof(TradeCurrencyTypeConverter))]
    public TradeCurrencyType Type { get; init; }

    /// <summary>
    /// Amount of currency.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Amount} {Type}";
    }
}
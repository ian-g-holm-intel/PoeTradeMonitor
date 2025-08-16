using System;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;
public class Poe2ScoutPriceInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("item_id")]
    public string? ItemId { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("currency_id")]
    public CurrencyIdentifier? CurrencyId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("nominal_price")]
    public decimal NominalPrice { get; set; }

    [JsonPropertyName("bid")]
    public bool Bid { get; set; }

    [JsonPropertyName("flag")]
    public bool Flag { get; set; }
}

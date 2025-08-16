using System;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;

public class PriceLog
{
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }
}
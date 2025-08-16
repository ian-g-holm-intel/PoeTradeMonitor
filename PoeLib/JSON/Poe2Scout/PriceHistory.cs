using System;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;
public class PriceHistory
{
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("nominal_price")]
    public decimal? NominalPrice { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}

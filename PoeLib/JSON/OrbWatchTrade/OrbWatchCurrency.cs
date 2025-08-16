using System.Text.Json.Serialization;

namespace PoeLib.JSON.OrbWatchTrade;

public class OrbWatchCurrency
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("median_price")]
    public decimal MedianPrice { get; set; }

    [JsonPropertyName("mean_price")]
    public decimal MeanPrice { get; set; }

    [JsonPropertyName("num_listings")]
    public int NumListings { get; set; }

    [JsonPropertyName("displayed_listings")]
    public int DisplayedListings { get; set; }

    [JsonPropertyName("confidence")]
    public string? Confidence { get; set; }
}
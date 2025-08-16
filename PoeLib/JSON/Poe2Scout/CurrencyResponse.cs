using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;

public class CurrencyResponse
{
    [JsonPropertyName("items")]
    public List<CurrencyItem>? Items { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("pages")]
    public int Pages { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }
}
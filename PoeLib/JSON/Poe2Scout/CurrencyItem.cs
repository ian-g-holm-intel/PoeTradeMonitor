using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;

public class CurrencyItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    [JsonPropertyName("currencyCategoryId")]
    public int CurrencyCategoryId { get; set; }

    [JsonPropertyName("apiId")]
    public string? ApiId { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("categoryApiId")]
    public string? CategoryApiId { get; set; }

    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("itemMetadata")]
    public ItemMetadata? ItemMetadata { get; set; }

    [JsonPropertyName("priceLogs")]
    public List<PriceLog>? PriceLogs { get; set; }

    [JsonPropertyName("currentPrice")]
    public decimal CurrentPrice { get; set; }
}
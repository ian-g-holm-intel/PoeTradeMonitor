using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;

public class CurrencyIdentifier
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("currency_type")]
    public string? CurrencyType { get; set; }

    [JsonPropertyName("localisation_name")]
    public string? LocalisationName { get; set; }
}
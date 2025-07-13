using System.Text.Json.Serialization;

namespace PoeLib.JSON;

public class League
{
    [JsonPropertyName("id")]
    public string Name { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("startAt")]
    public string StartDate { get; set; }
    [JsonPropertyName("endAt")]
    public string EndDate { get; set; }
}

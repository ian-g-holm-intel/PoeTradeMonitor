using System.Text.Json.Serialization;

namespace PoeLib.JSON.OrbWatchTrade;

public class OrbWatchResponse
{
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("data")]
    public OrbWatchData? Data { get; set; }
}

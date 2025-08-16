using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a request to send a whisper message to a player for item trading.
/// Contains the token and item IDs for the trade communication.
/// </summary>
public record ItemWhisperRequest
{
    /// <summary>
    /// Gets or sets the token for the whisper request.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of item values for the whisper request.
    /// </summary>
    [JsonPropertyName("values")]
    public List<int> Values { get; set; } = new List<int>();
}

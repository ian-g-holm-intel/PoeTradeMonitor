using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the response from a Path of Exile live search WebSocket connection.
/// Contains newly listed items that match the search criteria in real-time.
/// </summary>
public record LiveSearchResults
{
    /// <summary>
    /// Gets or sets the authentication status for the WebSocket connection.
    /// Indicates whether the client is properly authenticated to receive live search updates.
    /// May be null if authentication status is not provided in the message.
    /// </summary>
    [JsonPropertyName("auth")]
    public bool? Auth { get; set; }

    /// <summary>
    /// Gets or sets the list of item listing IDs for newly posted items.
    /// These IDs represent items that have just been listed and match the live search criteria.
    /// The IDs can be used to fetch detailed item information via the trade API.
    /// </summary>
    [JsonPropertyName("new")]
    public List<string> Ids { get; set; } = new List<string>();
}
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents the online status information for a player account.
/// Contains the league they are currently playing and their status (online, afk, etc.).
/// </summary>
public record OnlineStatus
{
    /// <summary>
    /// Gets the name of the league the player is currently playing in.
    /// </summary>
    [JsonPropertyName("league")]
    public string League { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the current status of the player (e.g., "online", "afk", or null for offline).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
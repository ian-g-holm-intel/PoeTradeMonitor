using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents account information for a Path of Exile player in trade listings.
/// Contains account name, online status, character information, and regional settings.
/// </summary>
public record AccountInfo
{
    /// <summary>
    /// Gets the account name of the player.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the online status of the player (online, offline, or afk).
    /// </summary>
    [JsonPropertyName("online")]
    public OnlineStatus? Online { get; init; }
    
    /// <summary>
    /// Gets the name of the last character the player was logged in with.
    /// </summary>
    [JsonPropertyName("lastCharacterName")]
    public string LastCharacterName { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the preferred language setting for the player's account.
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the game realm/server region where the player's account is located.
    /// </summary>
    [JsonPropertyName("realm")]
    public string Realm { get; init; } = string.Empty;
}
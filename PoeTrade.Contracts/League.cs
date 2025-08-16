using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a Path of Exile league with its metadata and configuration.
/// Contains information about league timing, realm, and associated URLs.
/// </summary>
public record League
{
    /// <summary>
    /// Gets the unique identifier for the league.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the display name of the league.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the realm where this league is available (e.g., "pc", "xbox", "sony").
    /// </summary>
    [JsonPropertyName("realm")]
    public string Realm { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the URL to the league's ladder page.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the start date and time of the league in ISO 8601 format.
    /// May be null for permanent leagues.
    /// </summary>
    [JsonPropertyName("startAt")]
    public string? StartAt { get; init; }
    
    /// <summary>
    /// Gets the end date and time of the league in ISO 8601 format.
    /// May be null for ongoing or permanent leagues.
    /// </summary>
    [JsonPropertyName("endAt")]
    public string? EndAt { get; init; }
}
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a trade listing with seller information, stash details, and pricing.
/// Contains all the metadata needed to contact a seller and complete a trade transaction.
/// </summary>
public record TradeListing
{
    /// <summary>
    /// Gets the trading method (e.g., "whisper" for direct message trading).
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the date and time when this listing was indexed by the trade API.
    /// </summary>
    [JsonPropertyName("indexed")]
    public DateTime Indexed { get; init; }
    
    /// <summary>
    /// Gets the stash tab information where the item is located.
    /// </summary>
    [JsonPropertyName("stash")]
    public StashInfo Stash { get; init; } = new();
    
    /// <summary>
    /// Gets the pre-formatted whisper message for contacting the seller.
    /// </summary>
    [JsonPropertyName("whisper")]
    public string Whisper { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the token for the whisper request used in trade communication.
    /// </summary>
    [JsonPropertyName("whisper_token")]
    public string WhisperToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the account information for the seller.
    /// </summary>
    [JsonPropertyName("account")]
    public AccountInfo Account { get; init; } = new();
    
    /// <summary>
    /// Gets the price information for the item listing.
    /// May be null for items without a set price.
    /// </summary>
    [JsonPropertyName("price")]
    public PriceInfo? Price { get; init; }
}
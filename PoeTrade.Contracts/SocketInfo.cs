using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents information about a socket on a Path of Exile item.
/// Contains socket color, type, linking group, and any socketed items.
/// </summary>
public record SocketInfo
{
    /// <summary>
    /// Gets the link group number for this socket. Sockets with the same group number are linked together.
    /// </summary>
    [JsonPropertyName("group")]
    public int Group { get; init; }
    
    /// <summary>
    /// Gets the attribute requirement for this socket (Strength, Dexterity, Intelligence).
    /// </summary>
    [JsonPropertyName("attr")]
    public string? Attr { get; init; }
    
    /// <summary>
    /// Gets the color of the socket (R for red, G for green, B for blue, W for white).
    /// </summary>
    [JsonPropertyName("sColour")]
    public string? SColour { get; init; }
    
    /// <summary>
    /// Gets the type of the socket (currently unused in most contexts).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
    
    /// <summary>
    /// Gets the identifier of any item socketed in this socket.
    /// </summary>
    [JsonPropertyName("item")]
    public string? Item { get; init; }
}
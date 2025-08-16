using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents socket-specific filters for Path of Exile trade searches.
/// Contains filters for socket colors, numbers, and linking requirements.
/// </summary>
public record SocketFilters
{
    /// <summary>
    /// Gets whether this socket filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific socket filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public SocketFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for socket properties in trade searches.
/// </summary>
public record SocketFilterOptions
{
    /// <summary>
    /// Gets the socket color and count filter.
    /// </summary>
    [JsonPropertyName("sockets")]
    public SocketColorFilter? Sockets { get; init; }
    
    /// <summary>
    /// Gets the linked socket color and count filter.
    /// </summary>
    [JsonPropertyName("links")]
    public SocketColorFilter? Links { get; init; }
}

/// <summary>
/// Represents filters for socket colors and counts.
/// </summary>
public record SocketColorFilter
{
    /// <summary>
    /// Gets the number of red sockets required.
    /// </summary>
    [JsonPropertyName("r")]
    public int? Red { get; init; }
    
    /// <summary>
    /// Gets the number of green sockets required.
    /// </summary>
    [JsonPropertyName("g")]
    public int? Green { get; init; }
    
    /// <summary>
    /// Gets the number of blue sockets required.
    /// </summary>
    [JsonPropertyName("b")]
    public int? Blue { get; init; }
    
    /// <summary>
    /// Gets the number of white sockets required.
    /// </summary>
    [JsonPropertyName("w")]
    public int? White { get; init; }
    
    /// <summary>
    /// Gets the minimum number of sockets/links required.
    /// </summary>
    [JsonPropertyName("min")]
    public int? Min { get; init; }
    
    /// <summary>
    /// Gets the maximum number of sockets/links required.
    /// </summary>
    [JsonPropertyName("max")]
    public int? Max { get; init; }
}
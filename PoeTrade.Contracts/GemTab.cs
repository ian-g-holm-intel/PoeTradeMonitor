using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a gem tab container with multiple pages of gem information.
/// Used for organizing skill and support gems in Path of Exile.
/// </summary>
public record GemTab
{
    /// <summary>
    /// Gets the name of the gem tab.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets the list of pages within this gem tab.
    /// </summary>
    [JsonPropertyName("pages")]
    public List<GemTabPage>? Pages { get; init; }
}
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a page within a gem tab containing statistics and information about gems.
/// </summary>
public record GemTabPage
{
    /// <summary>
    /// Gets the list of statistics or properties for gems on this page.
    /// </summary>
    [JsonPropertyName("stats")]
    public List<string>? Stats { get; init; }
}
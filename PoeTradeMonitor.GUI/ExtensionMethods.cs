using PoeTradeMonitor.GUI.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace PoeTradeMonitor.GUI;

/// <summary>
/// Utilities for JSON serialization of common types.
/// </summary>
public static class SerializationUtilities
{
    public static string Serialize(this List<string> list)
    {
        return JsonSerializer.Serialize(list);
    }

    public static List<string> DeserializeAccounts(this string listString)
    {
        if (string.IsNullOrEmpty(listString))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(listString) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public static string Serialize(this List<SearchGuiItem> items)
    {
        return JsonSerializer.Serialize(items);
    }

    public static List<SearchGuiItem> DeserializeItems(this string itemsString)
    {
        if (string.IsNullOrEmpty(itemsString))
            return new List<SearchGuiItem>();

        try
        {
            return JsonSerializer.Deserialize<List<SearchGuiItem>>(itemsString) ?? new List<SearchGuiItem>();
        }
        catch
        {
            return new List<SearchGuiItem>();
        }
    }
}

/// <summary>
/// Extension methods for various types.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Sends a text message through the WebSocket asynchronously.
    /// </summary>
    /// <param name="webSocket">The WebSocket to send the message through.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task SendAsync(this ClientWebSocket webSocket, string message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webSocket);
        ArgumentNullException.ThrowIfNull(message);

        var bytesToSend = Encoding.UTF8.GetBytes(message);
        return webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cancellationToken);
    }
}

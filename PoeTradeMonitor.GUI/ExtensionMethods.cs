using PoeLib.GuiDataClasses;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using PoeTradeMonitor.GUI.Commands;

namespace PoeTradeMonitor.GUI;

public static class TaskUtilities
{
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
    public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            handler?.HandleError(ex);
        }
    }
}

public static class SerializationUtilitiess
{
    public static string Serialize(this List<string> list)
    {
        return JsonSerializer.Serialize(list);
    }

    public static List<string> DeserializeAccounts(this string listString)
    {
        if (string.IsNullOrEmpty(listString))
            return new List<string>();

        return JsonSerializer.Deserialize<List<string>>(listString);
    }

    public static string Serialize(this List<SearchGuiItem> items)
    {
        return JsonSerializer.Serialize(items);
    }

    public static List<SearchGuiItem> DeserializeItems(this string itemsString)
    {
        if (string.IsNullOrEmpty(itemsString))
            return new List<SearchGuiItem>();

        return JsonSerializer.Deserialize<List<SearchGuiItem>>(itemsString);
    }
}

public static class ExtensionMethods
{
    public static Task SendAsync(this ClientWebSocket webSocket, string message)
    {
        if(webSocket == null)
            return Task.CompletedTask;

        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        return webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

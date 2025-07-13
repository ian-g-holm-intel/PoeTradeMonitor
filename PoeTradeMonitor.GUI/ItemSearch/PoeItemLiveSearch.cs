using System.Net.WebSockets;
using System.Text;
using PoeLib.JSON;
using PoeLib.Trade;
using System.Text.Json;
using System.Buffers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Services;
using Microsoft.Extensions.Logging;
using PoeLib.GuiDataClasses;
using PoeTradeMonitor.GUI.Clients;

namespace PoeTradeMonitor.GUI.ItemSearch;

public class PoeItemLiveSearch : IPoeItemLiveSearch
{
    private WebSocket webSocket;
    private CancellationTokenSource ctSource;
    private readonly ILiveSearchResultProcessor liveSearchResultProcessor;
    private readonly StatisticsManager statsManager;
    private readonly ILogger logger;
    private string itemName;
    private string searchId;
    private string leagueName;
    private readonly IPoeHttpClient poeHttpClient;
    private SearchGuiItem searchGuiItem;
    public bool IsAlive => webSocket != null;
    public bool Running => webSocket != null;

    public string ItemName => itemName;

    public event Action<SearchGuiItem> LiveSearchStopped;

    public PoeItemLiveSearch(IPoeHttpClient poeHttpClient, SearchGuiItem searchGuiItem, ILiveSearchResultProcessor liveSearchResultProcessor, ILogger logger, StatisticsManager statsManager)
    {
        this.liveSearchResultProcessor = liveSearchResultProcessor;
        this.poeHttpClient = poeHttpClient;
        this.searchGuiItem = searchGuiItem;
        this.statsManager = statsManager;
        this.logger = logger;
    }

    public async Task StartAsync(string league, PoeItemSearchRequest searchRequest, IPoeItemSearchRequestCache itemSearchRequestCache)
    {
        if (webSocket == null)
        {
            itemName = searchRequest.GetName();
            searchId = await itemSearchRequestCache.LookupId(league, searchRequest).ConfigureAwait(false);
            if (string.IsNullOrEmpty(searchId))
                return;

            leagueName = league;

            if (ctSource != null)
            {
                ctSource.Cancel();
                ctSource.Dispose();
            }
            ctSource = new CancellationTokenSource();

            webSocket = await poeHttpClient.InitializeWebSocket(league, searchId, ctSource.Token).ConfigureAwait(false);

            logger.LogInformation("Starting {itemName} live search", itemName);
            StartListening(ctSource.Token);
        }
    }

    public async Task StartAsync(string league)
    {
        if (webSocket == null)
        {
            itemName = searchGuiItem.Name;
            searchId = searchGuiItem.SearchID;
            leagueName = league;

            if (ctSource != null)
            {
                ctSource.Cancel();
                ctSource.Dispose();
            }
            ctSource = new CancellationTokenSource();

            webSocket = await poeHttpClient.InitializeWebSocket(league, searchId, ctSource.Token).ConfigureAwait(false);

            logger.LogInformation("Starting {itemName} live search", itemName);
            StartListening(ctSource.Token);
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (webSocket != null)
        {
            logger.LogInformation($"Websocket {itemName} shutting down");
            ctSource?.Cancel();
            try
            {
                await CloseWebSocket().ConfigureAwait(false);
            }
            catch { }
        }
    }

    private async Task RestartWebsocket(CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Restarting websocket for {itemName}", itemName);
            await CloseWebSocket().ConfigureAwait(false);
            webSocket = await poeHttpClient.InitializeWebSocket(leagueName, searchId, ctSource.Token).ConfigureAwait(false);
            logger.LogInformation($"Websocket restart for {itemName} {(webSocket?.State == WebSocketState.Open ? "succeeded" : "failed")}");
        }
        catch (OperationCanceledException)
        {
            await CloseWebSocket().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await CloseWebSocket().ConfigureAwait(false);
            logger.LogError(ex, "Failed to connect to websocket for {itemName}", itemName);
            throw;
        }
    }

    private async Task CloseWebSocket()
    {
        if (webSocket?.State == WebSocketState.Open || webSocket?.State == WebSocketState.CloseReceived)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing the connection", CancellationToken.None).ConfigureAwait(false);
        }

        webSocket?.Dispose();
        webSocket = null;
    }

    private void StartListening(CancellationToken ct)
    {
        Task.Run(async () =>
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    if (webSocket?.State == WebSocketState.Open)
                    {
                        try
                        {
                            var arrayPool = ArrayPool<byte>.Shared;
                            var messageBuffer = arrayPool.Rent(1024 * 64);
                            try
                            {
                                var receiveResult = await webSocket.ReceiveAsync(messageBuffer, ct).ConfigureAwait(false);
                                switch (receiveResult.MessageType)
                                {
                                    case WebSocketMessageType.Text:
                                        var webSocketMessage = Encoding.UTF8.GetString(messageBuffer, 0, receiveResult.Count);
                                        if (!receiveResult.EndOfMessage)
                                        {
                                            logger.LogError($"Websocket {itemName} incomplete message");
                                        }

                                        if (!string.IsNullOrEmpty(webSocketMessage))
                                            OnMessage(webSocketMessage);
                                        break;
                                    case WebSocketMessageType.Binary:
                                        logger.LogWarning("Received binary for " + itemName);
                                        break;
                                    case WebSocketMessageType.Close:
                                        if (!ct.IsCancellationRequested)
                                        {
                                            logger.LogWarning($"Websocket {itemName} closed with status: {receiveResult.CloseStatus}, restarting");
                                        }
                                        break;
                                }
                            }
                            finally
                            {
                                arrayPool.Return(messageBuffer);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (!ct.IsCancellationRequested && !ex.Message.Contains("The remote party closed the WebSocket connection without completing the close handshake"))
                            {
                                logger.LogWarning($"Websocket {itemName} error, restarting: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        if (!ct.IsCancellationRequested)
                            await RestartWebsocket(ct).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Websocket {itemName} stopped", itemName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Websocket {itemName} stopped unexpectedly", itemName);
                LiveSearchStopped?.Invoke(searchGuiItem);
            }
        }, ct);
    }

    private void OnMessage(string message)
    {
        var newResults = JsonSerializer.Deserialize<PoeItemNewResults>(message);
        if (newResults.auth.HasValue)
            return;

        statsManager.LogWebsocketItemsReceived(searchGuiItem, newResults.@new.Count);
        liveSearchResultProcessor.QueueItems(newResults.@new.Select(result => new ItemSearchRequest { ID = result, SearchID = searchId, ListingTime = DateTime.Now, SearchGuiItem = searchGuiItem }));
    }
}

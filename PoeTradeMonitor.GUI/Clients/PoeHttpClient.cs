using ComposableAsync;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PoeAuthenticator;
using PoeLib;
using PoeLib.JSON;
using PoeLib.JSON.Currency;
using PoeLib.Settings;
using PoeLib.Trade;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace PoeTradeMonitor.GUI.Clients;

public interface IPoeHttpClient
{
    Task<HttpResponseMessage> GetBackendRequest(string league, int tabIndex);
    Task<HttpResponseMessage> GetFetchRequest(IEnumerable<string> searchIDs);
    Task<HttpResponseMessage> GetLeagues();
    Task<WebSocket> InitializeWebSocket(string league, string searchId, CancellationToken ct);
    Task<HttpResponseMessage> PostExchangeRequest(string league, CurrencyExchangeRequest exchangeRequest);
    Task<HttpResponseMessage> PostSearchRequest(string league, PoeItemSearchRequest searchRequest);
    Task SendTradeWhisper(string searchId, string itemId, int count);
    void SendTradeWhisperFromToken(string searchId, string whisper_token, int count);
}

public class PoeHttpClient : IPoeHttpClient
{
    private static readonly AsyncLock asyncLock = new();
    private readonly ILogger<PoeHttpClient> logger;
    private readonly PoeSettings settings;
    private readonly CookieContainer cookieContainer;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IPoeRateLimitService poeRateLimitService;

    public PoeHttpClient(ILogger<PoeHttpClient> logger, PoeSettings settings, CookieContainer cookieContainer, IHttpClientFactory httpClientFactory, IPoeRateLimitService poeRateLimitService)
    {
        this.logger = logger;
        this.settings = settings;
        this.cookieContainer = cookieContainer;
        this.httpClientFactory = httpClientFactory;
        this.poeRateLimitService = poeRateLimitService;
    }

    public async Task<HttpResponseMessage> GetLeagues()
    {
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Get, "https://api.pathofexile.com/leagues?type=main&compact=1");
        AddApiHeaders(message);
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            return await client.SendAsync(message).ConfigureAwait(false);
        }
    }

    public async Task<HttpResponseMessage> GetBackendRequest(string league, int tabIndex)
    {
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Get, $"/character-window/get-stash-items?league={league}&tabIndex={tabIndex}&accountName={settings.Account}");
        AddApiHeaders(message);
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            return await client.SendAsync(message).ConfigureAwait(false);
        }
    }

    public async Task<HttpResponseMessage> GetFetchRequest(IEnumerable<string> searchIDs)
    {
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Get, $"/api/trade/fetch/{string.Join(",", searchIDs)}");
        AddApiHeaders(message);
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            return await client.SendAsync(message).ConfigureAwait(false);
        }
    }

    public async Task<HttpResponseMessage> PostSearchRequest(string league, PoeItemSearchRequest searchRequest)
    {
        var stringContent = searchRequest.ToStringContent(out var json);
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Post, $"/api/trade/search/{league}");
        AddApiHeaders(message);
        message.Content = stringContent;
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            return await client.SendAsync(message).ConfigureAwait(false);
        }
    }

    public async Task<HttpResponseMessage> PostExchangeRequest(string league, CurrencyExchangeRequest exchangeRequest)
    {
        var stringContent = exchangeRequest.ToStringContent(out var json);
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Post, $"/api/trade/exchange/{league}");
        AddApiHeaders(message);
        message.Content = stringContent;
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            return await client.SendAsync(message).ConfigureAwait(false);
        }
    }

    private async Task<HttpResponseMessage> RefreshSearchPage(string league)
    {
        var client = httpClientFactory.CreateClient("PoeApi");
        var message = new HttpRequestMessage(HttpMethod.Get, "/trade/search/Standard");
        AddWebHeaders(message);
        return await client.SendAsync(message).ConfigureAwait(false);
    }

    public async Task SendTradeWhisper(string searchId, string itemId, int count)
    {
        var fetchResponse = await GetFetchRequest(new[] { itemId }).ConfigureAwait(false);
        using var jsonStream = await fetchResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var searchResult = await JsonSerializer.DeserializeAsync<PoeItemSearchResults>(jsonStream).ConfigureAwait(false);

        var itemSearchResult = searchResult.result.First();
        if (itemSearchResult == null)
        {
            logger.LogWarning($"Failed to send whisper for item {itemId} because it is no longer listed");
            return;
        }

        var whisper_token = searchResult.result.First().listing.whisper_token;
        if (whisper_token == null)
        {
            logger.LogError($"Failed to send whisper for item {itemId} because whisper_token was null");
            return;
        }

        SendTradeWhisperFromToken(searchId, whisper_token, count);
    }

    public void SendTradeWhisperFromToken(string searchId, string whisper_token, int count)
    {
        var itemWhisper = new ItemWhisper { token = whisper_token };
        if (count > 0)
            itemWhisper.values = new() { count };

        _ = Task.Run(async () =>
        {
            for (int i = 0; i < 3; i++)
            {
                var message = new HttpRequestMessage(HttpMethod.Post, "https://www.pathofexile.com/api/trade/whisper");
                AddApiHeaders(message);
                message.Headers.Add("Referer", $"https://www.pathofexile.com/trade/search/{settings.League}/{searchId}");
                string jsonPayload = JsonSerializer.Serialize(itemWhisper, options: new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                message.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var client = httpClientFactory.CreateClient("PoeApi");
                using (await asyncLock.LockAsync().ConfigureAwait(false))
                {
                    var whisperResponse = await client.SendAsync(message).ConfigureAwait(false);
                    if (whisperResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return;
                    }
                    logger.LogError($"Failed to send whisper, whisper_token: {whisper_token}, StatusCode: {whisperResponse.StatusCode}");
                }
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        });
    }

    private ClientWebSocket GetWebSocketClient(Uri requestUri)
    {
        var cookies = cookieContainer.GetCookies(new Uri("https://www.pathofexile.com"));
        var cookieHeader = string.Join("; ", cookies.Cast<Cookie>().Select(c => $"{c.Name}={c.Value}"));

        var client = new ClientWebSocket { Options = { KeepAliveInterval = TimeSpan.Zero } };
        client.Options.SetRequestHeader("Accept-Encoding", "gzip, deflate, br, zstd");
        client.Options.SetRequestHeader("Accept-Language", "en-US,en;q=0.9");
        client.Options.SetRequestHeader("Cache-Control", "no-cache");
        client.Options.SetRequestHeader("Pragma", "no-cache");
        client.Options.SetRequestHeader("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits");
        client.Options.SetRequestHeader("cookie", cookieHeader);
        client.Options.SetRequestHeader("Host", "www.pathofexile.com");
        client.Options.SetRequestHeader("Origin", "https://www.pathofexile.com");
        client.Options.SetRequestHeader("user-agent", PoeSettings.UserAgent);

        return client;
    }

    public async Task<WebSocket> InitializeWebSocket(string league, string searchId, CancellationToken ct)
    {
        //await GetSearchRequest(league, searchId);
        var serverUri = new Uri($"wss://www.pathofexile.com/api/trade/live/{league}/{searchId}");
        
        using (await asyncLock.LockAsync().ConfigureAwait(false))
        {
            try
            {
                var webSocket = GetWebSocketClient(serverUri);
                if (webSocket == null)
                    throw new Exception($"Failed to get WebSocketClient for SearchID: {searchId}");

                await poeRateLimitService.WebSocketLimiter;
                await webSocket.ConnectAsync(serverUri, ct).ConfigureAwait(false);
                return webSocket;
            }
            catch (WebSocketException)
            {
                await RefreshSearchPage(league);

                var webSocket = GetWebSocketClient(serverUri);
                if (webSocket == null)
                    throw new Exception($"Failed to get WebSocketClient for SearchID: {searchId}");

                await poeRateLimitService.WebSocketLimiter;
                await webSocket.ConnectAsync(serverUri, ct).ConfigureAwait(false);
                return webSocket;
            }
        }
    }

    private HttpRequestMessage AddApiHeaders(HttpRequestMessage message)
    {
        message.Headers.Add("Origin", "https://www.pathofexile.com");
        message.Headers.Add("Accept", "*/*");
        message.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        message.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        message.Headers.Add("Priority", "u=1, i");
        message.Headers.Add("Sec-Ch-Ua", PoeSettings.SecChUa);
        message.Headers.Add("Sec-Ch-Ua.Arch", "\"x86\"");
        message.Headers.Add("Sec-Ch-Ua.Bitness", "\"64\"");
        message.Headers.Add("Sec-Ch-Ua-Full-Version-List", PoeSettings.SecChUaFullVersionList);
        message.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        message.Headers.Add("Sec-Ch-Ua-Model", "\"\"");
        message.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
        message.Headers.Add("Sec-Ch-Ua-Platform-Version", settings.WindowsVersion);
        message.Headers.Add("Sec-Fetch-Dest", "empty");
        message.Headers.Add("Sec-Fetch-Mode", "cors");
        message.Headers.Add("Sec-Fetch-Site", "same-origin");
        message.Headers.Add("Sec-Gpc", "1");
        message.Headers.Add("X-Requested-With", "XMLHttpRequest");
        message.Headers.UserAgent.ParseAdd(PoeSettings.UserAgent);
        return message;
    }

    private HttpRequestMessage AddWebHeaders(HttpRequestMessage message)
    {
        message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        message.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        message.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        message.Headers.Add("Priority", "u=0, i");
        message.Headers.Add("Sec-Ch-Ua", PoeSettings.SecChUa);
        message.Headers.Add("Sec-Ch-Ua.Arch", "\"x86\"");
        message.Headers.Add("Sec-Ch-Ua.Bitness", "\"64\"");
        message.Headers.Add("Sec-Ch-Ua-Full-Version-List", PoeSettings.SecChUaFullVersionList);
        message.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        message.Headers.Add("Sec-Ch-Ua-Model", "\"\"");
        message.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
        message.Headers.Add("Sec-Ch-Ua-Platform-Version", settings.WindowsVersion);
        message.Headers.Add("Sec-Fetch-Dest", "document");
        message.Headers.Add("Sec-Fetch-Mode", "navigate");
        message.Headers.Add("Sec-Fetch-Site", "none");
        message.Headers.Add("Sec-Fetch-User", "?1");
        message.Headers.Add("Sec-Gpc", "1");
        message.Headers.Add("upgrade-insecure-requests", "1");
        message.Headers.UserAgent.ParseAdd(PoeSettings.UserAgent);
        return message;
    }
}

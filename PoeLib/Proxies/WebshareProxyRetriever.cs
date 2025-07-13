using PoeLib.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;

namespace PoeLib.Proxies;

public class WebshareProxyRetriever : IProxyRetriever
{
    private readonly ILogger<WebshareProxyRetriever> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IProxySpeedTester speedTester;
    private bool doSpeedTest = true;
    private const string serverListAPI = @"https://proxy.webshare.io/api/proxy/list/";

    public WebshareProxyRetriever(IProxySpeedTester speedTester, ILogger<WebshareProxyRetriever> logger, IHttpClientFactory httpClientFactory)
    {
        this.speedTester = speedTester;
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<Dictionary<string, WebProxy>> GetProxiesAsync()
    {
        var webProxies = new Dictionary<string, WebProxy>();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, serverListAPI);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", Constants.WebshareApiKey);
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(requestMessage);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return webProxies;
        }

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var allServers = JsonSerializer.Deserialize<WebshareProxyServers>(json);
            foreach (var server in allServers.results)
            {
                var proxy = server.proxy_address.GetProxy(server.ports.http.ToString(), server.username, server.password);
                if (doSpeedTest)
                {
                    server.speed = await speedTester.GetDownloadSpeed(proxy);
                    if (server.speed > 0)
                    {
                        logger.LogInformation("Accepting Webshare Server: {server}, Speed: {speed}", server.proxy_address, server.speed);
                        webProxies[server.proxy_address] = proxy;
                    }
                    else
                    {
                        logger.LogWarning("Rejecting Webshare Server: {server}", server.proxy_address);
                    }
                }
                else
                {
                    try
                    {
                        var httpClientHandler = new HttpClientHandler { Proxy = proxy };
                        using var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                        using var timeout = new CancellationTokenSource(2000);
                        var requestResponse = await client.GetAsync("http://www.google.com", timeout.Token);
                        if (requestResponse.StatusCode == HttpStatusCode.OK)
                        {
                            logger.LogInformation("Accepting Webshare Server: {server}", server.proxy_address);
                            webProxies[server.proxy_address] = proxy;
                        }
                        else
                        {
                            logger.LogWarning("Rejecting Webshare Server: {server}", server.proxy_address);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Rejecting Webshare Server: {server}", server.proxy_address);
                    }
                }
            }
        }
        catch (Exception)
        {
            logger.LogError($"Failed to deserialize proxy list: {json}");
        }
        return webProxies;
    }
}

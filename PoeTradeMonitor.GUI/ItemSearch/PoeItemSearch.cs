using System.Net;
using Microsoft.Extensions.Logging;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Clients;
using System.Net.Http.Json;
using PoeTrade.Contracts;
using PoeLib.Common;

namespace PoeTradeMonitor.GUI.ItemSearch;

public class PoeItemSearch : IPoeItemSearch
{
    private readonly ILogger<PoeItemSearch> log;
    private readonly IPoeHttpClient poeHttpClient;

    public PoeItemSearch(ILogger<PoeItemSearch> log, IPoeHttpClient poeHttpClient)
    {
        this.log = log;
        this.poeHttpClient = poeHttpClient;
    }

    public async Task<TradeSearchResponse?> SearchAsync(string league, TradeSearchRequest request)
    {
        var response = await poeHttpClient.PostSearchRequest(league, request);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new LiveSearchException($"Rate Limited");
        }
        else if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new LiveSearchException($"Failed to retrieve search results: {response.RequestMessage}");
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<TradeSearchResponse>();
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize search results for {request.Query.Name ?? request.Query.Type ?? ""} - {ex}");
            throw;
        }
    }

    public async Task<TradeFetchResponse?> FetchItemResults(IEnumerable<string> ids)
    {
        var response = await poeHttpClient.GetFetchRequest(ids.Take(10));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            log.LogError("Failed to fetch item results - Status: " + response.StatusCode);
            return null;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<TradeFetchResponse>();
        }
        catch (Exception ex)
        {
            var json = await response.Content.ReadAsStringAsync();
            log.LogError($"Failed to deserialize fetch results: {json} - {ex}");
            throw;
        }
    }
}

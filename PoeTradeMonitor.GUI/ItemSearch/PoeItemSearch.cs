using System.Net;
using Microsoft.Extensions.Logging;
using PoeLib;
using PoeLib.JSON;
using PoeLib.Trade;
using System.Text.Json;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Clients;

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

    public async Task<PoeItemSearchResponse> SearchAsync(string league, PoeItemSearchRequest request)
    {
        var result = await poeHttpClient.PostSearchRequest(league, request);
        if (result.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new LiveSearchException($"Rate Limited");
        }
        else if (result.StatusCode != HttpStatusCode.OK)
        {
            throw new LiveSearchException($"Failed to retrieve search results: {result.RequestMessage}");
        }

        using (var jsonStream = await result.Content.ReadAsStreamAsync())
        {
            try
            {
                var response = await JsonSerializer.DeserializeAsync<PoeItemSearchResponse>(jsonStream);
                if (response.result == null)
                {
                    log.LogError($"Failed to retrieve search results for {request.query.name ?? request.query.type ?? ""}");
                }
                return response;
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to deserialize search results for {request.query.name ?? request.query.type ?? ""} - {ex}");
                throw;
            }
        }
    }

    public async Task<PoeItemSearchResults> FetchItemResults(IEnumerable<string> ids)
    {
        var response = await poeHttpClient.GetFetchRequest(ids.Take(10));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            log.LogError("Failed to fetch item results - Status: " + response.StatusCode);
            return null;
        }

        using (var jsonStream = await response.Content.ReadAsStreamAsync())
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<PoeItemSearchResults>(jsonStream);
            }
            catch (Exception ex)
            {
                var json = await response.Content.ReadAsStringAsync();
                log.LogError($"Failed to deserialize fetch results: {json} - {ex}");
                throw;
            }
        }
    }
}

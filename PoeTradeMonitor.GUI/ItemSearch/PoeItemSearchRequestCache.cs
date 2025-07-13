using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PoeLib.Trade;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.ItemSearch;

public class PoeItemSearchRequestCache : IPoeItemSearchRequestCache
{
    private static readonly string cacheFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoeItemSearchRequestCache.json");
    private readonly IPoeItemSearch poeItemSearch;
    private readonly ILogger<PoeItemSearchRequestCache> log;
    private readonly IPoeHttpClient poeHttpClient;

    private static ConcurrentDictionary<string, string> cache { get; set; }

    public PoeItemSearchRequestCache(IPoeItemSearch poeItemSearch, ILogger<PoeItemSearchRequestCache> log, IPoeHttpClient poeHttpClient)
    {
        this.poeItemSearch = poeItemSearch;
        this.log = log;
        this.poeHttpClient = poeHttpClient;
        if (File.Exists(cacheFilePath))
        {
            using (var fileStream = new FileStream(cacheFilePath, FileMode.Open))
            {
                cache = new ConcurrentDictionary<string, string>(JsonSerializer.DeserializeAsync<Dictionary<string, string>>(fileStream).Result.OrderBy(kvp => kvp.Key));
            }
        }
        else
        {
            cache = new ConcurrentDictionary<string, string>();
        }
    }

    public async Task<string> LookupId(string league, PoeItemSearchRequest request)
    {
        var maxPrice = request?.query?.filters?.trade_filters?.filters?.price?.max;
        var searchKey = $"{request.GetName()} - {league}{(maxPrice != null ? $" - {maxPrice}" : string.Empty)}";
        if (!cache.ContainsKey(searchKey))
        {
            var searchResponse = await poeItemSearch.SearchAsync(league, request);
            if (searchResponse.id == null)
            {
                log.LogError($"Failed to start {request.GetName()} websocket");
                return string.Empty;
            }

            cache[searchKey] = searchResponse.id;
        }
        return cache[searchKey];
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                using (var fileStream = new FileStream(cacheFilePath, FileMode.Create))
                {
                    using (var writer = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = true }))
                    {
                        JsonSerializer.Serialize(writer, cache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                    }
                }
            }

            disposed = true;

        }
    }
}

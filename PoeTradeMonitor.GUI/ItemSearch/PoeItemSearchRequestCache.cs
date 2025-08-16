using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.ItemSearch;

public class PoeItemSearchRequestCache : IPoeItemSearchRequestCache
{
    private static readonly string cacheFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoeItemSearchRequestCache.json");
    private readonly IPoeItemSearch poeItemSearch;
    private readonly ILogger<PoeItemSearchRequestCache> log;

    private static ConcurrentDictionary<string, string> cache { get; set; } = new ConcurrentDictionary<string, string>();

    public PoeItemSearchRequestCache(IPoeItemSearch poeItemSearch, ILogger<PoeItemSearchRequestCache> log)
    {
        this.poeItemSearch = poeItemSearch;
        this.log = log;
        _ = InitializeCacheAsync(); // Fire and forget initialization
    }

    /// <summary>
    /// Initializes the cache from disk asynchronously.
    /// </summary>
    private async Task InitializeCacheAsync()
    {
        if (File.Exists(cacheFilePath))
        {
            try
            {
                using var fileStream = new FileStream(cacheFilePath, FileMode.Open, FileAccess.Read);
                var result = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(fileStream);
                if (result != null)
                    cache = new ConcurrentDictionary<string, string>(result.OrderBy(kvp => kvp.Key));
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to load cache from disk, starting with empty cache");
            }
        }
    }

    public async Task<string> LookupId(string league, TradeSearchRequest request)
    {
        var maxPrice = request.Query?.Filters?.TradeFilters?.Filters?.Price?.Max;
        var searchKey = $"{request} - {league}{(maxPrice != null ? $" - {maxPrice}" : string.Empty)}";
        if (!cache.ContainsKey(searchKey))
        {
            var searchResponse = await poeItemSearch.SearchAsync(league, request);
            if (searchResponse == null || searchResponse.Id == null)
            {
                log.LogError($"Failed to start {request} websocket");
                return string.Empty;
            }

            cache[searchKey] = searchResponse.Id;
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

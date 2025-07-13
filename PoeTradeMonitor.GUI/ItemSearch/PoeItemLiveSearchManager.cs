using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.Tools;
using PoeLib.Trade;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.ItemSearch;

public class PoeItemLiveSearchManager : IPoeItemLiveSearchManager
{
    private readonly ILiveSearchResultProcessor liveSearchResultProcessor;
    private readonly ILogger<PoeItemLiveSearchManager> logger;
    private readonly ICurrencyCache currencyCache;
    private readonly StatisticsManager statsManager;
    private readonly ILiveSearchItemCache liveSearchItemCache;
    private readonly IPoeHttpClient poeHttpClient;
    private readonly IPoeItemSearchRequestCache poeItemSearchRequestCache;
    private readonly ConcurrentDictionary<SearchGuiItem, IPoeItemLiveSearch> liveSearches = new ConcurrentDictionary<SearchGuiItem, IPoeItemLiveSearch>();

    public PoeItemLiveSearchManager(
        ILiveSearchResultProcessor liveSearchResultProcessor, 
        IPoeItemSearchRequestCache poeItemSearchRequestCache, 
        ILogger<PoeItemLiveSearchManager> logger, 
        ICurrencyCache currencyCache, 
        StatisticsManager statsManager, 
        ILiveSearchItemCache liveSearchItemCache,
        IPoeHttpClient poeHttpClient)
    {
        this.liveSearchResultProcessor = liveSearchResultProcessor;
        this.logger = logger;
        this.currencyCache = currencyCache;
        this.statsManager = statsManager;
        this.liveSearchItemCache = liveSearchItemCache;
        this.poeHttpClient = poeHttpClient;
        this.poeItemSearchRequestCache = poeItemSearchRequestCache;
    }

    public bool IsConnected => liveSearches.Count != 0 && liveSearches.Values.All(ls => ls.IsAlive);

    public async Task UpdateLiveSearchesAsync(Strictness strictness, string league, IEnumerable<SearchGuiItem> searchGuiItems)
    {
        try
        {
            // Stop dead live searches
            foreach (var searchGuiItem in liveSearches.Keys)
            {
                if (!liveSearches[searchGuiItem].IsAlive)
                {
                    logger.LogInformation($"Stopping {searchGuiItem.Name} because it is no longer alive");
                    await StopLiveSearchAsync(searchGuiItem);
                }
            }

            // Remove items removed from dealfinder
            foreach (var item in liveSearches.Keys.Where(i => !searchGuiItems.Contains(i)).ToArray())
            {
                logger.LogInformation($"Stopping {item.Name} because it is no longer present in dealfinder list");
                await StopLiveSearchAsync(item);
            }

            var accountItems = new List<SearchGuiItem>();
            foreach (var item in searchGuiItems)
            {
                if (liveSearches.ContainsKey(item))
                {
                    var existingItem = liveSearches.Keys.Single(i => i.Equals(item));
                    existingItem.OfferPrice = item.OfferPrice;
                }
                else
                {
                    if (liveSearchItemCache.TryAddItem(item))
                        accountItems.Add(item);
                }
            }
            await StartLiveSearches(strictness, league, accountItems);

            logger.LogInformation($"Currently monitoring {liveSearches.Keys.Count} searches");
        }
        catch (Exception ex)
        {
            logger.LogError("Unexpected error in LiveSearch Manager: {ex}", ex);
        }
    }

    private Task StartLiveSearches(Strictness strictness, string league, IEnumerable<SearchGuiItem> items)
    {
        return Task.Run(async () =>
        {
            foreach (var item in items)
            {
                await CreateLiveSearchAsync(strictness, league, item);
            }
        });
    }

    private async Task StopLiveSearchAsync(SearchGuiItem item)
    {
        if (liveSearches.TryRemove(item, out var liveSearch))
        {
            await liveSearch.StopAsync();
            liveSearchItemCache.RemoveItem(item);
        }
    }

    private async Task CreateLiveSearchAsync(Strictness strictness, string league, SearchGuiItem searchGuiItem)
    {
        try
        {
            var newLiveSearch = new PoeItemLiveSearch(poeHttpClient, searchGuiItem, liveSearchResultProcessor, logger, statsManager);
            liveSearches[searchGuiItem] = newLiveSearch;

            if (!string.IsNullOrEmpty(searchGuiItem.SearchID))
            {
                await newLiveSearch.StartAsync(league);
            }
            else
            {
                var searchRequest = new PoeItemSearchRequest
                {
                    query = new Query
                    {
                        filters = new Filters
                        {
                            trade_filters = new TradeFilters
                            {
                                filters = new FilterTypes
                                {
                                    price = new Price
                                    {
                                        min = 1,
                                        max = strictness == Strictness.Chaos7 ? currencyCache.GetChaosCount() : null
                                    }
                                }
                            }
                        }
                    }
                };

                //if (searchGuiItem.Rarity == Rarity.Currency || searchGuiItem.Rarity == Rarity.DivinationCard || searchGuiItem.Rarity == Rarity.Gem || searchGuiItem.Rarity == Rarity.Normal)
                //    searchRequest.query.type = searchGuiItem.Name;
                //else
                //{
                //    searchRequest.query.name = searchGuiItem.Name;
                //    searchRequest.query.type = searchGuiItem.BaseType;
                //}

                //if (!searchGuiItem.AllowCorrupted)
                //{
                //    searchRequest.query.filters.misc_filters = new MiscFilters
                //    {
                //        filters = new FilterTypes
                //        {
                //            corrupted = new Corrupted
                //            {
                //                option = false
                //            }
                //        }
                //    };
                //}

                //if (searchGuiItem.Rarity == Rarity.Unique || searchGuiItem.Rarity == Rarity.UniqueFoil)
                //{
                //    searchRequest.query.filters.type_filters = new TypeFilters
                //    {
                //        filters = new FilterTypes
                //        {
                //            rarity = new ItemRarity
                //            {
                //                option = searchGuiItem.Rarity == Rarity.UniqueFoil ? PoeItemRarity.uniquefoil.ToString() : PoeItemRarity.unique.ToString()
                //            }
                //        },
                //    };
                //}

                //if (searchGuiItem.MapTier != 0)
                //{
                //    searchRequest.query.filters.map_filters = new MapFilters
                //    {
                //        filters = new FilterTypes
                //        {
                //            map_tier = new MapTier
                //            {
                //                min = searchGuiItem.MapTier,
                //                max = searchGuiItem.MapTier
                //            }
                //        }
                //    };
                //}

                //if (searchGuiItem.Links > 4)
                //{
                //    searchRequest.query.filters.socket_filters = new SocketFilters { filters = new FilterTypes { links = new Links { min = searchGuiItem.Links, max = searchGuiItem.Links } } };
                //}

                //if (!string.IsNullOrEmpty(searchGuiItem.Variant))
                //{
                //    switch (searchGuiItem.Variant)
                //    {
                //        case "1 Jewel":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_3527617737", value = new Value { min = 1, max = 1 }, disabled = false });
                //            break;
                //        case "2 Jewels":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_3527617737", value = new Value { min = 2, max = 2 }, disabled = false });
                //            break;
                //        case "1 passive":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1085446536", value = new Value { min = 1, max = 1 }, disabled = false });
                //            break;
                //        case "3 passives":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1085446536", value = new Value { min = 3, max = 3 }, disabled = false });
                //            break;
                //        case "5 passives":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1085446536", value = new Value { min = 5, max = 5 }, disabled = false });
                //            break;
                //        case "7 passives":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1085446536", value = new Value { min = 7, max = 7 }, disabled = false });
                //            break;
                //        case "Maim":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1753916791", disabled = false });
                //            break;
                //        case "Poison":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_1114411822", disabled = false });
                //            break;
                //        case "Bleeding":
                //            searchRequest.query.stats[0].filters.Add(new StatFilter { id = "explicit.stat_4058504226", disabled = false });
                //            break;
                //        default:
                //            logger.LogError("Unhandled variant for {item}: {variant}", searchGuiItem.Name, searchGuiItem.Variant);
                //            break;
                //    }
                //}

                await newLiveSearch.StartAsync(league, searchRequest, poeItemSearchRequestCache);
            }
        }
        catch (ProxyNotAvailableException)
        {
            logger.LogWarning("Not enough proxies or poe accounts to create live search for: {item}", searchGuiItem);
        }
        catch (WebSocketException ex)
        {
            if (ex.InnerException is WebException webEx)
            {
                logger.LogError("Failed to start {itemName} live search: {ex}", searchGuiItem.Name, webEx);
            }
            else
            {
                logger.LogError("Failed to start {itemName} live search: {ex}", searchGuiItem.Name, ex);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to start {itemName} live search: {ex}", searchGuiItem.Name, ex);
        }
    }

    public async Task StopAllLiveSearchesAsync()
    {
        logger.LogInformation("Stopping all live searches");
        var stopTasks = new List<Task>();
        foreach (var item in liveSearches.Keys)
        {
            if (liveSearches.TryRemove(item, out IPoeItemLiveSearch liveSearch))
                stopTasks.Add(liveSearch.StopAsync());
        }
        await Task.WhenAll(stopTasks).ConfigureAwait(false);
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            logger.LogInformation("Disposing PoeTradeLiveSearchManager");
            AsyncContext.Run(() => StopAllLiveSearchesAsync());
        }

        disposed = true;
    }
    #endregion
}

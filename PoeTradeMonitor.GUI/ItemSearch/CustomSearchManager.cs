using Microsoft.Extensions.Logging;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.ViewModels;
using System.Collections.Concurrent;

namespace PoeTradeMonitor.GUI.ItemSearch;

public interface ICustomSearchManager
{
    Task StartSearchAsync(SearchGuiItem searchGuiItem);
    Task StopSearchAsync(SearchGuiItem searchGuiItem);
    void SetViewModel(MainWindowViewModel vm);
}

public class CustomSearchManager : ICustomSearchManager
{
    private readonly ILiveSearchResultProcessor liveSearchResultProcessor;
    private readonly ILogger<CustomSearchManager> logger;
    private readonly IPoeHttpClient poeHttpClient;
    private readonly PoeSettings poeSettings;
    private readonly StatisticsManager statsManager;
    private readonly IPoeItemSearchRequestCache poeItemSearchRequestCache;
    private readonly IPoePriceChecker poePriceChecker;
    private MainWindowViewModel? mainWindowViewModel;
    private readonly ConcurrentDictionary<SearchGuiItem, PoeItemLiveSearch> liveSearches = new();

    public CustomSearchManager(ILiveSearchResultProcessor liveSearchResultProcessor, ILogger<CustomSearchManager> logger, IPoeHttpClient poeHttpClient, PoeSettings poeSettings,
        StatisticsManager statsManager, IPoeItemSearchRequestCache poeItemSearchRequestCache, IPoePriceChecker poePriceChecker)
    {
        this.liveSearchResultProcessor = liveSearchResultProcessor;
        this.logger = logger;
        this.poeHttpClient = poeHttpClient;
        this.poeSettings = poeSettings;
        this.statsManager = statsManager;
        this.poeItemSearchRequestCache = poeItemSearchRequestCache;
        this.poePriceChecker = poePriceChecker;
    }

    public void SetViewModel(MainWindowViewModel vm)
    {
        mainWindowViewModel = vm;
    }

    public async Task StartSearchAsync(SearchGuiItem searchGuiItem)
    {
        if (!liveSearches.ContainsKey(searchGuiItem))
        {
            await StartLiveSearchAsync(searchGuiItem);
            if (mainWindowViewModel != null)
                mainWindowViewModel.Connected = true;
        }
        else
        {
            throw new ArgumentException($"Failed to start search, {nameof(SearchGuiItem)} {searchGuiItem.Name} did not have SearchID");
        }
    }

    private async Task StartLiveSearchAsync(SearchGuiItem searchGuiItem)
    {
        if (!liveSearches.ContainsKey(searchGuiItem))
        {
            var liveSearch = new PoeItemLiveSearch(poeHttpClient, searchGuiItem, liveSearchResultProcessor, logger, statsManager);
            liveSearch.LiveSearchStopped += LiveSearch_LiveSearchStopped;
            if (string.IsNullOrEmpty(searchGuiItem.SearchID))
            {
                var searchRequest = CreateBasicRequest(searchGuiItem.Name, searchGuiItem.IsTypeName);
                poePriceChecker.StartMonitoringItem(searchRequest);
                searchGuiItem.AutoSearchID = await poeItemSearchRequestCache.LookupId(poeSettings.League, searchRequest);
            }
            await liveSearch.StartAsync(poeSettings.League);
            liveSearches[searchGuiItem] = liveSearch;
            if (mainWindowViewModel != null)
                mainWindowViewModel.Connected = true;
        }
        else
        {
            logger.LogWarning($"Live search for {searchGuiItem.Name} already exists");
        }
    }

    public static TradeSearchRequest CreateBasicRequest(string name, bool nameIsType = false)
    {
        var itemName = nameIsType ? string.Empty : name;
        var itemType = nameIsType ? name : string.Empty;

        return new TradeSearchRequest
        {
            Query = new TradeQuery
            {
                Status = new StatusFilter
                {
                    Option = "online"
                },
                Name = !string.IsNullOrEmpty(itemName) ? itemName : null,
                Type = !string.IsNullOrEmpty(itemType) ? itemType : null,
                Stats = new List<StatsFilter>
                    {
                        new StatsFilter
                        {
                            Type = "and",
                            Filters = new()
                        }
                    },
                Filters = new QueryFilters
                {
                    MiscFilters = new MiscFilters
                    {
                        Filters = new MiscFilterOptions
                        {
                            Corrupted = new OptionFilter
                            {
                                Option = "false"
                            }
                        },
                        Disabled = false
                    },
                    TradeFilters = new TradeFilters
                    {
                        Filters = new TradeFilterOptions
                        {
                            Price = new PriceFilter
                            {
                                Min = null,
                                Max = null,
                                Option = "divine"
                            }
                        },
                        Disabled = false
                    }
                }
            },
            Sort = new SortOptions
            {
                Price = "asc"
            }
        };
    }

    private async void LiveSearch_LiveSearchStopped(SearchGuiItem obj)
    {
        try
        {
            if (mainWindowViewModel != null)
            {
                await mainWindowViewModel.DisableSearchItem(obj);
                mainWindowViewModel.Connected = false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception caught invoking {nameof(mainWindowViewModel.DisableSearchItem)}");
        }
    }

    public async Task StopSearchAsync(SearchGuiItem searchGuiItem)
    {
        if (liveSearches.TryRemove(searchGuiItem, out var liveSearch))
        {
            await liveSearch.StopAsync();
            if (!string.IsNullOrEmpty(searchGuiItem.AutoSearchID))
                poePriceChecker.StopMonitoringItem(searchGuiItem.Name);
        }
    }
}

using Microsoft.Extensions.Logging;
using PoeLib.GuiDataClasses;
using PoeLib.Settings;
using PoeLib.Tools;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.Interfaces;
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
    private readonly ISettingsManager settingsManager;
    private readonly StatisticsManager statsManager;
    private readonly ICurrencyCache currencyCache;
    private readonly ITradeRequestScheduler tradeRequestScheduler;
    private MainWindowViewModel mainWindowViewModel;
    private readonly ConcurrentDictionary<SearchGuiItem, PoeItemLiveSearch> liveSearches = new();

    public CustomSearchManager(ILiveSearchResultProcessor liveSearchResultProcessor, ILogger<CustomSearchManager> logger, IPoeHttpClient poeHttpClient, ISettingsManager settingsManager,
        StatisticsManager statsManager, ICurrencyCache currencyCache, ITradeRequestScheduler tradeRequestScheduler)
    {
        this.liveSearchResultProcessor = liveSearchResultProcessor;
        this.logger = logger;
        this.poeHttpClient = poeHttpClient;
        this.settingsManager = settingsManager;
        this.statsManager = statsManager;
        this.currencyCache = currencyCache;
        this.tradeRequestScheduler = tradeRequestScheduler;
    }

    public void SetViewModel(MainWindowViewModel vm)
    {
        mainWindowViewModel = vm;
    }

    public async Task StartSearchAsync(SearchGuiItem searchGuiItem)
    {
        if (!string.IsNullOrEmpty(searchGuiItem.SearchID))
        {
            await StartLiveSearchAsync(searchGuiItem);
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
            await liveSearch.StartAsync(settingsManager.Settings.League);
            liveSearches[searchGuiItem] = liveSearch;
        }
        else
        {
            logger.LogWarning($"Live search for {searchGuiItem.Name} already exists");
        }
    }

    private async void LiveSearch_LiveSearchStopped(SearchGuiItem obj)
    {
        try
        {
            await mainWindowViewModel.ExecuteDisableSearchItemCommand(obj);
            mainWindowViewModel.Connected = false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception caught invoking {nameof(mainWindowViewModel.ExecuteDisableSearchItemCommand)}");
        }
    }

    public async Task StopSearchAsync(SearchGuiItem searchGuiItem)
    {
        if (liveSearches.TryRemove(searchGuiItem, out var liveSearch))
        {
            await liveSearch.StopAsync();
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PoeAuthenticator;
using PoeAuthenticator.Services;
using PoeLib.Common;
using PoeLib.Tools;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ItemSearch;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.Views;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Threading;

namespace PoeTradeMonitor.GUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger<MainWindowViewModel> logger;
    private readonly ICookieMonitorService cookieMonitorService;
    private readonly ICustomSearchManager customSearchManager;
    private readonly IPoePriceChecker poePriceChecker;
    private readonly PoeSettings poeSettings;
    private readonly ICurrencyCache currencyCache;
    private readonly ICurrencyPriceRetriever currencyPriceRetriever;
    private readonly IPoeHttpClient poeHttpClient;
    private readonly CookieContainer cookieContainer;
    private readonly IPoeCookieReader poeCookieReader;
    private readonly IStashDataUpdater stashDataUpdater;
    private readonly ITradeRequestScheduler tradeRequestScheduler;
    private readonly ICurrencyPriceCache currencyPriceCache;
    private readonly ITradeBotClient tradeBotClient;
    private readonly DispatcherTimer antiAfkTimer;
    private readonly DispatcherTimer connectionTimer;
    private readonly DispatcherTimer statsTimer;
    private readonly IBrowserService browserService;
    private readonly object syncLock = new object();

    public MainWindowViewModel(ITradeBotClient tbc, ICurrencyCache currency, ICurrencyPriceRetriever currencyPriceRetriever, IPoeHttpClient poeHttpClient, CookieContainer cookieContainer, IPoeCookieReader poeCookieReader,
                               IStashDataUpdater sdu, ITradeRequestScheduler trs, ICurrencyPriceCache currencyPrices, ILogger<MainWindowViewModel> logger, ICookieMonitorService cookieMonitorService,
                               ICustomSearchManager customSearchManager, IPoePriceChecker poePriceChecker, StatisticsManager statsManager, PoeSettings poeSettings, IBrowserService browserService)
    {
        this.poeSettings = poeSettings;
        this.logger = logger;
        this.cookieMonitorService = cookieMonitorService;
        currencyCache = currency;
        this.currencyPriceRetriever = currencyPriceRetriever;
        this.poeHttpClient = poeHttpClient;
        this.cookieContainer = cookieContainer;
        this.poeCookieReader = poeCookieReader;
        currencyPriceCache = currencyPrices;
        tradeBotClient = tbc;
        stashDataUpdater = sdu;
        stashDataUpdater.SetViewModel(this);
        tradeRequestScheduler = trs;
        tradeRequestScheduler.UpdateSettings(UnattendedModeEnabled, ServiceLocation);
        this.customSearchManager = customSearchManager;
        this.poePriceChecker = poePriceChecker;
        this.customSearchManager.SetViewModel(this);
        StatisticsManager = statsManager;
        this.browserService = browserService;

        connectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        antiAfkTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(550) };
        statsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };

        SearchList = new(poeSettings.SearchItems);
        SearchList.CollectionChanged += SearchList_CollectionChanged;
    }

    /// <summary>
    /// Initializes the MainWindowViewModel asynchronously.
    /// </summary>
    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing MainWindowViewModel");
        try
        {
            await Task.Run(async () =>
            {
                var poeCookies = await poeCookieReader.GetPoeCookiesAsync(CancellationToken.None);
                cookieContainer.UpdateCookies(poeCookies, logger);
            });

            cookieMonitorService.Start();

            do
            {
                var response = await poeHttpClient.GetLeagues();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    logger.LogError($"Failed to retrieve league list, Status: {response.StatusCode}");
                }

                var leagueList = await response.Content.ReadFromJsonAsync<List<League>>();
                if (leagueList != null)
                    LeagueList = new ObservableCollection<string>(leagueList.Select(league => league.Name));
            } while (LeagueList.Count == 0);

            if (string.IsNullOrEmpty(SelectedLeague))
            {
                if (LeagueList.Count > 8)
                {
                    SelectedLeague = LeagueList[9];
                }
                else if (LeagueList.Count > 4)
                {
                    SelectedLeague = LeagueList[4];
                }
                else
                {
                    SelectedLeague = LeagueList[0];
                }
            }
            else
            {
                if (!LeagueList.Contains(SelectedLeague))
                {
                    SelectedLeague = LeagueList[0];
                }
            }

            IgnoredAccounts = poeSettings.IgnoredAccounts;

            tradeRequestScheduler.AlertsEnabled = AlertsEnabled;

            await Task.WhenAll(ReloadDataCommand.ExecuteAsync(null), AutoReplyUpdatedCommand.ExecuteAsync(null));

            if (PriceLoggerEnabled)
            {
                poePriceChecker.Start();
                await poePriceChecker.UpdatePricesAndLogAsync();
            }

            foreach (var item in SearchList.Where(i => i.Enabled))
            {
                await customSearchManager.StartSearchAsync(item);
            }

            connectionTimer.Tick += (sender, args) => { LastDataReceived = LastDataReceived; };
            connectionTimer.Start();

            antiAfkTimer.Tick += async (sender, args) =>
            {
                try
                {
                    await tradeBotClient.AntiAFK(ServiceLocation.ToString());
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Caught exception sending antiafk: {ex}");
                }
            };
            AntiAFKEnabled = poeSettings.AntiAFKEnabled;

            statsTimer.Tick += async (sender, args) =>
            {
                try
                {
                    await ReloadDataCommand.ExecuteAsync(null);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Caught exception reloading data: {ex}");
                }
            };
            statsTimer.Start();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    public delegate void ScrollIntoViewDelegateSignature(StashGuiItem objEvent);
    public ScrollIntoViewDelegateSignature? ScrollIntoView { get; set; }
    public ObservableCollection<SearchGuiItem> SearchList { get; set; }
    public ObservableCollection<StashGuiItem> StashGuiItems { get; set; } = new();
    public StatisticsManager StatisticsManager { get; set; }
    public List<string> IgnoredAccounts { get; set; } = new();

    // Commands using RelayCommand
    [RelayCommand]
    private async Task AddSearch()
    {
        NewSearchDialogWindowViewModel vm = new NewSearchDialogWindowViewModel();
        NewSearchDialogWindow cw = new NewSearchDialogWindow(vm)
        {
            ShowInTaskbar = false,
            Owner = Application.Current.MainWindow
        };
        if (cw.ShowDialog() == true)
        {
            var item = vm.Item;
            if (!SearchList.Contains(item))
            {
                SearchList.Add(item);
                await customSearchManager.StartSearchAsync(item);
            }
        }
    }

    [RelayCommand]
    private async Task RemoveSearchItem(SearchGuiItem searchGuiItem)
    {
        poeSettings.SearchItems = SearchList.ToList();
        await customSearchManager.StopSearchAsync(searchGuiItem).ConfigureAwait(true);
        SearchList.Remove(searchGuiItem);
    }

    [RelayCommand]
    private async Task EnableSearchItem(SearchGuiItem searchGuiItem)
    {
        searchGuiItem.Enabled = true;
        poeSettings.SearchItems = SearchList.ToList();
        await customSearchManager.StartSearchAsync(searchGuiItem);
        OnPropertyChanged(nameof(EnabledSearchCount));
    }

    [RelayCommand]
    public async Task DisableSearchItem(SearchGuiItem searchGuiItem)
    {
        searchGuiItem.Enabled = false;
        poeSettings.SearchItems = SearchList.ToList();
        await customSearchManager.StopSearchAsync(searchGuiItem);
        OnPropertyChanged(nameof(EnabledSearchCount));
    }

    [RelayCommand]
    private void EditSearchItem(SearchGuiItem searchGuiItem)
    {
        NewSearchDialogWindowViewModel vm = new NewSearchDialogWindowViewModel(searchGuiItem);
        NewSearchDialogWindow cw = new NewSearchDialogWindow(vm)
        {
            ShowInTaskbar = false,
            Owner = Application.Current.MainWindow
        };

        if (cw.ShowDialog() == true)
        {
            var updatedItem = vm.Item;
            searchGuiItem.Enabled = updatedItem.Enabled;
            searchGuiItem.SearchID = updatedItem.SearchID;
            searchGuiItem.OfferPrice = updatedItem.OfferPrice;
        }

        poeSettings.SearchItems = SearchList.ToList();
    }

    /// <summary>
    /// Opens a search in the web browser using the configured browser settings.
    /// </summary>
    [RelayCommand]
    private async Task OpenSearchInBrowser(SearchGuiItem searchGuiItem)
    {
        ArgumentNullException.ThrowIfNull(searchGuiItem);
        
        if (string.IsNullOrWhiteSpace(searchGuiItem.SearchID))
        {
            logger.LogWarning("Cannot open search in browser: SearchID is null or empty");
            return;
        }

        var url = $"https://www.pathofexile.com/trade/search/{SelectedLeague}/{searchGuiItem.SearchID}";
        
        try
        {
            await browserService.OpenUrlAsync(url);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open search {SearchId} in browser", searchGuiItem.SearchID);
        }
    }

    [RelayCommand]
    private async Task ClearSearchItem()
    {
        foreach (var search in SearchList)
            await customSearchManager.StopSearchAsync(search);
        SearchList.Clear();
    }

    [RelayCommand]
    private async Task BuyStashGuiItem(StashGuiItem item)
    {
        try
        {
            await poeHttpClient.SendTradeWhisper(item.SearchID, item.ItemID, item.StackSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send item trade whisper");
        }
    }

    [RelayCommand]
    private void IgnoreAccount(StashGuiItem item)
    {
        if (!IgnoredAccounts.Contains(item.Account))
        {
            logger.LogInformation($"Ignoring account {item.Account}");
            poeSettings.IgnoredAccounts.Add(item.Account);
            IgnoredAccounts.Add(item.Account);
        }
    }

    [RelayCommand]
    private void ClearStashGuiItems()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            StashGuiItems.Clear();
        });
    }

    [RelayCommand]
    private async Task ExecuteCommand(StashGuiItem item)
    {
        logger.LogInformation($"Manually Buying: {item}");
        item.ExecuteEnabled = false;
        if (item.TradeRequest != null)
        {
            await tradeBotClient.QueueTrade(item.TradeRequest, ServiceLocation.ToString());
        }
    }

    [RelayCommand]
    private async Task AutoReplyUpdated()
    {
        try
        {
            await tradeBotClient.SetAutoReplyAsync(AutoreplyEnabled, ServiceLocation.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to set AutoReply: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ReloadData()
    {
        await currencyPriceRetriever.GetCurrencyPrices(SelectedLeague);
        await currencyCache.UpdateCurrenciesAsync(SelectedLeague);
        BaseCurrencyCount = currencyCache.GetCurrencyCount();
        if (currencyPriceCache.ContainsPrice(TradeCurrencyType.Divine))
        {
            DivineRateDecimal = currencyPriceCache.GetPrice(TradeCurrencyType.Divine).SellPrice;
        }
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    // Observable Properties with [ObservableProperty]
    [ObservableProperty]
    private SearchGuiItem? currentSearchItem;

    [ObservableProperty]
    private SearchGuiItem? selectedSearchItem;

    [ObservableProperty]
    private StashGuiItem? selectedStashGuiItem;

    [ObservableProperty]
    private DateTime lastDataReceived = DateTime.Now;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DivineRate))]
    [NotifyPropertyChangedFor(nameof(DivineCount))]
    private decimal divineRateDecimal;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DivineCount))]
    private int baseCurrencyCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Disconnected))]
    private bool connected;

    // Complex properties that need custom logic
    private ObservableCollection<string> leagueList = new ObservableCollection<string>();
    public ObservableCollection<string> LeagueList
    {
        get => leagueList ?? (leagueList = new ObservableCollection<string>());
        set
        {
            var currentLeague = poeSettings.League;
            leagueList.Clear();
            foreach (var item in value)
                leagueList.Add(item);
            OnPropertyChanged();

            if (!string.IsNullOrEmpty(currentLeague))
                SelectedLeague = currentLeague;
        }
    }

    public ServiceLocation ServiceLocation
    {
        get => poeSettings.ServiceLocation;
        set => SetProperty(poeSettings.ServiceLocation, value, poeSettings, (settings, val) => settings.ServiceLocation = val);
    }

    public bool AntiAFKEnabled
    {
        get => poeSettings.AntiAFKEnabled;
        set
        {
            if (SetProperty(poeSettings.AntiAFKEnabled, value, poeSettings, (settings, val) => settings.AntiAFKEnabled = val))
            {
                if (value)
                    antiAfkTimer.Start();
                else
                    antiAfkTimer.Stop();
            }
        }
    }

    public bool Running
    {
        get => poeSettings.Running;
        set => SetProperty(poeSettings.Running, value, poeSettings, (settings, val) => settings.Running = val);
    }

    public bool AutoreplyEnabled
    {
        get => poeSettings.AutoreplyEnabled;
        set => SetProperty(poeSettings.AutoreplyEnabled, value, poeSettings, (settings, val) => settings.AutoreplyEnabled = val);
    }

    public bool AlertsEnabled
    {
        get => poeSettings.AlertsEnabled;
        set
        {
            if (SetProperty(poeSettings.AlertsEnabled, value, poeSettings, (settings, val) => settings.AlertsEnabled = val))
            {
                tradeRequestScheduler.AlertsEnabled = value;
            }
        }
    }

    public bool UnattendedModeEnabled
    {
        get => poeSettings.UnattendedModeEnabled;
        set
        {
            if (SetProperty(poeSettings.UnattendedModeEnabled, value, poeSettings, (settings, val) => settings.UnattendedModeEnabled = val))
            {
                tradeRequestScheduler.UpdateSettings(value, ServiceLocation);
            }
        }
    }

    public bool PriceLoggerEnabled
    {
        get => poeSettings.PriceLoggerEnabled;
        set
        {
            if (SetProperty(poeSettings.PriceLoggerEnabled, value, poeSettings, (settings, val) => settings.PriceLoggerEnabled = val))
            {
                if (value)
                    poePriceChecker.Start();
                else
                    poePriceChecker.Stop();
            }
        }
    }

    public string SelectedLeague
    {
        get => poeSettings.League;
        set => SetProperty(poeSettings.League, value, poeSettings, (settings, val) => settings.League = val);
    }

    public int DivineRate => Convert.ToInt32(DivineRateDecimal);

    public decimal DivineCount => DivineRateDecimal == 0 ? 0 : Math.Round(BaseCurrencyCount / DivineRateDecimal, 1);

    public int EnabledSearchCount => SearchList?.Count(item => item.Enabled) ?? 0;

    public bool Disconnected => !Connected;

    public bool AddStashGuiItem(StashGuiItem item)
    {
        lock (syncLock)
        {
            if (StashGuiItems.Contains(item))
            {
                logger.LogWarning($"Failed to add item to GUI because it already contains that item: {item}");
                return false;
            }

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StashGuiItems.Add(item);
                    ScrollIntoView?.Invoke(item);
                });
                return true;
            }

            logger.LogWarning($"Failed to add item to GUI: {item}");
            return false;
        }
    }

    private void SearchList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        try
        {
            poeSettings.SearchItems = SearchList.ToList();
            OnPropertyChanged(nameof(EnabledSearchCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update SearchList");
        }
    }
}
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.Tools;
using PoeTradeMonitor.GUI.Commands;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Views;
using PoeLib.JSON;
using PoeTradeMonitor.GUI.ItemSearch;
using PoeLib.Settings;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Clients;
using PoeAuthenticator.Services;
using System.Net;
using PoeAuthenticator;
using System.Text.Json;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;

namespace PoeTradeMonitor.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand AddSearchCommand { get; set; }
    public ICommand EnableSearchItemCommand { get; set; }
    public ICommand DisableSearchItemCommand { get; set; }
    public ICommand EditSearchItemCommand { get; set; }
    public ICommand RemoveSearchItemCommand { get; set; }
    public ICommand OpenSearchInBrowserCommand { get; set; }
    public ICommand ClearSearchItemCommand { get; set; }
    public ICommand BuyStashGuiItemCommand { get; set; }
    public ICommand IgnoreAccountCommand { get; set; }
    public ICommand DealfinderEnabledChangedCommand { get; set; }
    public ICommand ClearStashGuiItemsCommand { get; set; }
    public ICommand ReloadDataCommand { get; set; }
    public ICommand ExitCommand { get; set; }
    public ICommand ExecuteCommand { get; set; }
    public ICommand AutoReplyUpdated { get; set; }
    public ObservableCollection<SearchGuiItem> SearchList { get; set; }
    public ObservableCollection<StashGuiItem> StashGuiItems { get; set; } = new();
    public StatisticsManager StatisticsManager { get; set; }
    public List<string> IgnoredAccounts { get; set; } = new();
    private readonly ILogger<MainWindowViewModel> logger;
    private readonly ICookieMonitorService cookieMonitorService;
    private readonly ICustomSearchManager customSearchManager;
    private readonly ICurrencyPriceRetriever currencyPriceRetriever;
    private readonly IPoePriceChecker poePriceChecker;
    private readonly ICurrencyCache currencyCache;
    private readonly ICurrencyPriceRetriever currencyDetailsRetriever;
    private readonly IPoeHttpClient poeHttpClient;
    private readonly CookieContainer cookieContainer;
    private readonly IPoeCookieReader poeCookieReader;
    private readonly IStashDataUpdater stashDataUpdater;
    private readonly ITradeRequestScheduler tradeRequestScheduler;
    private readonly ICurrencyPriceCache currencyPriceCache;
    private readonly IPoeProxyClient poeProxyClient;
    private readonly ITradeBotClient tradeBotClient;
    private readonly ItemDealfinder itemDealfinder;
    private readonly DispatcherTimer antiAfkTimer;
    private readonly DispatcherTimer connectionTimer;
    private readonly DispatcherTimer statsTimer;
    private readonly ISettingsManager settingsManager;
    private readonly object syncLock = new object();

    public MainWindowViewModel(IPoeProxyClient ppc, ITradeBotClient tbc, ICurrencyCache currency, ICurrencyPriceRetriever currencyRetriever, IPoeHttpClient poeHttpClient, CookieContainer cookieContainer, IPoeCookieReader poeCookieReader,
                               IStashDataUpdater sdu, ITradeRequestScheduler trs, ICurrencyPriceCache currencyPrices, ItemDealfinder dealfinder, ISettingsManager settings, ILogger<MainWindowViewModel> logger, ICookieMonitorService cookieMonitorService,
                               ICustomSearchManager customSearchManager, ICurrencyPriceRetriever currencyPriceRetriever, IPoePriceChecker poePriceChecker, StatisticsManager statsManager)
    {
        this.logger = logger;
        this.cookieMonitorService = cookieMonitorService;
        currencyCache = currency;
        currencyDetailsRetriever = currencyRetriever;
        this.poeHttpClient = poeHttpClient;
        this.cookieContainer = cookieContainer;
        this.poeCookieReader = poeCookieReader;
        currencyPriceCache = currencyPrices;
        itemDealfinder = dealfinder;
        poeProxyClient = ppc;
        tradeBotClient = tbc;
        settingsManager = settings;
        stashDataUpdater = sdu;
        stashDataUpdater.SetViewModel(this);
        tradeRequestScheduler = trs;
        tradeRequestScheduler.UpdateSettings(UnattendedModeEnabled, TradeConfirmationEnabled, ServiceLocation);
        this.customSearchManager = customSearchManager;
        this.currencyPriceRetriever = currencyPriceRetriever;
        this.poePriceChecker = poePriceChecker;
        this.customSearchManager.SetViewModel(this);
        StatisticsManager = statsManager;
        
        ExitCommand = new DelegateCommand(ExecuteExitCommand);

        EnableSearchItemCommand = new AsyncCommand<SearchGuiItem>(ExecuteEnableSearchItemCommand);
        DisableSearchItemCommand = new AsyncCommand<SearchGuiItem>(ExecuteDisableSearchItemCommand);
        AddSearchCommand = new AsyncCommand(ExecuteAddSearchCommand);
        RemoveSearchItemCommand = new AsyncCommand<SearchGuiItem>(ExecuteRemoveSearchItemCommand);
        EditSearchItemCommand = new AsyncCommand<SearchGuiItem>(ExecuteEditSearchItemCommand);
        OpenSearchInBrowserCommand = new DelegateCommand<SearchGuiItem>(ExecuteOpenSearchInBrowserCommand);
        ClearSearchItemCommand = new AsyncCommand(ExecuteClearSearchItemCommand);
        ReloadDataCommand = new AsyncCommand(ExecuteReloadDataCommand);

        DealfinderEnabledChangedCommand = new AsyncCommand(ExecuteDealfinderEnabledChangedCommand);
        BuyStashGuiItemCommand = new AsyncCommand<StashGuiItem>(ExecuteBuyStashGuiItemCommand);
        IgnoreAccountCommand = new AsyncCommand<StashGuiItem>(ExecuteIgnoreAccountCommand);
        ClearStashGuiItemsCommand = new DelegateCommand(ExecuteClearStashGuiItemsCommand);
        ExecuteCommand = new AsyncCommand<StashGuiItem>(ExecuteCommandExecute, item => true);
        AutoReplyUpdated = new AsyncCommand(ExecuteAutoReplyUpdated);

        connectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        antiAfkTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(550) };
        statsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };

        SearchList = new(settingsManager.Settings.SearchItems);
        SearchList.CollectionChanged += SearchList_CollectionChanged;
    }

    public delegate void ScrollIntoViewDelegateSignature(StashGuiItem objEvent);
    public ScrollIntoViewDelegateSignature ScrollIntoView { get; set; }

    public async Task InitializeAsync()
    {
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
                var leagueJson = await response.Content.ReadAsStringAsync();
                var leagueList = JsonSerializer.Deserialize<List<League>>(leagueJson);
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

            await settingsManager.SaveAsync();

            IgnoredAccounts = settingsManager.Settings.IgnoredAccounts;

            itemDealfinder.League = SelectedLeague;
            itemDealfinder.Strictness = Strictness == 0 ? Strictness.Chaos7 : Strictness;
            itemDealfinder.UndercutNotificationsEnabled = UndercutNotificationEnabled;
            itemDealfinder.HighMarginMode = HighMarginMode;
            tradeRequestScheduler.AlertsEnabled = AlertsEnabled;

            await Task.WhenAll(ExecuteReloadDataCommand(), ExecuteAutoReplyUpdated());

            if (PriceLoggerEnabled)
            {
                poePriceChecker.Start();
                await poePriceChecker.UpdatePricesAndLogAsync();
            }

            foreach (var item in SearchList.Where(i => i.Enabled))
            {
                await customSearchManager.StartSearchAsync(item);
            }

            if (DealfinderEnabled)
                itemDealfinder.Start();

            connectionTimer.Tick += (sender, args) => { LastDataReceived = LastDataReceived; };
            connectionTimer.Start();

            antiAfkTimer.Tick += async (sender, args) => 
            { 
                try
                {
                    await poeProxyClient.AntiAFK(ServiceLocation.ToString());
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Caught exception sending antiafk: {ex}");
                }
            };
            AntiAFKEnabled = settingsManager.Settings.AntiAFKEnabled;

            statsTimer.Tick += async (sender, args) => 
            { 
                try
                {
                    await ExecuteReloadDataCommand();
                }
                catch(Exception ex)
                {
                    logger.LogWarning($"Caught exception reloading data: {ex}");
                }
            };
            statsTimer.Start();
        }
        catch(Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    private SearchGuiItem currentSearchItem;
    public SearchGuiItem CurrentSearchItem
    {
        get => currentSearchItem;
        set
        {
            currentSearchItem = value;
            RaisePropertyChanged();
        }
    }

    private SearchGuiItem selectedSearchItem;
    public SearchGuiItem SelectedSearchItem
    {
        get => selectedSearchItem;
        set
        {
            selectedSearchItem = value;
            RaisePropertyChanged();
        }
    }

    private StashGuiItem selectedStashGuiItem;
    public StashGuiItem SelectedStashGuiItem
    {
        get => selectedStashGuiItem;
        set
        {
            selectedStashGuiItem = value;
            RaisePropertyChanged();
        }
    }

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

    public void ClearStashGuiItems()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            StashGuiItems.Clear();
        });
    }

    private async Task ExecuteAddSearchCommand()
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

    private async Task ExecuteRemoveSearchItemCommand(SearchGuiItem searchGuiItem)
    {
        settingsManager.Settings.SearchItems = SearchList.ToList();
        await settingsManager.SaveAsync().ConfigureAwait(true);
        await customSearchManager.StopSearchAsync(searchGuiItem).ConfigureAwait(true);
        SearchList.Remove(searchGuiItem);
    }

    private async Task ExecuteEnableSearchItemCommand(SearchGuiItem searchGuiItem)
    {
        searchGuiItem.Enabled = true;

        settingsManager.Settings.SearchItems = SearchList.ToList();
        await settingsManager.SaveAsync();

        await customSearchManager.StartSearchAsync(searchGuiItem);
        RaisePropertyChanged(nameof(EnabledSearchCount));
    }

    public async Task ExecuteDisableSearchItemCommand(SearchGuiItem searchGuiItem)
    {
        searchGuiItem.Enabled = false;

        settingsManager.Settings.SearchItems = SearchList.ToList();
        await settingsManager.SaveAsync();

        await customSearchManager.StopSearchAsync(searchGuiItem);
        RaisePropertyChanged(nameof(EnabledSearchCount));
    }

    public void ExecuteOpenSearchInBrowserCommand(SearchGuiItem searchGuiItem)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
            Arguments = $"https://www.pathofexile.com/trade/search/Phrecia/{searchGuiItem.SearchID} --profile-directory=\"Profile 1\"",
            UseShellExecute = true
        };
        Process.Start(startInfo);
    }

    private async Task ExecuteEditSearchItemCommand(SearchGuiItem searchGuiItem)
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
            searchGuiItem.IsCurrency = updatedItem.IsCurrency;
            searchGuiItem.CurrencyType = updatedItem.CurrencyType;
            searchGuiItem.MinChaos = updatedItem.MinChaos;
            searchGuiItem.BuyThreshold = updatedItem.BuyThreshold;
            searchGuiItem.MinStock = updatedItem.MinStock;
            searchGuiItem.OfferPrice = updatedItem.OfferPrice;
        }

        settingsManager.Settings.SearchItems = SearchList.ToList();
        await settingsManager.SaveAsync();
    }

    private async Task ExecuteClearSearchItemCommand()
    {
        foreach (var search in SearchList)
            await customSearchManager.StopSearchAsync(search);
        SearchList.Clear();
    }

    private async Task ExecuteBuyStashGuiItemCommand(StashGuiItem item)
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

    private async Task ExecuteIgnoreAccountCommand(StashGuiItem item)
    {
        if (!IgnoredAccounts.Contains(item.Account))
        {
            logger.LogInformation($"Ignoring account {item.Account}");
            settingsManager.Settings.IgnoredAccounts.Add(item.Account);
            await settingsManager.SaveAsync();
            IgnoredAccounts.Add(item.Account);
        }
    }

    private async Task ExecuteDealfinderEnabledChangedCommand()
    {
        if (DealfinderEnabled)
        {
            itemDealfinder.Start();
        }
        else
        {
            await itemDealfinder.StopAsync();
        }
    }

    private void ExecuteClearStashGuiItemsCommand()
    {
        ClearStashGuiItems();
    }

    private async Task ExecuteCommandExecute(StashGuiItem item)
    {
        logger.LogInformation($"Manually Buying: {item}");
        item.ExecuteEnabled = false;
        if (item.TradeRequest != null)
        {
            await tradeBotClient.QueueTrade(item.TradeRequest, ServiceLocation.ToString());
        }
    }

    private async Task ExecuteAutoReplyUpdated()
    {
        try
        { 
            await poeProxyClient.SetAutoReplyAsync(AutoreplyEnabled, ServiceLocation.ToString());
        }
        catch(Exception ex)
        {
            logger.LogError($"Failed to set AutoReply: {ex.Message}");
        }
    }

    public async Task ExecuteReloadDataCommand()
    {
        await currencyDetailsRetriever.GetCurrencyPrices(SelectedLeague);
        await currencyCache.UpdateCurrencies(SelectedLeague);
        ChaosCount = currencyCache.GetChaosCount();
        if (currencyPriceCache.ContainsPrice(CurrencyType.divine))
        {
            DivineRateDecimal = currencyPriceCache.GetPrice(CurrencyType.divine).SellPrice;
        }
    }

    private void ExecuteExitCommand()
    {
        Application.Current.Shutdown();
    }

    private ObservableCollection<string> leagueList;
    public ObservableCollection<string> LeagueList
    {
        get => leagueList ?? (leagueList = new ObservableCollection<string>());
        set
        {
            var currentLeague = settingsManager.Settings.League;
            leagueList.Clear();
            foreach(var item in value)
                leagueList.Add(item);
            RaisePropertyChanged();

            if(!string.IsNullOrEmpty(currentLeague))
                SelectedLeague = currentLeague;
        }
    }

    private async void SearchList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        try
        {
            settingsManager.Settings.SearchItems = SearchList.ToList();
            await settingsManager.SaveAsync();
            RaisePropertyChanged(nameof(EnabledSearchCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update SearchList");
        }
    }

    public Strictness Strictness
    {
        get => settingsManager.Settings.Strictness;
        set
        {
            settingsManager.Settings.Strictness = value;
            settingsManager.Save();
            itemDealfinder.Strictness = value;
            RaisePropertyChanged();
        }
    }

    public ServiceLocation ServiceLocation
    {
        get => settingsManager.Settings.ServiceLocation;
        set
        {
            settingsManager.Settings.ServiceLocation = value;
            settingsManager.Save();
            RaisePropertyChanged();
        }
    }

    public bool TradeConfirmationEnabled
    {
        get => settingsManager.Settings.TradeConfirmationEnabled;
        set
        {
            if (settingsManager.Settings.TradeConfirmationEnabled != value)
            {
                settingsManager.Settings.TradeConfirmationEnabled = value;
                settingsManager.Save();
                tradeRequestScheduler.UpdateSettings(UnattendedModeEnabled, value, ServiceLocation);
                RaisePropertyChanged();
            }
        }
    }
    
    public bool AntiAFKEnabled
    {
        get => settingsManager.Settings.AntiAFKEnabled;
        set
        {
            settingsManager.Settings.AntiAFKEnabled = value;
            settingsManager.Save();
            if(value)
                antiAfkTimer.Start();
            else
                antiAfkTimer.Stop();
            RaisePropertyChanged();
        }
    }
    
    public bool DealfinderEnabled
    {
        get => settingsManager.Settings.DealfinderEnabled;
        set
        {
            settingsManager.Settings.DealfinderEnabled = value;
            settingsManager.Save();
            RaisePropertyChanged();
        }
    }
    
    public bool Running
    {
        get => settingsManager.Settings.Running;
        set
        {
            settingsManager.Settings.Running = value;
            settingsManager.Save();
            RaisePropertyChanged();
        }
    }

    public bool AutoreplyEnabled
    {
        get => settingsManager.Settings.AutoreplyEnabled;
        set
        {
            settingsManager.Settings.AutoreplyEnabled = value;
            settingsManager.Save();
            RaisePropertyChanged();
        }
    }
    
    public bool AlertsEnabled
    {
        get => settingsManager.Settings.AlertsEnabled;
        set
        {
            settingsManager.Settings.AlertsEnabled = value;
            settingsManager.Save();
            RaisePropertyChanged();
            tradeRequestScheduler.AlertsEnabled = value;
        }
    }

    public bool UnattendedModeEnabled
    {
        get => settingsManager.Settings.UnattendedModeEnabled;
        set
        {
            settingsManager.Settings.UnattendedModeEnabled = value;
            settingsManager.Save();
            tradeRequestScheduler.UpdateSettings(value, TradeConfirmationEnabled, ServiceLocation);
            RaisePropertyChanged();
        }
    }

    public bool PriceLoggerEnabled
    {
        get => settingsManager.Settings.PriceLoggerEnabled;
        set
        {
            settingsManager.Settings.PriceLoggerEnabled = value;
            settingsManager.Save();
            if (value)
                poePriceChecker.Start();
            else
                poePriceChecker.Stop();
            RaisePropertyChanged();
        }
    }

    public bool UndercutNotificationEnabled
    {
        get => settingsManager.Settings.UndercutNotificationEnabled;
        set
        {
            settingsManager.Settings.UndercutNotificationEnabled = value;
            settingsManager.Save();
            itemDealfinder.UndercutNotificationsEnabled = value;
            RaisePropertyChanged();
        }
    }

    private DateTime lastDataReceived = DateTime.Now;
    public DateTime LastDataReceived
    {
        get => lastDataReceived;
        set
        {
            lastDataReceived = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ConnectedDuration));
        }
    }
    
    public string ConnectedDuration
    {
        get
        {
            var timeConnected = DateTime.Now - LastDataReceived;
            return Connected ? timeConnected.ToString("m'm's's'") : "0m0s";
        }
    }

    private decimal divineRateDecimal;
    public decimal DivineRateDecimal
    {
        get => divineRateDecimal;
        set
        {
            divineRateDecimal = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(DivineRate));
            RaisePropertyChanged(nameof(DivineCount));
        }
    }

    public int DivineRate
    {
        get => Convert.ToInt32(divineRateDecimal);
    }

    private int chaosCount;
    public int ChaosCount
    {
        get => chaosCount;
        set
        {
            chaosCount = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(DivineCount));
        }
    }
    
    public decimal DivineCount => DivineRateDecimal == 0 ? 0 : Math.Round(chaosCount/DivineRateDecimal, 1);
    
    public string SelectedLeague
    {
        get => settingsManager.Settings.League;
        set
        {
            settingsManager.Settings.League = value;
            settingsManager.Save();
            itemDealfinder.League = value;
            RaisePropertyChanged();
        }
    }

    public int EnabledSearchCount
    {
        get => SearchList?.Count(item => item.Enabled) ?? 0;
    }

    private bool connected;
    public bool Connected
    {
        get => connected;
        set
        {
            connected = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Disconnected));
        }
    }

    public bool Disconnected
    {
        get => !connected;
    }

    public bool HighMarginMode
    {
        get => settingsManager.Settings.HighMarginMode;
        set
        {
            settingsManager.Settings.HighMarginMode = value;
            settingsManager.Save();
            itemDealfinder.HighMarginMode = value;
            RaisePropertyChanged();
        }
    }
}

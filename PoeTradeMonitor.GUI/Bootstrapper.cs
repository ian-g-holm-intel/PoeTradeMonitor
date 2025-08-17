using Microsoft.Extensions.DependencyInjection;
using PoeAuthenticator;
using PoeLib.Common;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.PriceFetchers.PoeNinja;
using PoeLib.Proto;
using PoeLib.Tools;
using PoeLib.Tools.Notification;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ItemSearch;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.ViewModels;
using PoeTradeMonitor.GUI.Views;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace PoeTradeMonitor.GUI
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddPoeTradeMonitorGui(this IServiceCollection services)
        {
            services.AddSingleton<CallbackService>();
            services.AddSingleton<ITradeRequestScheduler, TradeRequestScheduler>();

            services.AddSingleton<StatisticsManager>();
            services.AddSingleton<ItemPriceCache>();
            services.AddSingleton<ICustomSearchManager, CustomSearchManager>();
            services.AddSingleton<ISearchCriteriaMatcher, SearchCriteriaMatcher>();
            services.AddSingleton<IStashDataUpdater, StashDataUpdater>();
            services.AddSingleton<ILiveSearchResultProcessor, LiveSearchResultProcessor>();
            services.AddSingleton<IPoePriceChecker, PoePriceChecker>();
            services.AddSingleton<INotificationClient, PushoverNotificationClient>();
            services.AddSingleton<IPoeItemSearchRequestCache, PoeItemSearchRequestCache>();
            services.AddSingleton<IPoeItemSearch, PoeItemSearch>();
            services.AddSingleton(LoadPoeSettings());
            services.AddSingleton<SettingsManager>();
            services.AddTransient<IPoeHttpClient, PoeHttpClient>();
            services.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
            services.AddSingleton<IChatMessageCache, ChatMessageCache>();
            services.AddSingleton<ICurrencyCache, CurrencyCache>();
            services.AddSingleton<IPriceFetcher, PoeNinjaWrapper>();
            services.AddSingleton<IPriceFetcherWrapper, PriceFetcherWrapper>();
            services.AddSingleton<IMessageParser, MessageParser>();
            services.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();
            services.AddSingleton<ICurrencyPriceRetriever, CurrencyPriceRetriever>();
            services.AddSingleton<IStashCurrencyRetriever, StashCurrencyRetriever>();
            services.AddSingleton<IStashItemsRetriever, StashItemsRetriever>();
            services.AddSingleton<ILiveSearchItemCache, LiveSearchItemCache>();
            services.AddSingleton<IBrowserService, BrowserService>();

            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowViewModel>();

            services.AddGrpc();
            services.AddPoeAuthenticator();
            services.AddHttpClient();

            // Register the main HttpClient for POE API requests
            services.AddHttpClient("PoeApi", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.BaseAddress = new Uri("https://www.pathofexile.com");
            })
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<HttpMessageHandler>())
            .AddHttpMessageHandler<LoginDelegateHandler>()
            .AddHttpMessageHandler<PoeRateLimitHandler>()
            .AddHttpMessageHandler<ClearanceHandler>();

            services.AddSingleton<IPartyManagerClient, PartyManagerClient>();
            services.AddSingleton<ITradeBotClient, TradeBotClient>();
            AddGrpcClient<PartyManager.PartyManagerClient>(services);
            AddGrpcClient<TradeBot.TradeBotClient>(services);

            return services;
        }

        private static PoeSettings LoadPoeSettings()
        {
            var settingsFilePath = Path.Combine(Constants.DataDirectory, "settings.json");
            if (!File.Exists(settingsFilePath))
            {
                return new PoeSettings();
            }

            try
            {
                var json = File.ReadAllText(settingsFilePath);
                return JsonSerializer.Deserialize<PoeSettings>(json, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    WriteIndented = true
                }) ?? new PoeSettings();
            }
            catch
            {
                return new PoeSettings();
            }
        }

        private static void AddGrpcClient<T>(IServiceCollection services) where T : class
        {
            services.AddGrpcClient<T>($"Local{typeof(T).Name}", o => { o.Address = new Uri("http://127.0.0.1:5002"); })
                .ConfigureChannel(o =>
                {
                    o.HttpHandler = new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
                        EnableMultipleHttp2Connections = true
                    };
                });
            services.AddGrpcClient<T>($"Remote{typeof(T).Name}", o => { o.Address = new Uri("http://10.2.1.189:5002"); })
                .ConfigureChannel(o =>
                {
                    o.HttpHandler = new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
                        EnableMultipleHttp2Connections = true
                    };
                });
        }
    }
}

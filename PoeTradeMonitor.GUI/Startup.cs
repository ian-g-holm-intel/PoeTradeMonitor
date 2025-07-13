using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoeAuthenticator;
using PoeLib;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.PriceFetchers.PoeNinja;
using PoeLib.Proxies;
using PoeLib.Settings;
using PoeLib.Tools;
using PoeLib.Tools.Notification;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ItemSearch;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.ViewModels;
using PoeTradeMonitor.GUI.Views;
using PoeTradeMonitorProto;
using System.Net.Http;

namespace PoeTradeMonitor.GUI;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().Get<PoeSettings>());
        services.AddSingleton<CallbackService>();
        services.AddSingleton<ITradeRequestScheduler, TradeRequestScheduler>();

        services.AddSingleton<StatisticsManager>();
        services.AddSingleton<ItemPriceCache>();
        services.AddSingleton<ItemDealfinder>();
        services.AddSingleton<ICustomSearchManager, CustomSearchManager>();
        services.AddSingleton<ISearchCriteriaMatcher, SearchCriteriaMatcher>();
        services.AddSingleton<IStashDataUpdater, StashDataUpdater>();
        services.AddSingleton<ILiveSearchResultProcessor, LiveSearchResultProcessor>();
        services.AddSingleton<IPoePriceChecker, PoePriceChecker>();
        services.AddSingleton<INotificationClient, PushoverNotificationClient>();
        services.AddSingleton<IPoeItemLiveSearchManager, PoeItemLiveSearchManager>();
        services.AddSingleton<IPoeItemSearchRequestCache, PoeItemSearchRequestCache>();
        services.AddSingleton<IPoePriceCache, PoePriceCache>();
        services.AddSingleton<IPoeItemSearch, PoeItemSearch>();
        services.AddSingleton<ISettingsManager, SettingsManager>();
        services.AddSingleton<IProxyRetriever, WebshareProxyRetriever>();
        services.AddSingleton<IProxySpeedTester, ProxySpeedTester>();
        services.AddTransient<IPoeHttpClient, PoeHttpClient>();
        services.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
        services.AddSingleton<IChatMessageCache, ChatMessageCache>();
        services.AddSingleton<ICurrencyCache, CurrencyCache>();
        services.AddSingleton<IPriceFetcher, PoeNinjaWrapper>();
        services.AddSingleton<IPriceFetcher, PoeNinjaWrapper>();
        services.AddSingleton<IPriceFetcherWrapper, PriceFetcherWrapper>();
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();
        services.AddSingleton<ICurrencyPriceRetriever, CurrencyPriceRetriever>();
        services.AddSingleton<IStashCurrencyRetriever, StashCurrencyRetriever>();
        services.AddSingleton<IStashItemsRetriever, StashItemsRetriever>();
        services.AddSingleton<ICurrencyInfoParser, CurrencyInfoParser>();
        services.AddSingleton<IPriceParser, PriceParser>();
        services.AddSingleton<ILiveSearchItemCache, LiveSearchItemCache>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();

        services.AddGrpc();
        services.AddPoeAuthenticator();
        services.AddHttpClient();

        services.AddSingleton<IPartyManagerClient, PartyManagerClient>();
        services.AddSingleton<IPoeProxyClient, PoeProxyClient>();
        services.AddSingleton<ITradeBotClient, TradeBotClient>();
        AddGrpcClient<PartyManager.PartyManagerClient>(services);
        AddGrpcClient<PoeProxy.PoeProxyClient>(services);
        AddGrpcClient<TradeBot.TradeBotClient>(services);
    }

    private void AddGrpcClient<T>(IServiceCollection services) 
        where T : class
    {
        services.AddGrpcClient<T>($"Local{typeof(T).Name}", o => { o.Address = new Uri("http://127.0.0.1:5002"); })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
            });
        services.AddGrpcClient<T>($"Remote{typeof(T).Name}", o => { o.Address = new Uri("http://10.2.1.189:5002"); })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
   
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<Services.CallbackService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}

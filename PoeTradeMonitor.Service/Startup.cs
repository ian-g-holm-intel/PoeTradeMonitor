using InputSimulatorStandard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoeLib;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.PriceFetchers.PoeNinja;
using PoeLib.PriceFetchers.PoeWatch;
using PoeLib.Tools;
using PoeLib.Tools.Notification;
using PoeHudWrapper;
using PoeTradeMonitor.Service.Clients;
using PoeTradeMonitor.Service.Interfaces;
using PoeTradeMonitor.Service.Services;
using PoeTradeMonitor.Services.Interfaces;
using System;
using TradeBotLib;
using PoeHudWrapper.MemoryObjects;
using System.Net.Http;
using System.Threading;
using PoeLib.Settings;
using Microsoft.Extensions.Configuration;
using PoeTradeMonitorProto;
using TradeBot = PoeTradeMonitor.Service.Services.TradeBot;

namespace PoeTradeMonitor.Service;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().Get<PoeSettings>());
        services.AddSingleton<TradeBotService>();
        services.AddSingleton<PartyManagerService>();
        services.AddSingleton<PoeProxyService>();
        services.AddSingleton<ITradeBot, TradeBot>();
        services.AddSingleton<ICallbackClient, CallbackClient>();
        services.AddSingleton<ITradeCommands, TradeCommands>();
        services.AddSingleton<ITradeExecutorService, TradeExecutorService>();
        services.AddSingleton<INotificationClient, PushoverNotificationClient>();
        services.AddSingleton<IPoeHudWrapper, PoeHudWrapper.PoeHudWrapper>();
        services.AddSingleton<IGameWrapper, GameWrapper>();
        services.AddSingleton<IMemoryProvider, MemoryProvider>();
        services.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();
        services.AddSingleton<IChatMessageCache, ChatMessageCache>();
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IPriceValidator, PriceValidator>();
        services.AddSingleton<IInputSimulator>(new InputSimulator());
        services.AddSingleton<IPriceFetcherWrapper, PriceFetcherWrapper>();
        services.AddSingleton<IPriceFetcher, PoeNinjaWrapper>();
        services.AddSingleton<IPriceFetcher, PoeWatchWrapper>();
        services.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
        services.AddSingleton<ITradeBotStateMachine, TradeBotStateMachine>();
        services.AddSingleton<ISettingsManager, SettingsManager>();

        services.AddHostedService(sp => sp.GetRequiredService<IPoeChatWatcher>());
        services.AddHostedService(sp => sp.GetRequiredService<ITradeBot>());

        services.AddHttpClient();
        services.AddGrpc();

        AddGrpcClient<Callback.CallbackClient>(services);
    }

    private void AddGrpcClient<T>(IServiceCollection services)
        where T : class
    {
        services.AddGrpcClient<T>((sp, o) => { o.Address = new Uri($"http://{sp.GetRequiredService<PoeSettings>().GuiAddress}:5001"); })
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
            endpoints.MapGrpcService<TradeBotService>();
            endpoints.MapGrpcService<PoeProxyService>();
            endpoints.MapGrpcService<PartyManagerService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}

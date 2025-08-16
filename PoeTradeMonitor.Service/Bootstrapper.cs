using InputSimulatorStandard;
using PoeLib.Parsers;
using PoeLib.Tools;
using PoeLib.Tools.Notification;
using PoeHudWrapper;
using PoeTradeMonitor.Service.Clients;
using PoeTradeMonitor.Service.Interfaces;
using PoeTradeMonitor.Service.Services;
using PoeTradeMonitor.Services.Interfaces;
using Serilog;
using PoeLib.Common;

namespace PoeTradeMonitor.Service;

public static class Bootstrapper
{
    public static IServiceCollection AddPoeTradeMonitorService(this IServiceCollection services)
    {
        services.AddSingleton<TradeBotService>();
        services.AddSingleton<PartyManagerService>();
        services.AddSingleton<ITradeBot, TradeBot>();
        services.AddSingleton<ICallbackClient, CallbackClient>();
        services.AddSingleton<ITradeCommands, TradeCommands>();
        services.AddSingleton<IStashCurrencyCache, StashCurrencyCache>();
        services.AddSingleton<ITradeExecutorService, TradeExecutorService>();
        services.AddSingleton<INotificationClient, PushoverNotificationClient>();
        services.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();
        services.AddSingleton<IChatMessageCache, ChatMessageCache>();
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IPriceValidator, PriceValidator>();
        services.AddSingleton<IInputSimulator>(new InputSimulator());
        services.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
        services.AddSingleton<ITradeBotStateMachine, TradeBotStateMachine>();
        services.AddSingleton<ITradeCommands, TradeCommands>();
        services.AddSingleton<IInputSimulator>(sp => new InputSimulator());
        services.AddSingleton<IPriceValidator, PriceValidator>();
        services.AddPoeHudWrapper();

        services.AddHostedService(sp => sp.GetRequiredService<IPoeChatWatcher>());
        services.AddHostedService(sp => sp.GetRequiredService<ITradeBot>());

        services.AddHttpClient();
        services.AddGrpc();
        services.AddSerilog();

        AddGrpcClient<PoeLib.Proto.Callback.CallbackClient>(services);

        return services;
    }

    private static void AddGrpcClient<T>(IServiceCollection services)
        where T : class
    {
        services.AddGrpcClient<T>((sp, o) => { o.Address = new Uri($"http://{sp.GetRequiredService<IConfiguration>().GetValue("GuiAddress", "127.0.0.1")}:5001"); })
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
}

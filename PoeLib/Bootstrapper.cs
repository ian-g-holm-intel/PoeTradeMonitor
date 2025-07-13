using Microsoft.Extensions.DependencyInjection;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.PriceFetchers.PoeNinja;
using PoeLib.PriceFetchers.PoeWatch;
using PoeLib.Tools;
using PoeLib.Tools.Notification;

namespace PoeLib;

public class Bootstrapper
{
    public void RegisterServices(IServiceCollection container)
    {
        container.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
        container.AddSingleton<IChatMessageCache, ChatMessageCache>();
        container.AddSingleton<INotificationClient, PushoverNotificationClient>();
        container.AddSingleton<IPriceFetcher, PoeNinjaWrapper>();
        container.AddSingleton<IPriceFetcher, PoeWatchWrapper>();
        container.AddSingleton<IPriceFetcherWrapper, PriceFetcherWrapper>();
        container.AddSingleton<IMessageParser, MessageParser>();
        container.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();
        container.AddSingleton<ICurrencyInfoParser, CurrencyInfoParser>();
        container.AddSingleton<IPriceParser, PriceParser>();
    }
}

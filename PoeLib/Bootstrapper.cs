using Microsoft.Extensions.DependencyInjection;
using PoeLib.Common;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.PriceFetchers.Poe2Scout;
using PoeLib.Tools;
using PoeLib.Tools.Notification;

namespace PoeLib;

public static class Bootstrapper
{
    public static IServiceCollection AddPoeLib(this IServiceCollection container)
    {
        container.AddSingleton<ICurrencyPriceCache, CurrencyPriceCache>();
        container.AddSingleton<IChatMessageCache, ChatMessageCache>();
        container.AddSingleton<INotificationClient, PushoverNotificationClient>();
        container.AddSingleton<IPriceFetcher, Poe2ScoutWrapper>();
        container.AddSingleton<IPriceFetcherWrapper, PriceFetcherWrapper>();
        container.AddSingleton<IMessageParser, MessageParser>();
        container.AddSingleton<IPoeChatWatcher, PoeChatWatcher>();

        return container;
    }
}

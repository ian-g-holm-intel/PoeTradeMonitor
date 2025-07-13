using System.Collections.Concurrent;
using PoeLib.JSON;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.Services;

public class PoePriceCache : IPoePriceCache
{
    private readonly ConcurrentDictionary<string, Item[]> itemPriceDictionary = new ConcurrentDictionary<string, Item[]>();

    public bool UpdateItemPrices(string name, Item[] items)
    {
        Item[] existingPrices = null;
        if (itemPriceDictionary.ContainsKey(name))
            existingPrices = itemPriceDictionary[name];

        itemPriceDictionary[name] = items;

        var pricesChanged = existingPrices != null && items.Any(i => !existingPrices.Contains(i));
        return pricesChanged;
    }
}

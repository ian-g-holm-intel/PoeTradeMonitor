using PoeLib.JSON;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoePriceCache
{
    bool UpdateItemPrices(string name, Item[] items);
}

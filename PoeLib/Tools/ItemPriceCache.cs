using System.Collections.Concurrent;
using System.Collections.Generic;
using PoeLib.GuiDataClasses;

namespace PoeLib.Tools;

public class ItemPriceCache
{
    private ConcurrentBag<SearchGuiItem> itemPrices = new ConcurrentBag<SearchGuiItem>();

    public void SetItemPrices(IEnumerable<SearchGuiItem> items)
    {
        ClearItemPrices();
        foreach (var item in items)
            itemPrices.Add(item);
    }

    public SearchGuiItem[] GetAllItemPrices()
    {
        return itemPrices.ToArray();
    }

    public void ClearItemPrices()
    {
        itemPrices = new ConcurrentBag<SearchGuiItem>();
    }
}

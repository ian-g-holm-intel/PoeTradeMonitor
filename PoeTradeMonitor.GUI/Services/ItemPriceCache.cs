using PoeTradeMonitor.GUI.Models;
using System.Collections.Concurrent;

namespace PoeTradeMonitor.GUI.Services;

/// <summary>
/// Cache for storing and managing item price information for search GUI items.
/// Provides thread-safe operations for price data management.
/// </summary>
public class ItemPriceCache
{
    private ConcurrentBag<SearchGuiItem> itemPrices = new ConcurrentBag<SearchGuiItem>();

    /// <summary>
    /// Sets the item prices by clearing existing data and adding new items.
    /// </summary>
    /// <param name="items">The collection of search GUI items with price information.</param>
    public void SetItemPrices(IEnumerable<SearchGuiItem> items)
    {
        ClearItemPrices();
        foreach (var item in items)
            itemPrices.Add(item);
    }

    /// <summary>
    /// Gets all cached item prices as an array.
    /// </summary>
    /// <returns>Array of all cached search GUI items with price information.</returns>
    public SearchGuiItem[] GetAllItemPrices()
    {
        return itemPrices.ToArray();
    }

    /// <summary>
    /// Clears all cached item price data.
    /// </summary>
    public void ClearItemPrices()
    {
        itemPrices = new ConcurrentBag<SearchGuiItem>();
    }
}

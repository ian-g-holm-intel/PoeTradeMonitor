using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Interfaces;

/// <summary>
/// Interface for checking and managing Path of Exile item prices.
/// </summary>
public interface IPoePriceChecker
{
    /// <summary>
    /// Starts the price checking service.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stops the price checking service.
    /// </summary>
    void Stop();

    /// <summary>
    /// Starts monitoring the price of an item.
    /// </summary>
    /// <param name="itemSearchRequest"></param>
    void StartMonitoringItem(TradeSearchRequest itemSearchRequest);

    /// <summary>
    /// Stops monitoring the price of an item.
    /// </summary>
    /// <param name="name"></param>
    void StopMonitoringItem(string name);

    /// <summary>
    /// Gets the current price for the specified item.
    /// </summary>
    /// <param name="itemName">The name of the item to get the price for.</param>
    /// <returns>The current price of the item.</returns>
    decimal GetPrice(string itemName);
    
    /// <summary>
    /// Updates all prices and logs the results asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdatePricesAndLogAsync();
}
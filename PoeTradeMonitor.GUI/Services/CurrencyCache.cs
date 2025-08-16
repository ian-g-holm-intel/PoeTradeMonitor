using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PoeLib.Tools;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeLib.Common;
using PoeLib.Extensions;

namespace PoeTradeMonitor.GUI.Services;

/// <summary>
/// Interface for managing currency cache operations including retrieval, updates, and trade validation.
/// </summary>
public interface ICurrencyCache
{
    /// <summary>
    /// Gets currency information for the specified currency type.
    /// </summary>
    /// <param name="type">The type of currency to retrieve.</param>
    /// <returns>Currency information or a default entry if not found.</returns>
    CurrencyInfo GetCurrency(TradeCurrencyType type);
    /// <summary>
    /// Asynchronously updates currency information from the player's stash for the specified league.
    /// </summary>
    /// <param name="league">The league to retrieve currency data from.</param>
    Task UpdateCurrenciesAsync(string league);
    /// <summary>
    /// Updates the currency cache with the provided currency dictionary.
    /// </summary>
    /// <param name="currencies">Dictionary of currency types and their information.</param>
    void UpdateCurrencies(Dictionary<TradeCurrencyType, CurrencyInfo> currencies);
    /// <summary>
    /// Gets the total currency count converted to base currency equivalent.
    /// </summary>
    /// <returns>The total currency value in base currency units.</returns>
    int GetCurrencyCount();
    /// <summary>
    /// Checks if there is enough currency available for the specified trade.
    /// </summary>
    /// <param name="currencies">The currencies required for the trade.</param>
    /// <returns>True if sufficient currency is available; otherwise, false.</returns>
    bool EnoughCurrencyForTrade(IEnumerable<CurrencyInfo> currencies);
    /// <summary>
    /// Converts divine orb fractional amounts to base currency for trading.
    /// </summary>
    /// <param name="price">The price information.</param>
    /// <param name="stackSize">The stack size for the trade.</param>
    /// <param name="divineRate">The exchange rate for divine orbs.</param>
    /// <returns>List of currency information with converted amounts.</returns>
    List<CurrencyInfo> ConvertDivFractions(PriceInfo price, int stackSize, decimal divineRate);
}

/// <summary>
/// Implementation of currency cache that manages Path of Exile currency information and trading operations.
/// </summary>
public class CurrencyCache : ICurrencyCache
{
    private readonly ILogger<CurrencyCache> log;
    private readonly IStashCurrencyRetriever currencyRetriever;
    private readonly ICurrencyPriceCache priceCache;
    private ConcurrentDictionary<TradeCurrencyType, CurrencyInfo> currencyDictionary = new ConcurrentDictionary<TradeCurrencyType, CurrencyInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyCache"/> class.
    /// </summary>
    /// <param name="currencyRetriever">Service for retrieving currency from stash.</param>
    /// <param name="priceCache">Cache for currency price information.</param>
    /// <param name="log">Logger for the currency cache.</param>
    public CurrencyCache(IStashCurrencyRetriever currencyRetriever, ICurrencyPriceCache priceCache, ILogger<CurrencyCache> log)
    {
        this.currencyRetriever = currencyRetriever;
        this.priceCache = priceCache;
        this.log = log;
    }

    public CurrencyInfo GetCurrency(TradeCurrencyType type)
    {
        return currencyDictionary.ContainsKey(type) ? currencyDictionary[type] : new CurrencyInfo { Type = type, Amount = 0 };
    }

    public async Task UpdateCurrenciesAsync(string league)
    {
        var currencies = await currencyRetriever.GetStashCurrency(league);
        if (currencies.Length == 0)
            return;

        currencyDictionary.Clear();
        foreach (var currencyItem in currencies)
        {
            if (currencyItem.Rarity != ItemRarity.Currency)
                continue;

            var type = currencyItem.Name.GetCurrencyType();
            if (type != TradeCurrencyType.Unknown)
            {
                var currency = new CurrencyInfo
                {
                    Type = type,
                    Amount = currencyItem.StackSize,
                };

                if (currencyDictionary.ContainsKey(type))
                {
                    currencyDictionary[type].Amount += currency.Amount;
                }
                else
                    currencyDictionary[type] = currency;
            }
        }
    }

    public void UpdateCurrencies(Dictionary<TradeCurrencyType, CurrencyInfo> currencies)
    {
        currencyDictionary = new ConcurrentDictionary<TradeCurrencyType, CurrencyInfo>(currencies);
    }

    public void LogCurrencies()
    {
        foreach (var currencyType in currencyDictionary.Keys)
        {
            log.LogInformation(currencyDictionary[currencyType].ToString());
        }
    }

    public int GetCurrencyCount()
    {
        var divineOrbs = currencyDictionary.ContainsKey(TradeCurrencyType.Divine) ? currencyDictionary[TradeCurrencyType.Divine].Amount : 0;
        var baseOrbs = currencyDictionary.ContainsKey(Constants.BaseCurrencyType) ? currencyDictionary[Constants.BaseCurrencyType].Amount : 0;
        CurrencyPrice divinePrice = new CurrencyPrice(){Type = TradeCurrencyType.Divine };
        try
        {
            divinePrice = priceCache.GetPrice(TradeCurrencyType.Divine);
        }
        catch (CurrencyPriceNotFoundException)
        {
            log.LogWarning("Unable to get divine price");
        }
        
        return Convert.ToInt32(baseOrbs + divinePrice.SellPrice * divineOrbs);
    }

    public List<CurrencyInfo> ConvertDivFractions(PriceInfo price, int stackSize, decimal divineRate)
    {
        var updatedCurrencies = new List<CurrencyInfo>();

        if (price.CurrencyType == TradeCurrencyType.Divine)
        {
            var amount = price.Amount * stackSize;
            var fraction = amount - Math.Truncate(amount);
            var baseCurrencyAmount = Math.Round(fraction * divineRate);
            updatedCurrencies.Add(new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = Math.Truncate(amount) });
            updatedCurrencies.Add(new CurrencyInfo { Type = Constants.BaseCurrencyType, Amount = baseCurrencyAmount });
        }
        else if (price.CurrencyType == Constants.BaseCurrencyType)
        {
            updatedCurrencies.Add(new CurrencyInfo { Type = Constants.BaseCurrencyType, Amount = Math.Ceiling(price.Amount) });
        }

        return updatedCurrencies;
    }

    public bool EnoughCurrencyForTrade(IEnumerable<CurrencyInfo> currencies)
    {
        foreach (var currency in currencies)
        {
            if (!currencyDictionary.ContainsKey(currency.Type))
            {
                log.LogInformation($"No currency of type {currency.Type} available for trade");
                //LogCurrencies();
                return false;
            }
            else if (currencyDictionary[currency.Type].Amount < currency.Amount)
            {
                log.LogInformation($"Not enough currency of type {currency.Type}: Need {currency.Amount}, Have {currencyDictionary[currency.Type].Amount}");
                //LogCurrencies();
                return false;
            }
        }

        if (currencies.Sum(c => c.GetCurrencyStatcks()) > 60)
        {
            log.LogInformation("Requested trade has more than 60 stacks of currency, too large for trade window");
            return false;
        }

        return true;
    }
}
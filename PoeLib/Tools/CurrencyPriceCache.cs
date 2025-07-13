using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PoeLib.Tools;

public interface ICurrencyPriceCache
{
    TimeSpan Age { get; }
    void SetPrices(IEnumerable<CurrencyPrice> prices);
    bool ContainsPrice(CurrencyType currencyType);
    CurrencyPrice GetPrice(CurrencyType currencyType);
    bool PricesHaveChanged(IEnumerable<CurrencyPrice> prices);
    IEnumerable<CurrencyPrice> GetAllPrices();
}

public class CurrencyPriceCache : ICurrencyPriceCache
{
    private readonly ConcurrentDictionary<CurrencyType, CurrencyPrice> currencyPricesDictionary;
    private DateTime lastUpdated = DateTime.MinValue;

    public CurrencyPriceCache()
    {
        currencyPricesDictionary = new ConcurrentDictionary<CurrencyType, CurrencyPrice>();
    }

    public TimeSpan Age => DateTime.Now - lastUpdated;

    public bool PricesHaveChanged(IEnumerable<CurrencyPrice> prices)
    {
        foreach (var price in prices)
        {
            if (!currencyPricesDictionary.ContainsKey(price.Type) || !currencyPricesDictionary[price.Type].Equals(price))
                return true;
        }
        return false;
    }

    public void SetPrices(IEnumerable<CurrencyPrice> prices)
    {
        foreach (var price in prices)
        { 
            currencyPricesDictionary[price.Type] = price;
        }
        lastUpdated = DateTime.Now;
    }

    public bool ContainsPrice(CurrencyType currencyType)
    {
        return currencyPricesDictionary.ContainsKey(currencyType);
    }

    public CurrencyPrice GetPrice(CurrencyType currencyType)
    {
        if (!currencyPricesDictionary.ContainsKey(currencyType))
            throw new CurrencyPriceNotFoundException(currencyType);
        return currencyPricesDictionary[currencyType];
    }

    public IEnumerable<CurrencyPrice> GetAllPrices()
    {
        return currencyPricesDictionary.Values.ToArray();
    }
}

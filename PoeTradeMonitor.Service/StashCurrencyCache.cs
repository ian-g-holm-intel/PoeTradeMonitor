using PoeLib.Common;
using PoeTrade.Contracts;
using System.Collections.Concurrent;

namespace PoeTradeMonitor.Service;

public interface IStashCurrencyCache
{
    Dictionary<TradeCurrencyType, CurrencyInfo> GetCurrency();
    void UpdateCurrencies(IEnumerable<CurrencyStack> currencyStacks);
}

public class StashCurrencyCache : IStashCurrencyCache
{
    private readonly ConcurrentDictionary<TradeCurrencyType, CurrencyInfo> currencyDictionary = new ConcurrentDictionary<TradeCurrencyType, CurrencyInfo>();
    private readonly ILogger<StashCurrencyCache> logger;

    public StashCurrencyCache(ILogger<StashCurrencyCache> logger)
    {
        this.logger = logger;
    }

    public void UpdateCurrencies(IEnumerable<CurrencyStack> currencyStacks)
    {
        currencyDictionary.Clear();
        foreach (var currencyStack in currencyStacks)
        {
            if (currencyStack.Type != TradeCurrencyType.Unknown)
            {
                var currency = new CurrencyInfo
                {
                    Type = currencyStack.Type,
                    Amount = currencyStack.Amount
                };

                if (currencyDictionary.ContainsKey(currencyStack.Type))
                {
                    currencyDictionary[currencyStack.Type].Amount += currency.Amount;
                }
                else
                    currencyDictionary[currencyStack.Type] = currency;
            }
        }
    }

    public Dictionary<TradeCurrencyType, CurrencyInfo> GetCurrency()
    {
        return currencyDictionary.ToDictionary();
    }
}

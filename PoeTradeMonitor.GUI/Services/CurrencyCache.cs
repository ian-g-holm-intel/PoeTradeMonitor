using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Force.DeepCloner;
using Microsoft.Extensions.Logging;
using PoeLib.GuiDataClasses;
using PoeTradeMonitor.GUI.DataRetrievers;

namespace PoeLib.Tools;

public interface ICurrencyCache
{
    Currency GetCurrency(CurrencyType type);
    Task UpdateCurrencies(string league);
    int GetChaosCount();
    bool EnoughCurrencyForTrade(IEnumerable<Currency> currencies);
    List<Currency> ConvertDivFractions(StashGuiItem stashGuiItem);
    string GetScaledCurrencyString(IEnumerable<Currency> currencies, int count);
}

public class CurrencyCache : ICurrencyCache
{
    private readonly ILogger<CurrencyCache> log;
    private readonly Regex numeratorPattern = new Regex(@"\d+", RegexOptions.Compiled);
    private readonly Regex denominatorPattern = new Regex(@"(?<=/)\d+", RegexOptions.Compiled);
    private readonly IStashCurrencyRetriever currencyRetriever;
    private readonly ICurrencyPriceCache priceCache;
    private readonly ConcurrentDictionary<CurrencyType, Currency> currencyDictionary = new ConcurrentDictionary<CurrencyType, Currency>();

    public CurrencyCache(IStashCurrencyRetriever currencyRetriever, ICurrencyPriceCache priceCache, ILogger<CurrencyCache> log)
    {
        this.currencyRetriever = currencyRetriever;
        this.priceCache = priceCache;
        this.log = log;
    }

    public Currency GetCurrency(CurrencyType type)
    {
        return currencyDictionary.ContainsKey(type) ? currencyDictionary[type] : new Currency { Type = type, Amount = 0 };
    }

    public async Task UpdateCurrencies(string league)
    {
        var currencies = await currencyRetriever.GetStashCurrency(league);
        if (currencies.Length == 0)
            return;

        currencyDictionary.Clear();
        foreach (var currencyItem in currencies)
        {
            if (currencyItem.rarity != Rarity.Currency)
                continue;

            var type = currencyItem.Name.GetCurrencyType();
            if (type != CurrencyType.none)
            {
                var currency = new Currency
                {
                    Type = type,
                    Amount = currencyItem.stackSize,
                    HasPriceSet = !string.IsNullOrEmpty(currencyItem.Note) && !currencyItem.Note.Contains("~skip")
                };

                if (currency.HasPriceSet)
                {
                    var numeratorStringMatch = numeratorPattern.Match(currencyItem.Note);
                    var denominatorStringMatch = denominatorPattern.Match(currencyItem.Note);
                    if (numeratorStringMatch.Success && denominatorStringMatch.Success)
                        currency.Price = new Fraction(decimal.ToInt32(decimal.Parse(numeratorStringMatch.ToString())), !string.IsNullOrEmpty(denominatorStringMatch.ToString()) ? decimal.ToInt32(decimal.Parse(denominatorStringMatch.ToString())) : 1);
                }

                if (currencyDictionary.ContainsKey(type))
                {
                    currencyDictionary[type].Amount += currency.Amount;
                }
                else
                    currencyDictionary[type] = currency;
            }
        }
    }

    public void LogCurrencies()
    {
        foreach (var currencyType in currencyDictionary.Keys)
        {
            log.LogInformation(currencyDictionary[currencyType].ToString());
        }
    }

    public int GetChaosCount()
    {
        var divineOrbs = currencyDictionary.ContainsKey(CurrencyType.divine) ? currencyDictionary[CurrencyType.divine].Amount : 0;
        var chaosOrbs = currencyDictionary.ContainsKey(CurrencyType.chaos) ? currencyDictionary[CurrencyType.chaos].Amount : 0;
        CurrencyPrice divinePrice = new CurrencyPrice() { Type = CurrencyType.divine };
        try
        {
            divinePrice = priceCache.GetPrice(CurrencyType.divine);
        }
        catch (CurrencyPriceNotFoundException)
        {
            log.LogWarning("Unable to get divine price");
        }

        return Convert.ToInt32(chaosOrbs + divinePrice.SellPrice * divineOrbs);
    }

    public string GetScaledCurrencyString(IEnumerable<Currency> currencies, int count)
    {
        var totalCurrency = new List<Currency>();
        foreach (var currency in currencies)
        {
            totalCurrency.Add(new Currency { Type = currency.Type, Amount = currency.Amount * count, ChaosEquiv = currency.ChaosEquiv });
        }
        return totalCurrency.Select(c => c.ToString()).Aggregate((current, next) => $"{current}, {next}");
    }

    public List<Currency> ConvertDivFractions(StashGuiItem stashGuiItem)
    {
        var price = stashGuiItem.TradeRequest.Price;
        var divToChaosRatio = stashGuiItem.TradeRequest.DivineRate;
        var currencyDict = price.Currencies.ToDictionary(item => item.Type).DeepClone();
        foreach (var currencyType in currencyDict.Keys)
            currencyDict[currencyType].Amount = currencyDict[currencyType].Amount * stashGuiItem.StackSize;

        if (currencyDict.ContainsKey(CurrencyType.divine))
        {
            var amount = currencyDict[CurrencyType.divine].Amount;
            var fraction = amount - Math.Truncate(amount);
            var chaosAmount = Math.Round(fraction * divToChaosRatio);
            currencyDict[CurrencyType.divine].Amount = Math.Truncate(amount);
            if (currencyDict.ContainsKey(CurrencyType.chaos))
            {
                currencyDict[CurrencyType.chaos].Amount += chaosAmount;
            }
            else
            {
                currencyDict[CurrencyType.chaos] = new Currency
                {
                    Type = CurrencyType.chaos,
                    Amount = chaosAmount,
                    HasPriceSet = false
                };
            }
        }
        else if (stashGuiItem.TradeRequest.Item.rarity != Rarity.Currency && currencyDict.ContainsKey(CurrencyType.chaos) && currencyDict[CurrencyType.chaos].Amount > divToChaosRatio)
        {
            var chaos = Convert.ToInt32(currencyDict[CurrencyType.chaos].Amount);
            var ratio = Convert.ToInt32(divToChaosRatio);
            currencyDict[CurrencyType.divine] = new Currency { Type = CurrencyType.divine, Amount = chaos / ratio, ChaosEquiv = (chaos / ratio) * divToChaosRatio, HasPriceSet = false };
            currencyDict[CurrencyType.chaos].Amount = currencyDict[CurrencyType.chaos].Amount - currencyDict[CurrencyType.divine].ChaosEquiv;
        }

        if (currencyDict.ContainsKey(CurrencyType.chaos))
            currencyDict[CurrencyType.chaos].Amount = Math.Ceiling(currencyDict[CurrencyType.chaos].Amount);

        var updatedCurrencies = currencyDict.Values.OrderByDescending(v => v.Type).ToList();
        price.Currencies = updatedCurrencies;
        return updatedCurrencies;
    }

    public bool EnoughCurrencyForTrade(IEnumerable<Currency> currencies)
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
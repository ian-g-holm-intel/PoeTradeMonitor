using PoeLib.Common;
using PoeLib.PriceFetchers;
using PoeLib.Tools;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.DataRetrievers;

public interface ICurrencyPriceRetriever
{
    Task<Dictionary<TradeCurrencyType, CurrencyPrice>> GetCurrencyPrices(string league);
}

public class CurrencyPriceRetriever : ICurrencyPriceRetriever
{
    private readonly IPriceFetcherWrapper priceFetcher;
    private readonly ICurrencyPriceCache currencyPriceCache;

    public CurrencyPriceRetriever(IPriceFetcherWrapper priceFetcher, ICurrencyPriceCache currencyPriceCache)
    {
        this.priceFetcher = priceFetcher;
        this.currencyPriceCache = currencyPriceCache;
    }

    public async Task<Dictionary<TradeCurrencyType, CurrencyPrice>> GetCurrencyPrices(string league)
    {
        var prices = await priceFetcher.GetCurrencyData(league);
        if (prices == null)
            return new Dictionary<TradeCurrencyType, CurrencyPrice>();
        
        currencyPriceCache.SetPrices(prices.Values);
        return prices;
    }
}

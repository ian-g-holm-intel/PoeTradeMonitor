using PoeLib;
using PoeLib.PriceFetchers;
using PoeLib.Tools;

namespace PoeTradeMonitor.GUI.DataRetrievers;

public interface ICurrencyPriceRetriever
{
    Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyPrices(string league);
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

    public async Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyPrices(string league)
    {
        var prices = await priceFetcher.GetCurrencyData(league);
        if (prices == null)
            return new Dictionary<CurrencyType, CurrencyPrice>();
        
        currencyPriceCache.SetPrices(prices.Values);
        return prices;
    }
}

using Microsoft.Extensions.Logging;
using PoeLib.Common;
using PoeLib.JSON.OrbWatchTrade;
using PoeTrade.Contracts;
using System.Text.Json;

namespace PoeLib.PriceFetchers.OrbWatchTrade;

public class OrbWatchTradeWrapper : IPriceFetcher
{
    private readonly ILogger<OrbWatchTradeWrapper> log;
    private readonly IHttpClientFactory httpClientFactory;

    public OrbWatchTradeWrapper(ILogger<OrbWatchTradeWrapper> log, IHttpClientFactory httpClientFactory)
    {
        this.log = log;
        this.httpClientFactory = httpClientFactory;
    }

    public string Name => "OrbWatchTrade";

    public async Task<Dictionary<TradeCurrencyType, CurrencyPrice>> GetCurrencyData(string league)
    {
        var client = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://orbwatch.trade/api/currency/market-data?mode=buy&realm=Standard");

        AddWebHeaders(request);
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var currencyResponse = JsonSerializer.Deserialize<OrbWatchResponse>(json)!;
                return MapToCurrencyPrices(currencyResponse);
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to deserialize currency data: {json} - {ex}");
                return new Dictionary<TradeCurrencyType, CurrencyPrice>();
            }
        }
        return new Dictionary<TradeCurrencyType, CurrencyPrice>();
    }

    private Dictionary<TradeCurrencyType, CurrencyPrice> MapToCurrencyPrices(OrbWatchResponse response)
    {
        var prices = new Dictionary<TradeCurrencyType, CurrencyPrice>();
        if (response.Data?.Currencies != null)
        {
            foreach (var currency in response.Data.Currencies)
            {
                if (currency.Id != null && TryParseCurrencyType(currency.Id, out var currencyType))
                {
                    prices[currencyType] = new CurrencyPrice
                    {
                        Type = currencyType,
                        BuyPrice = 1/currency.MedianPrice,
                        SellPrice = 1/currency.MedianPrice, 
                        AvgPrice = 1/currency.MeanPrice
                    };
                }
            }
        }
        return prices;
    }

    private bool TryParseCurrencyType(string id, out TradeCurrencyType currencyType)
    {
        switch (id.ToLower())
        {
            case "alch": currencyType = TradeCurrencyType.Alch; return true;
            case "annul": currencyType = TradeCurrencyType.Annul; return true;
            case "aug": currencyType = TradeCurrencyType.Aug; return true;
            case "chance": currencyType = TradeCurrencyType.Chance; return true;
            case "chaos": currencyType = TradeCurrencyType.Chaos; return true;
            case "divine": currencyType = TradeCurrencyType.Divine; return true;
            case "exalted": currencyType = TradeCurrencyType.Exalted; return true;
            case "gcp": currencyType = TradeCurrencyType.Gcp; return true;
            case "mirror": currencyType = TradeCurrencyType.Mirror; return true;
            case "regal": currencyType = TradeCurrencyType.Regal; return true;
            case "vaal": currencyType = TradeCurrencyType.Vaal; return true;
            default: currencyType = TradeCurrencyType.Unknown; return false;
        }
    }

    private HttpRequestMessage AddWebHeaders(HttpRequestMessage message)
    {
        message.Headers.Add("Accept", "*/*");
        message.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        message.Headers.Add("If-None-Match", "W/\"4b868-5ev7FowttQCtDBj7FoDEi7q2rP0\"");
        message.Headers.Add("Priority", "u=1, i");
        message.Headers.Add("Referer", "https://orbwatch.trade/");
        message.Headers.Add("Sec-Ch-Ua", "\"Not(A:Brand\";v=\"99\", \"Brave\";v=\"133\", \"Chromium\";v=\"133\"");
        message.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        message.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
        message.Headers.Add("Sec-Fetch-Dest", "empty");
        message.Headers.Add("Sec-Fetch-Mode", "cors");
        message.Headers.Add("Sec-Fetch-Site", "same-origin");
        message.Headers.Add("Sec-Gpc", "1");
        message.Headers.UserAgent.ParseAdd(Constants.UserAgent);
        return message;
    }

    public Task<SearchItemGroup> GetFragmentData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetUniqueJewelData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetUniqueFlaskData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetUniqueWeaponData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetUniqueArmorData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetGemsData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetUniqueAccessoryData(string league) =>
        throw new NotImplementedException();

    public Task<SearchItemGroup> GetDivinationCardData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueMapData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetFossils(string league)
    {
        throw new NotImplementedException();
    }
}
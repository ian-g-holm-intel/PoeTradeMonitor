using Microsoft.Extensions.Logging;
using PoeLib.Common;
using PoeLib.JSON.Poe2Scout;
using PoeTrade.Contracts;
using System.Text.Json;

namespace PoeLib.PriceFetchers.Poe2Scout;
public class Poe2ScoutWrapper : IPriceFetcher
{
    private ILogger<Poe2ScoutWrapper> log;
    private readonly IHttpClientFactory httpClientFactory;

    public Poe2ScoutWrapper(ILogger<Poe2ScoutWrapper> log, IHttpClientFactory httpClientFactory)
    {
        this.log = log;
        this.httpClientFactory = httpClientFactory;
    }

    public string Name => "Poe2Scout";

    public async Task<Dictionary<TradeCurrencyType, CurrencyPrice>> GetCurrencyData(string league)
    {
        var client = httpClientFactory.CreateClient();
        var currencyDetailsApi = $"https://poe2scout.com/api/items/currency/currency";
        var request = new HttpRequestMessage(HttpMethod.Get, currencyDetailsApi);
        AddWebHeaders(request);
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var currencyResponse = JsonSerializer.Deserialize<CurrencyResponse>(json)!;
                var prices = GetPrices(currencyResponse).ToDictionary(price => price.Type, price => price);
                return prices;
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to deserialize currency data: {json} - {ex}");
                return new Dictionary<TradeCurrencyType, CurrencyPrice>();
            }
        }
        return new Dictionary<TradeCurrencyType, CurrencyPrice>();
    }

    private HttpRequestMessage AddWebHeaders(HttpRequestMessage message)
    {
        message.Headers.Add("Accept", "*/*");
        message.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        message.Headers.Add("Priority", "u=1, i");
        message.Headers.Add("Referer", "https://poe2scout.com/economy/currency");
        message.Headers.Add("Sec-Ch-Ua", "\"Brave\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
        message.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        message.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
        message.Headers.Add("Sec-Fetch-Dest", "empty");
        message.Headers.Add("Sec-Fetch-Mode", "cors");
        message.Headers.Add("Sec-Fetch-Site", "same-origin");
        message.Headers.Add("Sec-Gpc", "1");
        message.Headers.UserAgent.ParseAdd(Constants.UserAgent);
        return message;
    }

    private IEnumerable<CurrencyPrice> GetPrices(CurrencyResponse currencyResponse)
    {
        foreach (var currencyType in (TradeCurrencyType[])Enum.GetValues(typeof(TradeCurrencyType)))
        {
            if (currencyType == TradeCurrencyType.Unknown)
                continue;

            // Convert enum to string (e.g., TradeCurrencyType.chaos -> "chaos")
            string currencyApiId = currencyType.ToString().ToLower();

            // Find the currency in the response
            var currencyInfo = currencyResponse.Items?.FirstOrDefault(item =>
                item.ApiId?.Equals(currencyApiId, StringComparison.OrdinalIgnoreCase) == true);

            if (currencyInfo == null || currencyInfo.CurrentPrice == 0)
                continue;

            // The price is now directly available as CurrentPrice
            decimal buyPrice = currencyInfo.CurrentPrice;

            // Find any sell orders in price history
            var sellOrder = currencyInfo.PriceLogs
                ?.Where(ph => ph != null && ph.Price.HasValue && ph.Quantity.HasValue)
                .OrderByDescending(ph => ph.Time)
                .FirstOrDefault();

            decimal sellPrice = sellOrder?.Price ?? buyPrice;

            // If prices are given as fractions (like 0.5 exalts), we need to invert them
            // since we want the price in terms of the target currency
            if (buyPrice < 1)
                buyPrice = 1 / buyPrice;
            if (sellPrice < 1)
                sellPrice = 1 / sellPrice;

            yield return new CurrencyPrice
            {
                Type = currencyType,
                BuyPrice = buyPrice,
                SellPrice = sellPrice,
                AvgPrice = (buyPrice + sellPrice) / 2
            };
        }
    }

    public Task<SearchItemGroup> GetFragmentData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueJewelData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueFlaskData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueWeaponData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueArmorData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetGemsData(string league)
    {
        throw new NotImplementedException();
    }

    public Task<SearchItemGroup> GetUniqueAccessoryData(string league)
    {
        throw new NotImplementedException();
    }

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
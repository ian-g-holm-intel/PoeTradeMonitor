using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoeLib.JSON.PoeWatch;
using System.Text.Json;
using System.Net.Http;

namespace PoeLib.PriceFetchers.PoeWatch;

public class PoeWatchWrapper : IPriceFetcher
{
    private readonly ILogger<PoeWatchWrapper> log;
    private readonly IHttpClientFactory httpClientFactory;

    public PoeWatchWrapper(ILogger<PoeWatchWrapper> log, IHttpClientFactory httpClientFactory)
    {
        this.log = log;
        this.httpClientFactory = httpClientFactory;
    }

    public string Name => "PoeWatch";

    public async Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyData(string league)
    {
        string json = "";
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=currency";
            json = await client.GetStringAsync(currencyDetailsApi);
            var currencyData = JsonSerializer.Deserialize<CurrencyData[]>(json);
            return GetPrices(currencyData.Where(currency => currency.group.Equals("currency")).ToArray()).ToDictionary(price => price.Type, price => price);
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize currency data: {json} - {ex}");
            return new Dictionary<CurrencyType, CurrencyPrice>();
        }
    }

    public async Task<Category[]> GetCategories()
    {
        string json = "";
        try
        {
            var client = httpClientFactory.CreateClient();
            var categoriesApi = "https://api.poe.watch/categories";
            json = await client.GetStringAsync(categoriesApi);
            return JsonSerializer.Deserialize<Category[]>(json);
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize category data: {json} - {ex}");
            return new Category[0];
        }
    }

    private IEnumerable<CurrencyPrice> GetPrices(CurrencyData[] currencyData)
    {
        foreach (var currencyType in (CurrencyType[]) Enum.GetValues(typeof(CurrencyType)))
        {
            switch (currencyType)
            {
                case CurrencyType.none:
                    continue;
                case CurrencyType.chaos:
                    yield return new CurrencyPrice {Type = CurrencyType.chaos};
                    break;
                default:
                {
                    var currencyName = currencyType.GetCurrencyDescription();
                    var currencyInfo = currencyData.FirstOrDefault(currency => currency.name == currencyName);
                    if (currencyInfo == null)
                        continue;

                    yield return new CurrencyPrice{Type = currencyType, BuyPrice = currencyInfo.min, SellPrice = currencyInfo.min, AvgPrice = currencyInfo.min};
                    break;
                }
            }
        }
    }

    public async Task<SearchItemGroup> GetFragmentData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Fragments" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=fragment";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Normal, Source = Name, AllowCorrupted = true }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize fragment data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetDivinationCardData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Divination Cards" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=card";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.DivinationCard, Volatile = detail.lowConfidence, Source = Name, AllowCorrupted = true }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize divination card data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueMapData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Maps" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=uniqueMap";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, MapTier = detail.mapTier ?? 1, Volatile = detail.lowConfidence, Source = Name, AllowCorrupted = true }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique map data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueJewelData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Jewels" };
        try
        { 
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=jewel";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, Variant = string.Empty, Volatile = detail.lowConfidence, Source = Name }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique jewel data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueFlaskData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Flasks" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=flask";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, Variant = string.Empty, Volatile = detail.lowConfidence, Source = Name }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique flask data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueWeaponData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Weapons" };
        try
        { 
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=weapon";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, Links = detail.linkCount > 4 ? detail.linkCount.Value : 0, Volatile = detail.lowConfidence, Source = Name }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique weapon data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueArmorData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Armor" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=armour";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, Links = detail.linkCount > 4 ? detail.linkCount.Value : 0, Volatile = detail.lowConfidence, Source = Name }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique armor data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueAccessoryData(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Accessories" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=accessory";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Unique, Volatile = detail.lowConfidence, Source = Name }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize unique accessory data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetFossils(string league)
    {
        string json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Fossils" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://api.poe.watch/get?league={league}&category=fossil";
            json = await client.GetStringAsync(currencyDetailsApi);
            var itemData = JsonSerializer.Deserialize<ItemData[]>(json);
            if (itemData == null)
                return searchItemGroup;
            var items = itemData.OrderByDescending(f => f.min);
            searchItemGroup.SearchItems = items.Select(detail => new SearchItem {Name = detail.name, Price = Math.Round(detail.min), Rarity = Rarity.Currency, Volatile = detail.lowConfidence, Source = Name, AllowCorrupted = true }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize fossil data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public Task<SearchItemGroup> GetGemsData(string league)
    {
        return Task.FromResult(new SearchItemGroup());
    }
}

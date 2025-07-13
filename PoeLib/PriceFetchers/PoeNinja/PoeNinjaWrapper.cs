using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoeLib.JSON.PoeNinja;
using System.Text.Json;
using System.Net.Http;

namespace PoeLib.PriceFetchers.PoeNinja;

public class PoeNinjaWrapper : IPriceFetcher
{
    private ILogger<PoeNinjaWrapper> log;
    private readonly IHttpClientFactory httpClientFactory;

    public PoeNinjaWrapper(ILogger<PoeNinjaWrapper> log, IHttpClientFactory httpClientFactory)
    {
        this.log = log;
        this.httpClientFactory = httpClientFactory;
    }

    public string Name => "PoeNinja";

    public async Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyData(string league)
    {
        string json = "";
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/Data/currencyoverview?league={league}&type=Currency";
            json = await client.GetStringAsync(currencyDetailsApi);
            var currencyData = JsonSerializer.Deserialize<CurrencyData>(json);
            return GetPrices(currencyData).ToDictionary(price => price.Type, price => price);
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize currency data: {json} - {ex}");
            return new Dictionary<CurrencyType, CurrencyPrice>();
        }
    }

    public async Task<PoeNinjaStats> GetStats()
    {
        string json = "";
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = "https://poe.ninja/api/Data/GetStats";
            json = await client.GetStringAsync(currencyDetailsApi);
            return JsonSerializer.Deserialize<PoeNinjaStats>(json);
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize currency data: {json} - {ex}");
            return new PoeNinjaStats();
        }
    }

    private IEnumerable<CurrencyPrice> GetPrices(CurrencyData currencyData)
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
                    var currencyInfo = currencyData.lines.FirstOrDefault(line => line.currencyTypeName == currencyName);

                    if (currencyInfo?.pay == null || currencyInfo.receive == null)
                        continue;

                    yield return new CurrencyPrice{Type = currencyType, BuyPrice = currencyInfo.receive.value, SellPrice = 1/currencyInfo.pay.value, AvgPrice = currencyInfo.chaosEquivalent};
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
            var currencyDetailsApi = $"https://poe.ninja/api/Data/currencyoverview?league={league}&type=Fragment";
            json = await client.GetStringAsync(currencyDetailsApi);
            var fragmentDetails = JsonSerializer.Deserialize<CurrencyData>(json);
            if (fragmentDetails == null)
                return searchItemGroup;
            var fragments = fragmentDetails.lines.OrderByDescending(f => f.chaosEquivalent);
            searchItemGroup.SearchItems = fragments.Select(detail => new SearchItem { Name = detail.currencyTypeName, Price = Math.Round(detail.chaosEquivalent), Rarity = Rarity.Normal, Source = Name, AllowCorrupted = true }).ToList();
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
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Divination Cards" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/data/itemoverview?league={league}&type=DivinationCard";
            json = await client.GetStringAsync(currencyDetailsApi);
            var divinationCardDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (divinationCardDetails == null)
                return searchItemGroup;
            var divinationCards = divinationCardDetails.lines.OrderByDescending(c => c.chaosValue);
            searchItemGroup.SearchItems = divinationCards.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = Rarity.DivinationCard, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name, AllowCorrupted = true }).ToList();
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
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Maps" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/data/itemoverview?league={league}&type=UniqueMap";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueMapDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueMapDetails == null)
                return searchItemGroup;
            var uniqueMaps = uniqueMapDetails.lines.OrderByDescending(m => m.chaosValue);
            searchItemGroup.SearchItems = uniqueMaps.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, MapTier = detail.mapTier.Value, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name, AllowCorrupted = true }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
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
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Jewels" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/data/itemoverview?league={league}&type=UniqueJewel";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueJewelDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueJewelDetails == null)
                return searchItemGroup;
            var uniqueJewels = uniqueJewelDetails.lines.OrderByDescending(j => j.chaosValue);
            searchItemGroup.SearchItems = uniqueJewels.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize jewel data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueFlaskData(string league)
    {
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Flasks" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/data/itemoverview?league={league}&type=UniqueFlask";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueFlaskDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueFlaskDetails == null)
                return searchItemGroup;
            var uniqueFlasks = uniqueFlaskDetails.lines.OrderByDescending(f => f.chaosValue);
            searchItemGroup.SearchItems = uniqueFlasks.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize flask data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueWeaponData(string league)
    {
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Weapons" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/Data/ItemOverview?league={league}&type=UniqueWeapon";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueWeaponDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueWeaponDetails == null)
                return searchItemGroup;
            var uniqueWeapons = uniqueWeaponDetails.lines.OrderByDescending(w => w.chaosValue);
            searchItemGroup.SearchItems = uniqueWeapons.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Links = (detail.links.HasValue && detail.links.Value > 4) ? detail.links.Value : 0, Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize weapon data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueArmorData(string league)
    {
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Armor" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/Data/ItemOverview?league={league}&type=UniqueArmour";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueArmorDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueArmorDetails == null)
                return searchItemGroup;
            var uniqueArmors = uniqueArmorDetails.lines.OrderByDescending(a => a.chaosValue);
            searchItemGroup.SearchItems = uniqueArmors.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Links = (detail.links.HasValue && detail.links.Value > 4) ? detail.links.Value : 0, Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize armor data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetGemsData(string league)
    {
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Gems" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var currencyDetailsApi = $"https://poe.ninja/api/data/itemoverview?league={league}&type=SkillGem";
            json = await client.GetStringAsync(currencyDetailsApi);
            var uniqueArmorDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueArmorDetails == null)
                return searchItemGroup;
            var uniqueArmors = uniqueArmorDetails.lines.OrderByDescending(a => a.chaosValue);
            searchItemGroup.SearchItems = uniqueArmors.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Links = (detail.links.HasValue && detail.links.Value > 4) ? detail.links.Value : 0, Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize gems data: {json} - {ex}");
            return searchItemGroup;
        }
    }

    public async Task<SearchItemGroup> GetUniqueAccessoryData(string league)
    {
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Accessories" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var uniqueAccessoryDetailsApi = $"https://poe.ninja/api/Data/ItemOverview?league={league}&type=UniqueAccessory";
            json = await client.GetStringAsync(uniqueAccessoryDetailsApi);
            var uniqueAccessoryDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (uniqueAccessoryDetails == null)
                return searchItemGroup;
            var uniqueAccessories = uniqueAccessoryDetails.lines.OrderByDescending(a => a.chaosValue);
            searchItemGroup.SearchItems = uniqueAccessories.Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = detail.detailsId.Contains("relic") ? Rarity.UniqueFoil : Rarity.Unique, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name }).Where(i => i.Rarity != Rarity.UniqueFoil).ToList();
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
        var json = "";
        var searchItemGroup = new SearchItemGroup { GroupName = "Fossils" };
        try
        {
            var client = httpClientFactory.CreateClient();
            var fossilDetailsApi = $"https://poe.ninja/api/Data/ItemOverview?league={league}&type=Fossil";
            json = await client.GetStringAsync(fossilDetailsApi);
            var fossilDetails = JsonSerializer.Deserialize<ItemData>(json);
            if (fossilDetails == null)
                return searchItemGroup;
            var uniqueAccessories = fossilDetails.lines.OrderByDescending(a => a.chaosValue);
            searchItemGroup.SearchItems = uniqueAccessories.Where(detail => detail.count >= 10).Select(detail => new SearchItem { Name = detail.name, Price = Math.Round(detail.chaosValue), Rarity = Rarity.Currency, BaseType = detail.baseType, Variant = detail.variant, Volatile = detail.count < 10 || detail.sparkline.Volatile(), Source = Name, AllowCorrupted = true }).ToList();
            return searchItemGroup;
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to deserialize fossil data: {json} - {ex}");
            return searchItemGroup;
        }
    }
}

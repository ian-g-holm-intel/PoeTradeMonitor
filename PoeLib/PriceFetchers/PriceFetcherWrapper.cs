using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PoeLib.PriceFetchers;

public interface IPriceFetcherWrapper
{
    Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyData(string league);
    Task<SearchItemGroup> GetFragmentData(string league);
    Task<SearchItemGroup> GetDivinationCardData(string league);
    Task<SearchItemGroup> GetUniqueMapData(string league);
    Task<SearchItemGroup> GetUniqueJewelData(string league);
    Task<SearchItemGroup> GetUniqueFlaskData(string league);
    Task<SearchItemGroup> GetUniqueWeaponData(string league);
    Task<SearchItemGroup> GetUniqueArmorData(string league);
    Task<SearchItemGroup> GetUniqueAccessoryData(string league);
    Task<SearchItemGroup> GetFossils(string league);
}

public class PriceFetcherWrapper : IPriceFetcherWrapper
{
    private IEnumerable<IPriceFetcher> priceFetchers;
    private readonly ILogger<PriceFetcherWrapper> log;

    public PriceFetcherWrapper(IEnumerable<IPriceFetcher> priceFetchers, ILogger<PriceFetcherWrapper> log)
    {
        this.priceFetchers = priceFetchers;
        this.log = log;
    }

    private async Task<List<SearchItem>> Execute(Func<IPriceFetcher, Task<SearchItemGroup>> action)
    {
        return await GetPoeNinjaPrio(action);
    }

    private async Task<List<SearchItem>> GetPoeNinjaPrio(Func<IPriceFetcher, Task<SearchItemGroup>> action)
    {
        var itemGroups = await Task.WhenAll(priceFetchers.Select(action.Invoke));
        var poeNinjaItems = itemGroups[0].SearchItems;
        var poeWatchItems = itemGroups[1].SearchItems;

        return poeNinjaItems.Where(item => !item.Volatile).OrderByDescending(item => item.Price).ToList();
    }

    public async Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyData(string league)
    {
        foreach (var fetcher in priceFetchers)
        {
            try
            {
                return await fetcher.GetCurrencyData(league);
            }
            catch (Exception ex)
            {
                log.LogWarning($"Failed to reach {fetcher.Name}, retrying with next: {ex.Message}");
            }
        }
        return new Dictionary<CurrencyType, CurrencyPrice>();
    }

    public async Task<SearchItemGroup> GetFragmentData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Fragments" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetFragmentData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetDivinationCardData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Divination Cards" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetDivinationCardData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueMapData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Maps" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueMapData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueJewelData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Jewels" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueJewelData(league));

        var voices = searchItemGroup.SearchItems.SingleOrDefault(i => i.Name == "Voices" && string.IsNullOrEmpty(i.Variant));
        if(voices != null)
            voices.Variant = "1 passive";

        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueFlaskData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Flasks" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueFlaskData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueWeaponData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Weapons" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueWeaponData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueArmorData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Armor" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueArmorData(league));

        searchItemGroup.SearchItems.RemoveAll(i => i.Name == "Yriel's Fostering" && string.IsNullOrEmpty(i.Variant));

        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetUniqueAccessoryData(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Unique Accessories" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetUniqueAccessoryData(league));
        return searchItemGroup;
    }

    public async Task<SearchItemGroup> GetFossils(string league)
    {
        var searchItemGroup = new SearchItemGroup { GroupName = "Fossils" };
        searchItemGroup.SearchItems = await Execute(fetcher => fetcher.GetFossils(league));
        return searchItemGroup;
    }
}

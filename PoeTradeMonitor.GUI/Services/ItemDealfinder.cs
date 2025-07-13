using Microsoft.Extensions.Logging;
using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.Parsers;
using PoeLib.PriceFetchers;
using PoeLib.Tools;
using PoeTradeMonitor.GUI.DataRetrievers;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ItemSearch;

namespace PoeTradeMonitor.GUI.Services;

public class ItemDealfinder
{
    private bool running;
    private readonly IPriceFetcherWrapper priceFetcher;
    private readonly ICurrencyPriceRetriever currencyPriceRetriever;
    private readonly ICurrencyPriceCache currencyPriceCache;
    private readonly IPoePriceChecker poePriceChecker;
    private readonly IPoeItemLiveSearchManager poeLiveSearchManager;
    private readonly ICurrencyCache currencyCache;
    private readonly IStashItemsRetriever stashItemRetriever;
    private readonly ILiveSearchItemCache liveSearchItemCache;
    private readonly ItemPriceCache itemPriceCache;
    private readonly List<string> excludeList = new List<string>();
    private readonly List<string> veiledItems = new List<string>();
    private readonly ILogger<ItemDealfinder> logger;
    private CancellationTokenSource ctSource = new CancellationTokenSource();
    private Task dealfinderTask;
    private int LiveSearchLimit => liveSearchItemCache.ItemsPerAccount;
    public Strictness Strictness { get; set; }
    public string League { get; set; }
    public bool UndercutNotificationsEnabled { get; set; }
    public bool HighMarginMode { get; set; }

    public ItemDealfinder(IStashItemsRetriever stashItems, ICurrencyCache cc, IPriceFetcherWrapper pf, ICurrencyPriceRetriever currencyRetriever, ICurrencyPriceCache priceCache, IPoePriceChecker priceChecker, 
        IPoeItemLiveSearchManager poeLSM, ILiveSearchItemCache lsic, ILogger<ItemDealfinder> logger, ItemPriceCache itemCache)
    {
        stashItemRetriever = stashItems;
        currencyCache = cc;
        priceFetcher = pf;
        currencyPriceRetriever = currencyRetriever;
        currencyPriceCache = priceCache;
        itemPriceCache = itemCache;
        poePriceChecker = priceChecker;
        poeLiveSearchManager = poeLSM;
        liveSearchItemCache = lsic;
        this.logger = logger;

        excludeList.Add("Doryani's Invitation");
        excludeList.Add("Astramentis");
        excludeList.Add("Atziri's Splendour");
        excludeList.Add("Vessel of Vinktar");
        excludeList.Add("Impresence");
        excludeList.Add("Watcher's Eye");
        excludeList.Add("Skin of the Loyal");
        excludeList.Add("Demigod's Dominance");
        excludeList.Add("Demigod's Authority");
        excludeList.Add("Precursor's Emblem");
        excludeList.Add("Garb of the Ephemeral");
        excludeList.Add("Replica Bated Breath");
        excludeList.Add("Yriel's Fostering");
        excludeList.Add("The Admiral");
        excludeList.Add("Circle of Anguish");
        excludeList.Add("Circle of Guilt");
        excludeList.Add("Circle of Nostalgia");
        excludeList.Add("Circle of Regret");
        excludeList.Add("Circle of Fear");
        excludeList.Add("Fleshcrafter");
        excludeList.Add("Soul Catcher");
        excludeList.Add("Aul's Uprising");
        excludeList.Add("Glorious Vanity");
        excludeList.Add("Militant Faith");
        excludeList.Add("Brutal Restraint");
        excludeList.Add("Elegant Hubris");
        excludeList.Add("Lethal Pride");
        excludeList.Add("Thread of Hope");
        excludeList.Add("The Eternal Struggle");
        excludeList.Add("Widowhail");
        excludeList.Add("Expedition's End");

        // Temporary
        excludeList.Add("Sublime Vision");
        excludeList.Add("Death Rush");
        excludeList.Add("Progenesis");

        veiledItems.Add("Cane of Kulemak");
        veiledItems.Add("The Queen's Hunger");
        veiledItems.Add("Vivinsect");
        veiledItems.Add("The Crimson Storm");
        veiledItems.Add("Paradoxica");
        veiledItems.Add("Hyperboreus");
        veiledItems.Add("Cinderswallow Urn");
        veiledItems.Add("The Devouring Diadem");
        veiledItems.Add("Bitterbind Point");
    }

    public void Start()
    {
        if (!running)
        {
            logger.LogInformation("Starting ItemDealfinder");
            running = true;
            ctSource = new CancellationTokenSource();
            dealfinderTask = Task.Factory.StartNew(async () =>
            {
                var token = ctSource.Token;
                try
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        while (League == null)
                            await Task.Delay(10000, token);

                        try
                        {
                            var stashItemTask = stashItemRetriever.GetStashItems(League);
                            List<Task<SearchItemGroup>> dataRetrievers = new List<Task<SearchItemGroup>>
                            {
                                priceFetcher.GetDivinationCardData(League),
                                priceFetcher.GetFragmentData(League),
                                priceFetcher.GetUniqueAccessoryData(League),
                                priceFetcher.GetUniqueArmorData(League),
                                priceFetcher.GetUniqueFlaskData(League),
                                priceFetcher.GetUniqueJewelData(League),
                                priceFetcher.GetUniqueMapData(League),
                                priceFetcher.GetUniqueWeaponData(League),
                                priceFetcher.GetFossils(League)
                            };
                            var allItems = await Task.WhenAll(dataRetrievers);
                            var stashItems = await stashItemTask;

                            await currencyPriceRetriever.GetCurrencyPrices(League);
                            decimal divinePrice;
                            try
                            {
                                divinePrice = currencyPriceCache.GetPrice(CurrencyType.divine).SellPrice;
                            }
                            catch (CurrencyPriceNotFoundException)
                            {
                                await Task.Delay(1000, token);
                                continue;
                            }

                            var currencySearchItems = new List<SearchItem>();
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.mirror);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.mirrorshard);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.fracturingorb);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.temperingorb);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.blessingchayula);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.huntersexaltedorb);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.divine);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.tainteddivineteardrop);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.secondaryregrading);
                            AddCurrencySearchItem(currencySearchItems, CurrencyType.orbofdominance);
                            var currencyGroup = new List<SearchItemGroup> { new SearchItemGroup { GroupName = "Currency", SearchItems = currencySearchItems } };

                            List<SearchGuiItem> searchItems = new List<SearchGuiItem>();
                            foreach (var itemGroup in allItems.Concat(currencyGroup).Where(group => group != null))
                            {
                                foreach (var item in itemGroup.SearchItems.Where(item => !excludeList.Contains(item.Name)))
                                {
                                    var name = item.Name;
                                    var numDivines = item.Price / divinePrice;
                                    var priceInDivines = GetOfferPriceInDivines(numDivines);
                                    if (HighMarginMode)
                                        priceInDivines -= 0.5m;
                                    var priceInChaos = priceInDivines * divinePrice;

                                    decimal minProfitInChaos;
                                    switch (Strictness)
                                    {
                                        case Strictness.Chaos7:
                                            minProfitInChaos = 7;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Chaos15:
                                            minProfitInChaos = 15;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Chaos30:
                                            minProfitInChaos = 30;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div0p5:
                                            minProfitInChaos = divinePrice * 0.5m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div0p75:
                                            minProfitInChaos = divinePrice * 0.75m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div1:
                                            minProfitInChaos = divinePrice * 1.0m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div1p5:
                                            minProfitInChaos = divinePrice * 1.5m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div2:
                                            minProfitInChaos = divinePrice * 2.0m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                        case Strictness.Div4:
                                            minProfitInChaos = divinePrice * 4.0m;
                                            if (item.Price - priceInChaos < minProfitInChaos)
                                                priceInChaos = item.Price - minProfitInChaos;
                                            break;
                                    }

                                    if (priceInChaos < 1)
                                        continue;

                                    if (currencyCache.GetCurrency(CurrencyType.divine).Amount == 0 && priceInChaos / divinePrice > 2)
                                        continue;

                                    if (stashItems.Any(stashItem => stashItem.Name == item.Name))
                                        continue;

                                    var searchItem = new SearchGuiItem(item.Name)
                                    {
                                        BaseType = item.BaseType,
                                        OfferPrice = new Price(priceInChaos, Convert.ToInt32(divinePrice)),
                                        Rarity = item.Rarity,
                                        Links = item.Links,
                                        Variant = item.Variant,
                                        MapTier = item.MapTier,
                                        Veiled = veiledItems.Contains(item.Name),
                                        Source = item.Source,
                                        AllowCorrupted = item.AllowCorrupted
                                    };
                                    if (!searchItems.Contains(searchItem))
                                        searchItems.Add(searchItem);
                                }
                            }

                            itemPriceCache.SetItemPrices(searchItems);

                            List<SearchGuiItem> poeItems = new List<SearchGuiItem>();
                            logger.LogInformation($"Item dealfinder found {searchItems.Count} items");
                            if (searchItems.Count > LiveSearchLimit)
                            {
                                var chaosCount = currencyCache.GetChaosCount();
                                var affordableItems = searchItems.Where(item => item.OfferPrice.PriceInChaos(divinePrice) < chaosCount).OrderBy(item => item.OfferPrice.PriceInChaos(divinePrice)).ToList();
                                if (affordableItems.Count > LiveSearchLimit)
                                {
                                    poeItems.AddRange(affordableItems.OrderByDescending(item => item.OfferPrice.PriceInChaos(divinePrice)).Take(LiveSearchLimit));
                                }
                                else
                                {
                                    poeItems.AddRange(affordableItems);
                                    poeItems.AddRange(searchItems.OrderByDescending(item => item.OfferPrice.PriceInChaos(divinePrice)).Take(LiveSearchLimit - affordableItems.Count));
                                }
                            }
                            else
                            {
                                poeItems.AddRange(searchItems.OrderByDescending(item => item.OfferPrice.PriceInChaos(divinePrice)).Take(LiveSearchLimit));
                            }

                            await poeLiveSearchManager.UpdateLiveSearchesAsync(Strictness, League, poeItems.OrderByDescending(i => i.OfferPrice.PriceInChaos(divinePrice)));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Unexpected error in item dealfinder");
                        }
                        finally
                        {
                            await Task.Delay(TimeSpan.FromMinutes(2), token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Stopping ItemDealfinder");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error in item dealfinder");
                }
                logger.LogInformation("Dealfinder exiting");
            }, ctSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }
    }

    private decimal GetOfferPriceInDivines(decimal priceInDivines)
    {
        decimal profit;

        if (priceInDivines < 1)
        {
            profit = priceInDivines * 0.5m;
        }
        else if (priceInDivines < 2)
        {
            profit = 0.5m + (priceInDivines - 1) * 0.25m;
        }
        else if (priceInDivines < 5)
        {
            profit = 0.75m + (priceInDivines - 2) * (0.45m / 3);
        }
        else if (priceInDivines < 10)
        {
            profit = 1.2m + (priceInDivines - 5) * (0.8m / 5);
        }
        else if (priceInDivines < 40)
        {
            profit = 2m + (priceInDivines - 10) * (1m / 30);
        }
        else if (priceInDivines < 80)
        {
            profit = 3m + (priceInDivines - 40) * (1m / 40);
        }
        else if (priceInDivines < 200)
        {
            profit = priceInDivines * 0.05m;
        }
        else
        {
            profit = 10m;
        }

        return priceInDivines - profit;
    }


    public async Task StopAsync()
    {
        if (running)
        {
            await poeLiveSearchManager.StopAllLiveSearchesAsync();
            ctSource.Cancel();
            if (dealfinderTask != null)
                await dealfinderTask;
            running = false;
        }
    }

    private void AddCurrencySearchItem(List<SearchItem> searchItems, CurrencyType currencyType)
    {
        if (currencyPriceCache.ContainsPrice(currencyType))
            searchItems.Add(new SearchItem { Name = currencyType.GetCurrencyDescription(), Price = currencyPriceCache.GetPrice(currencyType).BuyPrice, Rarity = Rarity.Currency, Source = "CurrencyPriceCache" });
    }
}

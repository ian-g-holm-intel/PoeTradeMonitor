using System.Drawing;
using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.JSON;
using PoeLib.Parsers;
using PoeLib.Tools;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ViewModels;
using Microsoft.Extensions.Logging;

namespace PoeTradeMonitor.GUI.Services;

public class StashDataUpdater : IStashDataUpdater
{
    private MainWindowViewModel mainWindowViewModel;
    private readonly ItemPriceCache itemPriceCache;
    private readonly ISearchCriteriaMatcher searchCriteriaMatcher;
    private readonly ICurrencyCache currencyCache;
    private readonly ILogger<StashDataUpdater> logger;
    private ITradeRequestScheduler tradeRequestScheduler;

    public StashDataUpdater(ITradeRequestScheduler trs, ISearchCriteriaMatcher matcher, ICurrencyCache currency, ItemPriceCache itemCache, ILogger<StashDataUpdater> logger)
    {
        tradeRequestScheduler = trs;
        itemPriceCache = itemCache;
        currencyCache = currency;
        searchCriteriaMatcher = matcher;
        this.logger = logger;
    }

    public void SetViewModel(MainWindowViewModel vm)
    {
        mainWindowViewModel = vm;
    }

    public async Task UpdateStash(PoeItemSearchResult itemSearchResult)
    {
        var league = mainWindowViewModel.SelectedLeague;
        var divineRate = mainWindowViewModel.DivineRate;
        var dealfinderItems = itemPriceCache.GetAllItemPrices();
        var searchItems = mainWindowViewModel.SearchList.Where(s => s.Enabled).ToList();
        var combinedItems = searchItems.Union(dealfinderItems).OrderBy(item => item.ToString()).ToArray();

        var item = itemSearchResult.item;
        var x = itemSearchResult.listing.stash.x;
        var y = itemSearchResult.listing.stash.y;
        var itemName = item.Name;
        var account = itemSearchResult.listing.account.name;
        var character = itemSearchResult.listing.account.lastCharacterName;

        mainWindowViewModel.LastDataReceived = DateTime.Now;

        if (mainWindowViewModel.IgnoredAccounts.Contains(account))
            return;

        if (item.rarity == Rarity.Unique && !item.identified)
            return;

        if (itemSearchResult.listing.price == null)
            return;

        var price = new Price { Currencies = new List<Currency> { new Currency { Amount = itemSearchResult.listing.price.amount, Type = itemSearchResult.listing.price.currency } } };
        if (price.PriceInChaos(divineRate).Equals(0))
            return;

        foreach (var search in combinedItems.Where(s => !string.IsNullOrEmpty(s.SearchID) || item.Name.Contains(s.Name)))
        {
            try
            {
                if (searchCriteriaMatcher.MatchesCriteria(search, item, price, divineRate))
                {
                    var newItem = new StashGuiItem
                    {
                        Timestamp = $"{DateTime.Now:G}",
                        Name = $"{itemName}{(item.stackSize > 1 ? $" {item.stackSize}x" : "")}",
                        StackSize = item.stackSize,
                        NumSockets = item.Sockets.Count,
                        Price = price,
                        ItemLevel = item.ItemLevel,
                        ExplicitMods = item.ExplicitMods,
                        FracturedMods = item.FracturedMods,
                        Character = character,
                        Account = account,
                        Rarity = item.rarity,
                        Location = new Point(x, y),
                        Stash = itemSearchResult.listing.stash.name,
                        League = league,
                        ItemID = itemSearchResult.id,
                        Source = search.Source,
                        SearchID = item.SearchID,
                        WhisperToken = item.whisper_token,
                        ServiceLocation = mainWindowViewModel.ServiceLocation,
                        TradeRequest = new ItemTradeRequest { CharacterName = character, AccountName = account, Item = item, Price = price, Timestamp = DateTime.Now, DivineRate = divineRate }
                    };

                    if (string.IsNullOrEmpty(search.SearchID) && mainWindowViewModel.Strictness != Strictness.Chaos7 && price.PriceInChaos(divineRate) == 1)
                        return;
                     
                    if (!mainWindowViewModel.AddStashGuiItem(newItem))
                        return;

                    if ((price.PriceInChaos(divineRate) <= search.OfferPrice.PriceInChaos(divineRate) || !string.IsNullOrEmpty(search.SearchID)) && mainWindowViewModel.Running)
                    {
                        var currencies = currencyCache.ConvertDivFractions(newItem);
                        if (!currencyCache.EnoughCurrencyForTrade(currencies))
                        {
                            logger.LogWarning($"Not enough currency for trade: {newItem}");
                            continue;
                        }

                        await tradeRequestScheduler.ScheduleRequestAsync(newItem);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Unhandled exception in StashDataUpdater: {ex}", ex);
            }
        }
    }
}

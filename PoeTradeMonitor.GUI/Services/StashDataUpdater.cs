using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitor.GUI.ViewModels;
using Microsoft.Extensions.Logging;
using PoeTrade.Contracts;
using PoeLib.Common;
using PoeLib.Extensions;

namespace PoeTradeMonitor.GUI.Services;

public class StashDataUpdater : IStashDataUpdater
{
    private MainWindowViewModel? mainWindowViewModel;
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

    public async Task UpdateStash(TradeSearchResult itemSearchResult)
    {
        var divineRate = mainWindowViewModel!.DivineRate;
        var dealfinderItems = itemPriceCache.GetAllItemPrices();
        var searchItems = mainWindowViewModel.SearchList.Where(s => s.Enabled).ToList();
        var combinedItems = searchItems.Union(dealfinderItems).OrderBy(item => item.ToString()).ToArray();

        var item = itemSearchResult.Item;
        var listing = itemSearchResult.Listing;
        var price = listing.Price;
        var x = listing.Stash.X;
        var y = listing.Stash.Y;
        var itemName = item.Name;
        var account = listing.Account.Name;
        var character = listing.Account.LastCharacterName;

        mainWindowViewModel.LastDataReceived = DateTime.Now;

        if (mainWindowViewModel.IgnoredAccounts.Contains(account))
            return;

        if (price == null || (price.CurrencyType != TradeCurrencyType.Divine && price.CurrencyType != Constants.BaseCurrencyType))
            return;

        if (price.PriceInBaseCurrency(divineRate).Equals(0))
            return;

        foreach (var search in combinedItems.Where(s => !string.IsNullOrEmpty(s.SearchID) || item.Name.Contains(s.Name)))
        {
            try
            {
                if (searchCriteriaMatcher.MatchesCriteria(search, item, price, divineRate))
                {
                    var newItem = new StashGuiItem(new ItemTradeRequest(character, account, item, price, currencyCache.ConvertDivFractions(price, item.StackSize, divineRate), DateTime.Now, divineRate))
                    {
                        Timestamp = $"{DateTime.Now:G}",
                        Name = $"{itemName}{(item.StackSize > 1 ? $" {item.StackSize}x" : "")}",
                        StackSize = item.StackSize,
                        NumSockets = item.Sockets?.Count ?? 0,
                        Price = price,
                        ExplicitMods = item.ExplicitMods ?? new(),
                        FracturedMods = item.FracturedMods ?? new(),
                        Character = character,
                        Account = account,
                        Rarity = item.Rarity,
                        ItemID = item.Id,
                        Source = search.Source,
                        SearchID = item.SearchID,
                        WhisperToken = item.WhisperToken ?? string.Empty,
                        ServiceLocation = mainWindowViewModel.ServiceLocation
                    };

                    if (string.IsNullOrEmpty(search.SearchID) && price.PriceInBaseCurrency(divineRate) == 1)
                        return;
                     
                    if (!mainWindowViewModel.AddStashGuiItem(newItem))
                        return;

                    if (mainWindowViewModel.Running)
                    {
                        if (!currencyCache.EnoughCurrencyForTrade(newItem.Currencies))
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

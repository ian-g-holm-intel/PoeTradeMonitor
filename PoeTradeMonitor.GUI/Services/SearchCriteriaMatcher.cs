using PoeLib.Extensions;
using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.Services;

public class SearchCriteriaMatcher : ISearchCriteriaMatcher
{
    private readonly IPoePriceChecker poePriceChecker;

    public SearchCriteriaMatcher(IPoePriceChecker poePriceChecker)
    {
        this.poePriceChecker = poePriceChecker;
    }

    public bool MatchesCriteria(SearchGuiItem searchGuiItem, TradeItem item, PriceInfo price, decimal divineRate)
    {
        if (!searchGuiItem.Source.Equals("GUI"))
            return false;

        if (!item.SearchID.Equals(searchGuiItem.SearchID) && !item.SearchID.Equals(searchGuiItem.AutoSearchID))
            return false;

        if (!string.IsNullOrEmpty(searchGuiItem.SearchID) && price.PriceInDivine(divineRate) > searchGuiItem.OfferPrice.PriceInDivine(divineRate))
            return false;

        if (!string.IsNullOrEmpty(searchGuiItem.AutoSearchID))
        {
            var avgPrice = poePriceChecker.GetPrice(searchGuiItem.Name);
            if (avgPrice == 0) return false;
            if (price.PriceInDivine(divineRate) > (avgPrice - searchGuiItem.OfferPrice.PriceInDivine(divineRate)))
                return false;
        }

        if (searchGuiItem.IsPlusOneCorruption && (item.Properties == null || !item.Properties.Where(p => p.Values != null).SelectMany(p => p.Values!).Select(property => property.Value).Contains("+1 Level from Corruption")))
        {
            return false;
        }

        return true;
    }
}

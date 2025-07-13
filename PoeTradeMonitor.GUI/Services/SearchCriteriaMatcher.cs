using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.JSON;
using PoeLib.Parsers;
using PoeTradeMonitor.GUI.Interfaces;

namespace PoeTradeMonitor.GUI.Services;

public class SearchCriteriaMatcher : ISearchCriteriaMatcher
{
    public bool MatchesCriteria(SearchGuiItem searchItem, Item item, Price price, decimal divineRate)
    {
        if (searchItem.Source.Equals("GUI") &&
               searchItem.SearchID.Equals(item.SearchID) &&
               price.PriceInDivine(divineRate) <= searchItem.OfferPrice.PriceInDivine(divineRate))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(searchItem.Name) && !item.Name.Contains(searchItem.Name))
        { 
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because it does not match name: {searchItem.Name}");
            return false;
        }
        if (price.PriceInChaos(divineRate) < 1 || price.PriceInChaos(divineRate) > searchItem.OfferPrice.PriceInChaos(divineRate))
        {
            return false;
        }
        if (item.ILvlTooLow)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because ilvl is too low");
            return false;
        }
        if ((item.Name.Contains("Replica") && !searchItem.Name.Contains("Replica")) || (!item.Name.Contains("Replica") && searchItem.Name.Contains("Replica")))
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because replica name does not match name: {searchItem.Name}");
            return false;
        }
        if (!searchItem.AllowCorrupted && item.corrupted)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because corruption {item.corrupted} doe not match expected: {searchItem.Corrupted}");
            return false;
        }
        if (!string.IsNullOrEmpty(searchItem.BaseType) && item.BaseType != searchItem.BaseType)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because basetype {item.BaseType} doe not match expected: {searchItem.BaseType}");
            return false;
        }
        if (price.Currencies.SingleOrDefault(curr => curr.Type == CurrencyType.chaos) != null && price.Currencies.SingleOrDefault(curr => curr.Type == CurrencyType.chaos)?.Amount.GetFraction() > 0)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because price is in fractions of chaos");
            return false;
        }
        if (searchItem.Sockets != 0 && item.NumSockets < searchItem.Sockets)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because socket count {item.NumSockets} does not match expected: {searchItem.Sockets}");
            return false;
        }
        if (searchItem.Links != 0 && item.NumLinks < searchItem.Links)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because link count {item.NumLinks} does not match expected: {searchItem.Links}");
            return false;
        }
        if (searchItem.ItemLevel != 0 && item.ItemLevel < searchItem.ItemLevel)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because ilvl {item.ItemLevel} does not match expected: {searchItem.ItemLevel}");
            return false;
        }
        if (searchItem.Rarity != Rarity.Any && item.rarity != searchItem.Rarity)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because rarity {item.rarity} does not match expected: {searchItem.Rarity}");
            return false;
        }
        if (item.veiled != searchItem.Veiled)
        {
            //logger.LogWarning($"Rejecting {item.Name}({item.SearchID}) because veiled {item.veiled} does not match expected: {searchItem.Veiled}");
            return false;
        }
        
        return true;
    }
}

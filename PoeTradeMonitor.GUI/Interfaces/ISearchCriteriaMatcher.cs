using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface ISearchCriteriaMatcher
{
    bool MatchesCriteria(SearchGuiItem searchItem, TradeItem item, PriceInfo price, decimal divineRate);
}

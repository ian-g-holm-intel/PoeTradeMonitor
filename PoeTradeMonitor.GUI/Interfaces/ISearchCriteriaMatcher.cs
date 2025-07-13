using PoeLib.GuiDataClasses;
using PoeLib.JSON;
using PoeLib.Parsers;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface ISearchCriteriaMatcher
{
    bool MatchesCriteria(SearchGuiItem searchItem, Item item, Price price, decimal divineRate);
}

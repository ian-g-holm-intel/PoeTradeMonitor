using PoeLib.JSON;
using PoeLib.Trade;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemSearch
{
    Task<PoeItemSearchResponse> SearchAsync(string league, PoeItemSearchRequest request);
    Task<PoeItemSearchResults> FetchItemResults(IEnumerable<string> ids);
}

using PoeLib.Trade;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemSearchRequestCache : IDisposable
{
    Task<string> LookupId(string league, PoeItemSearchRequest request);
}

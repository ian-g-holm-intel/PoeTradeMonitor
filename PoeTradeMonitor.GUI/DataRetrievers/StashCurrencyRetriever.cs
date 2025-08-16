using Microsoft.Extensions.Logging;
using PoeTradeMonitor.GUI.Clients;
using PoeTrade.Contracts;
using System.Net.Http.Json;

namespace PoeTradeMonitor.GUI.DataRetrievers;

public interface IStashCurrencyRetriever
{
    Task<TradeItem[]> GetStashCurrency(string League);
}

public class StashCurrencyRetriever : IStashCurrencyRetriever
{
    private readonly IPoeHttpClient poeHttpClient;
    private readonly ILogger<StashCurrencyRetriever> log;

    public StashCurrencyRetriever(IPoeHttpClient poeHttpClient, ILogger<StashCurrencyRetriever> log)
    {
        this.poeHttpClient = poeHttpClient;
        this.log = log;
    }

    public async Task<TradeItem[]> GetStashCurrency(string league)
    {
        try
        {
            var response = await poeHttpClient.GetBackendRequest(league, 0);
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                log.LogError($"Failed to get currency info - StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return Array.Empty<TradeItem>();
            }
            var stashTabResponse = await response.Content.ReadFromJsonAsync<StashTabResponse>();
            return stashTabResponse?.Items.ToArray() ?? Array.Empty<TradeItem>();
        }
        catch(Exception ex)
        {
            log.LogError($"Failed to get currency info: {ex}");
            return Array.Empty<TradeItem>();
        }
    }
}

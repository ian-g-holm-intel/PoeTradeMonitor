using PoeLib.JSON;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PoeTradeMonitor.GUI.Clients;

namespace PoeTradeMonitor.GUI.DataRetrievers;

public interface IStashCurrencyRetriever
{
    Task<Item[]> GetStashCurrency(string League);
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

    public async Task<Item[]> GetStashCurrency(string league)
    {
        try
        {
            var response = await poeHttpClient.GetBackendRequest(league, 0);
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                log.LogError($"Failed to get currency info - StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return new Item[0];
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StashCurrency>(json).items.ToArray();
        }
        catch(Exception ex)
        {
            log.LogError($"Failed to get currency info: {ex}");
            return new Item[0];
        }
    }
}

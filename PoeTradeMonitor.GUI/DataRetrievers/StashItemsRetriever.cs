using PoeLib.JSON;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PoeTradeMonitor.GUI.Clients;

namespace PoeTradeMonitor.GUI.DataRetrievers;

public interface IStashItemsRetriever
{
    Task<Item[]> GetStashItems(string League);
}

public class StashItemsRetriever : IStashItemsRetriever
{
    private readonly IPoeHttpClient poeHttpClient;
    private readonly ILogger<StashItemsRetriever> logger;

    public StashItemsRetriever(IPoeHttpClient poeHttpClient, ILogger<StashItemsRetriever> logger)
    {
        this.poeHttpClient = poeHttpClient;
        this.logger = logger;
    }

    public async Task<Item[]> GetStashItems(string League)
    {
        try
        {
            var tabIndex = 0;
            switch (League)
            {
                case "Standard":
                    tabIndex = 1;
                    break;
                default:
                    tabIndex = 1;
                    break;
            }

            var response = await poeHttpClient.GetBackendRequest(League, tabIndex);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError($"Failed to get stash items - StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return new Item[0];
            }
            using var jsonStream = await response.Content.ReadAsStreamAsync();
            var stashItems = await JsonSerializer.DeserializeAsync<StashItems>(jsonStream);
            return stashItems.items.ToArray();
        }
        catch(Exception ex)
        {
            logger.LogError("FAILED TO GET STASH ITEMS: {ex}", ex);
            return new Item[0];
        }
    }
}

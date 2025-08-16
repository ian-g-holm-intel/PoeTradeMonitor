using Microsoft.Extensions.Logging;
using PoeTradeMonitor.GUI.Models;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Interfaces;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace PoeTradeMonitor.GUI.Services;

public class LiveSearchResultProcessor : ILiveSearchResultProcessor
{
    private readonly Serilog.ILogger itemLog;
    private readonly ProcessingPipeline itemProcessor;

    public LiveSearchResultProcessor(IPoeItemSearch poeItemSearch, IStashDataUpdater stashDataUpdater, ILogger<LiveSearchResultProcessor> logger, StatisticsManager statsManager)
    {
        itemLog = GetItemLog();
        itemProcessor = new ProcessingPipeline(poeItemSearch, stashDataUpdater, logger, statsManager, itemLog);
    }

    public void QueueItems(IEnumerable<ItemSearchRequest> items)
    {
        itemProcessor.QueueItems(items);
    }

    private Serilog.ILogger GetItemLog()
    {
#if DEBUG
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}";
#else
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";
#endif
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PoeTradeMonitor", "PoeItemLog.txt");

        var log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logPath, outputTemplate: logFormat, rollOnFileSizeLimit: true, fileSizeLimitBytes: 52428800, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();

        return log;
    }
}

public record ItemSearchRequest(string ID, string SearchID, DateTime ListingTime, SearchGuiItem Item);

public class ProcessingPipeline : IDisposable
{
    private readonly IPoeItemSearch poeItemSearch;
    private readonly IStashDataUpdater stashDataUpdater;
    private readonly ILogger<LiveSearchResultProcessor> logger;
    private readonly StatisticsManager statsManager;
    private readonly Serilog.ILogger itemLog;
    private static int retryCount = 2;

    private Stopwatch stopwatch = new();
    private BatchBlock<ItemSearchRequest> batchBlock;
    private ActionBlock<ItemSearchRequest[]> actionBlock;
    private bool disposedValue;

    public ProcessingPipeline(IPoeItemSearch poeItemSearch, IStashDataUpdater stashDataUpdater, ILogger<LiveSearchResultProcessor> logger, StatisticsManager statsManager, Serilog.ILogger itemLog)
    {
        this.poeItemSearch = poeItemSearch;
        this.stashDataUpdater = stashDataUpdater;
        this.logger = logger;
        this.statsManager = statsManager;
        this.itemLog = itemLog;

        batchBlock = new BatchBlock<ItemSearchRequest>(10);
        actionBlock = new ActionBlock<ItemSearchRequest[]>(ProcessResults);

        batchBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
    }

    public void QueueItems(IEnumerable<ItemSearchRequest> items)
    {
        foreach (var Item in items)
            batchBlock.Post(Item);

        if (actionBlock.InputCount == 0)
            batchBlock.TriggerBatch();
    }

    private async Task ProcessResults(ItemSearchRequest[] itemSearchRequests)
    {
        var searchIdDictionary = new Dictionary<string, ItemSearchRequest>();
        var searchGuiItemCount = new ConcurrentDictionary<SearchGuiItem, List<string>>();
        foreach (var itemSearchRequest in itemSearchRequests)
        {
            var items = searchGuiItemCount.GetOrAdd(itemSearchRequest.Item, new List<string>());
            items.Add(itemSearchRequest.ID);
            searchIdDictionary[itemSearchRequest.ID] = itemSearchRequest;
        }

        try
        {
            for (int i = 0; i < retryCount; i++)
            {
                stopwatch.Restart();
                var itemResults = await poeItemSearch.FetchItemResults(searchIdDictionary.Keys);
                stopwatch.Stop();

                if (itemResults == null)
                {
                    logger.LogError("Failed to retrieve Item search results");
                    return;
                }

                var itemsWithoutCharacterName = itemResults.Result.Where(Item => string.IsNullOrEmpty(Item.Listing.Account.LastCharacterName) || string.IsNullOrEmpty(Item.Listing.Account.Name));
                if (itemsWithoutCharacterName.Any())
                {
                    logger.LogWarning($"The Item {itemsWithoutCharacterName.First().Item.Name} did not have an account or character name, retrying");
                    await Task.Delay(1000);
                    continue;
                }
                else if (itemResults.Result.Count != itemSearchRequests.Length)
                {
                    logger.LogWarning($"The number of items search results ({itemResults.Result.Count}) does not match the number of search requests ({itemSearchRequests.Length}), retrying");
                    await Task.Delay(1000);
                    continue;
                }

                foreach (var result in itemResults.Result)
                {
                    if (result == null) continue;

                    result.Item.SearchID = searchIdDictionary[result.Id].SearchID;
                    result.Item.WhisperToken = result.Listing.WhisperToken;

                    LogItem(searchIdDictionary, result);
                    await stashDataUpdater.UpdateStash(result);
                }

                break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception during results processing: {ex}");
        }
        finally
        {
            foreach (var searchGuiItem in searchGuiItemCount.Keys)
                statsManager.LogProcessedItemsReceived(searchGuiItem, searchGuiItemCount[searchGuiItem].Count);

            batchBlock.TriggerBatch();
        }
    }

    private void LogItem(Dictionary<string, ItemSearchRequest> searchIdDictionary, TradeSearchResult result)
    {
        // Calculate elapsed times
        var listingTimeMs = (DateTime.Now - searchIdDictionary[result.Id].ListingTime).TotalMilliseconds;
        var totalElapsedMs = stopwatch.ElapsedMilliseconds;

        // Extract Item and account details
        var characterName = result.Listing.Account.LastCharacterName;
        var accountName = result.Listing.Account.Name;
        var itemName = result.Item.Name;
        var searchId = result.Item.SearchID;
        var price = result.Listing.Price;

        // Build Item mods strings
        string fracturedModsText = result.Item.FracturedMods != null && result.Item.FracturedMods.Any()
            ? result.Item.FracturedMods
                .Aggregate("", (current, mod) => current + Environment.NewLine + mod.RawModText)
                .TrimStart('\r', '\n')
            : string.Empty;

        string explicitModsText = result.Item.ExplicitMods != null && result.Item.ExplicitMods.Any()
            ? result.Item.ExplicitMods
                .Aggregate("", (current, mod) => current + Environment.NewLine + mod.RawModText)
                .TrimStart('\r', '\n')
            : string.Empty;

        // Construct the log message, conditionally including non-empty sections
        var logMessage = new List<string>
        {
            $"({listingTimeMs:F0}ms / {totalElapsedMs:F0}ms) {characterName}({accountName}) selling {itemName} - {searchId} - {price}:"
        };

        if (!string.IsNullOrEmpty(fracturedModsText))
            logMessage.Add(fracturedModsText);
        if (!string.IsNullOrEmpty(explicitModsText))
            logMessage.Add(explicitModsText);

        logMessage.Add("-------------------------------------------------------------------------------------------------------------");

        itemLog.Information(string.Join(Environment.NewLine, logMessage));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                batchBlock.Complete();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

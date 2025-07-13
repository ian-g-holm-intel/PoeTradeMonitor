using PoeTradeMonitor.GUI.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Serilog;
using System.IO;
using PoeLib.GuiDataClasses;
using System.Collections.Concurrent;
using PoeTradeMonitor.GUI.Clients;

namespace PoeTradeMonitor.GUI.Services;

public class LiveSearchResultProcessor : ILiveSearchResultProcessor
{
    private readonly Serilog.ILogger itemLog;
    private readonly ProcessingPipeline itemProcessor;

    public LiveSearchResultProcessor(IPoeHttpClient poeHttpClient, IPoeItemSearch poeItemSearch, IStashDataUpdater stashDataUpdater, ILogger<LiveSearchResultProcessor> logger, StatisticsManager statsManager)
    {
        itemLog = GetItemLog();
        itemProcessor = new ProcessingPipeline(poeHttpClient, poeItemSearch, stashDataUpdater, logger, statsManager, itemLog);
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

public class ItemSearchRequest
{
    public string ID { get; set; }
    public string SearchID { get; set; }
    public DateTime ListingTime { get; set; }
    public SearchGuiItem SearchGuiItem { get; set; }

    public override bool Equals(object obj)
    {
        return obj is ItemSearchRequest request &&
               ID == request.ID &&
               SearchID == request.SearchID &&
               ListingTime == request.ListingTime;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, SearchID, ListingTime);
    }
}

public class ProcessingPipeline : IDisposable
{
    private readonly IPoeItemSearch poeItemSearch;
    private readonly IStashDataUpdater stashDataUpdater;
    private readonly ILogger<LiveSearchResultProcessor> logger;
    private readonly StatisticsManager statsManager;
    private readonly Serilog.ILogger itemLog;
    private readonly IPoeHttpClient poeHttpClient;
    private static int retryCount = 2;

    private Stopwatch stopwatch = new();
    private BatchBlock<ItemSearchRequest> batchBlock;
    private ActionBlock<ItemSearchRequest[]> actionBlock;
    private bool disposedValue;
    
    public ProcessingPipeline(IPoeHttpClient poeHttpClient, IPoeItemSearch poeItemSearch, IStashDataUpdater stashDataUpdater, ILogger<LiveSearchResultProcessor> logger, StatisticsManager statsManager, Serilog.ILogger itemLog)
    {
        this.poeHttpClient = poeHttpClient;
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
        foreach (var item in items)
            batchBlock.Post(item);

        if (actionBlock.InputCount == 0)
            batchBlock.TriggerBatch();
    }

    private async Task ProcessResults(ItemSearchRequest[] itemSearchRequests)
    {
        var searchIdDictionary = new Dictionary<string, ItemSearchRequest>();
        var searchGuiItemCount = new ConcurrentDictionary<SearchGuiItem, List<string>>();
        foreach (var itemSearchRequest in itemSearchRequests)
        {
            var items = searchGuiItemCount.GetOrAdd(itemSearchRequest.SearchGuiItem, new List<string>());
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
                    logger.LogError("Failed to retrieve item search results");
                    return;
                }

                var itemsWithoutCharacterName = itemResults.result.Where(item => string.IsNullOrEmpty(item.listing.account.lastCharacterName) || string.IsNullOrEmpty(item.listing.account.name));
                if (itemsWithoutCharacterName.Any())
                {
                    logger.LogWarning($"The item {itemsWithoutCharacterName.First().item.Name} did not have an account or character name, retrying");
                    await Task.Delay(1000);
                    continue;
                }
                else if (itemResults.result.Count != itemSearchRequests.Length)
                {
                    logger.LogWarning($"The number of items search results ({itemResults.result.Count}) does not match the number of search requests ({itemSearchRequests.Length}), retrying");
                    await Task.Delay(1000);
                    continue;
                }

                foreach (var result in itemResults.result)
                {
                    result.item.SearchID = searchIdDictionary[result.id].SearchID;
                    result.item.whisper_token = result.listing.whisper_token;

                    itemLog.Information($"({(DateTime.Now - searchIdDictionary[result.id].ListingTime).TotalMilliseconds}ms / {stopwatch.ElapsedMilliseconds}ms) {result.listing.account.lastCharacterName}({result.listing.account.name}) selling {result.item.Name} - {result.item.SearchID} - {result.listing.price}:" + Environment.NewLine + result.item.ExplicitMods.Aggregate("", (current, mod) => current + Environment.NewLine + mod.RawModText).TrimStart('\r', '\n') + Environment.NewLine + "-------------------------------------------------------------------------------------------------------------");
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

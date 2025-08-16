using System.Collections.Concurrent;
using System.Media;
using Microsoft.Extensions.Logging;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Clients;
using PoeTradeMonitor.GUI.Interfaces;
using PoeLib.Common;
using PoeLib.Extensions;

namespace PoeTradeMonitor.GUI.Services;

public class TradeRequestScheduler : ITradeRequestScheduler
{
    private readonly ILogger<TradeRequestScheduler> logger;
    private readonly IPoeHttpClient poeHttpClient;
    private readonly IPartyManagerClient partyManager;
    private readonly ITradeBotClient tradeBot;
    private readonly ICurrencyCache currencyCache;
    private readonly ConcurrentDictionary<string, TradeRequestTask> tradeRequestTasks;
    private delegate void TradeRequestComplete(string characterName);

    private ServiceLocation lastLocation = ServiceLocation.Local;
    private bool UnattendedEnabled { get; set; }
    public bool AlertsEnabled { get; set; }

    public TradeRequestScheduler(IPoeHttpClient poeHttpClient, IPartyManagerClient partyManager, ITradeBotClient tradeBot, ICurrencyCache currencyCache, ILogger<TradeRequestScheduler> logger)
    {
        this.poeHttpClient = poeHttpClient;
        this.partyManager = partyManager;
        this.tradeBot = tradeBot;
        this.currencyCache = currencyCache;
        this.logger = logger;
        tradeRequestTasks = new ConcurrentDictionary<string, TradeRequestTask>();
    }

    public async Task UpdateSettings(bool unattendedEnabled, ServiceLocation serviceLocation)
    {
        UnattendedEnabled = unattendedEnabled;
        if (!UnattendedEnabled)
        {
            if (await partyManager.Stop(serviceLocation.ToString()))
                logger.LogInformation("Stopped party manager");
        }
    }

    public Task ScheduleRequestAsync(StashGuiItem stashGuiItem)
    {
        //if (tradeRequestTasks.Values.Any(tradeRequest => tradeRequest.InTrade))
        //{
        //    logger.LogInformation($"Skipping trade for {stashGuiItem} because we're in a trade already");
        //    return Task.CompletedTask;
        //}

        if (string.IsNullOrEmpty(stashGuiItem.Character) || string.IsNullOrEmpty(stashGuiItem.Account))
        {
            logger.LogInformation($"Skipping trade for {stashGuiItem} because it's missing a character or account name");
            return Task.CompletedTask;
        }

        return Task.Run(async () =>
        {
            if (tradeRequestTasks.ContainsKey(stashGuiItem.Account)) return;
            tradeRequestTasks[stashGuiItem.Account] = new TradeRequestTask(this, tradeBot, currencyCache, logger, stashGuiItem.ServiceLocation.ToString());
            lastLocation = stashGuiItem.ServiceLocation;

            if (!string.IsNullOrEmpty(stashGuiItem.WhisperToken))
                poeHttpClient.SendTradeWhisperFromToken(stashGuiItem.SearchID, stashGuiItem.WhisperToken, stashGuiItem.WhisperValue);
            else
                await poeHttpClient.SendTradeWhisper(stashGuiItem.SearchID, stashGuiItem.ItemID, stashGuiItem.WhisperValue);

            if (UnattendedEnabled)
            {
                await partyManager.Start(stashGuiItem.Account, stashGuiItem.Character, Convert.ToInt32(stashGuiItem.TradeRequest.Price.PriceInBaseCurrency(stashGuiItem.TradeRequest.DivineRate)), stashGuiItem.ServiceLocation.ToString());
            }

            tradeRequestTasks[stashGuiItem.Account].RequestComplete += async accountName => await RemoveTask(accountName, stashGuiItem.ServiceLocation.ToString());
            tradeRequestTasks[stashGuiItem.Account].StartWaitingForResponse(stashGuiItem);
            logger.LogInformation($"Scheduled request for {stashGuiItem}");
        });
    }

    public async Task JoinedParty(string account, string characterName)
    {
        if (tradeRequestTasks.ContainsKey(account))
            tradeRequestTasks[account].JoinedParty(account, characterName);
        else
            await partyManager.LeaveParty(lastLocation.ToString());
    }

    public void TradeComplete(string account)
    {
        if (tradeRequestTasks.ContainsKey(account))
            tradeRequestTasks[account].TradeComplete(account);
    }

    private async Task RemoveTask(string accountName, string clientName)
    {
        if (tradeRequestTasks.TryRemove(accountName, out var tradeRequestTask))
            tradeRequestTask.Dispose();
        if (tradeRequestTasks.IsEmpty)
            await partyManager.Stop(clientName);
    }

    private class TradeRequestTask : IDisposable
    {
        private static readonly SoundPlayer alertPlayer = new SoundPlayer("alert.wav");
        private readonly ILogger taskLog;
        private readonly string clientName;
        private readonly TradeRequestScheduler scheduler;
        private readonly ITradeBotClient tradeBot;
        private readonly ICurrencyCache currencyCache;
        private readonly ManualResetEvent receivedInvite;
        private readonly ManualResetEvent tradeComplete;
        private readonly CancellationTokenSource ctSource;
        public event TradeRequestComplete? RequestComplete;
        private StashGuiItem? Item { get; set; }
        public bool InTrade { get; set; }

        public TradeRequestTask(TradeRequestScheduler scheduler, ITradeBotClient tradeBot, ICurrencyCache currencyCache, ILogger log, string clientName)
        {
            this.scheduler = scheduler;
            this.tradeBot = tradeBot;
            this.currencyCache = currencyCache;
            taskLog = log;
            this.clientName = clientName;
            receivedInvite = new ManualResetEvent(false);
            tradeComplete = new ManualResetEvent(false);
            ctSource = new CancellationTokenSource(120000);
        }

        public void JoinedParty(string account, string characterName)
        {
            taskLog.LogInformation($"Account: {account}, Character: {characterName} - Joined party");
            if (Item?.TradeRequest != null)
                Item.TradeRequest.CharacterName = characterName;
            receivedInvite.Set();
        }

        public void TradeComplete(string account)
        {
            taskLog.LogInformation($"Account: {account} - Trade complete");
            tradeComplete.Set();
        }

        public void StartWaitingForResponse(StashGuiItem item)
        {
            Item = item;
            _ = Task.Run(async () =>
            {
                try
                {
                    if (scheduler.UnattendedEnabled)
                    {
                        await receivedInvite.WaitOneAsync(ctSource.Token);
                        taskLog.LogInformation($"Party invite received from: {Item.Character}");
                        HandleAlert(item);
                        taskLog.LogInformation($"Queuing trade request: {Item}");
                        if (Item?.TradeRequest != null)
                            await tradeBot.QueueTrade(Item.TradeRequest, clientName);
                        InTrade = true;
                    }
                    else
                    {
                        if (await tradeBot.CheckPartyInviteAsync(Item.Account, 120000, clientName))
                        {
                            item.ExecuteEnabled = true;
                            HandleAlert(item);
                        }
                    }

                    await tradeComplete.WaitOneAsync(ctSource.Token);
                    taskLog.LogInformation($"Trade request complete: {Item?.Character ?? "Unknown"}");
                }
                catch (OperationCanceledException)
                {
                    taskLog.LogInformation($"Trade request timed out: {Item.Character}");
                }
                catch (Exception ex)
                {
                    taskLog.LogError("Unhandled exception in TradeRequestTask: {char} - {ex}", Item.Character, ex);
                }

                RequestComplete?.Invoke(Item?.Account ?? "Unknown");
            }, ctSource.Token);
        }

        private void HandleAlert(StashGuiItem item)
        {
            if (scheduler.AlertsEnabled && currencyCache.EnoughCurrencyForTrade(item.Currencies))
            {
                // Alert handling logic can be added here
            }
                alertPlayer.Play();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ctSource?.Dispose();
                }

                disposed = true;
            }
        }
        #endregion
    }
}

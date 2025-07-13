using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoeLib;
using PoeLib.Tools;
using System.Linq;
using TradeBotLib;
using System.Collections.Generic;
using PoeTradeMonitor.Service.Interfaces;
using Microsoft.Extensions.Hosting;
using PoeTradeMonitor.Services.Interfaces;

namespace PoeTradeMonitor.Service.Services;

public interface ITradeBot : IHostedService
{
    bool IsExecutingTrade { get; set; }
    void QueueTradeRequest(ItemTradeRequest tradeRequest);
}

public class TradeBot : ITradeBot
{
    private readonly ILogger<TradeBot> log;
    private readonly ITradeCommands tradeCommands;
    private readonly ITradeExecutorService tradeExecutor;
    private readonly IPoeChatWatcher poeChatWatcher;
    private readonly INotificationClient notificationClient;
    private readonly ICallbackClient callback;
    private List<ItemTradeRequest> itemTradeQueue = new List<ItemTradeRequest>();
    private readonly Stopwatch itemTradeTimer = new Stopwatch();
    private Task tradeLoopTask;
    private CancellationTokenSource ctSource;
    private readonly object queueLock = new object();
    public bool IsExecutingTrade { get; set; }


    public TradeBot(ITradeCommands tc, IPoeChatWatcher chatWater, ITradeExecutorService te, INotificationClient client, ICallbackClient callback, ILogger<TradeBot> log)
    {
        tradeCommands = tc;
        poeChatWatcher = chatWater;
        tradeExecutor = te;
        notificationClient = client;
        this.callback = callback;
        this.log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Initialize();

        ctSource = new CancellationTokenSource();
        tradeLoopTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                log.LogInformation($"Waiting for trade requests");
                while (!ctSource.Token.IsCancellationRequested)
                {
                    if (DequeueMostValuable(out var tradeRequest))
                    {
                        IsExecutingTrade = true;
                        log.LogInformation($"Executing Trade Request: {tradeRequest}");
                        await tradeExecutor.ExecuteItemTrade(tradeRequest, ctSource.Token);
                        itemTradeQueue.Clear();
                        IsExecutingTrade = false;
                        await callback.CompletedTradeAsync(tradeRequest.AccountName);
                    }
                    else
                    {
                        await Task.Delay(100, ctSource.Token);
                    }
                }

                log.LogInformation("TradeLoop shutting down");
            }
            catch (OperationCanceledException)
            {
                log.LogInformation("TradeLoop shutting down");
            }
            catch (Exception ex)
            {
                log.LogError($"Unexpected error in TradeLoop: {ex}");
                await notificationClient.SendPushNotification("ERROR", "Error", "Error in TradeLoop");
            }
        }, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    public void QueueTradeRequest(ItemTradeRequest tradeRequest)
    {
        lock(queueLock)
        {
            itemTradeQueue.Add(tradeRequest);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        log.LogInformation("TradeBot shutting down");
        ctSource?.Cancel();
        await tradeLoopTask;
    }

    private void Initialize()
    {
        tradeCommands.Initialize();
        poeChatWatcher.MessageFromCharacter += OnMessageFromCharacter;
        poeChatWatcher.AutoReplyMessageReceived += PoeChatWatcherOnAutoReplyMessageReceived;
        itemTradeTimer.Start();
        log.LogInformation("TradeBot initialization complete");
    }

    private async void PoeChatWatcherOnAutoReplyMessageReceived(string characterName, string reply)
    {
        await tradeCommands.SendMessage(characterName, reply, true);
    }

    private void OnMessageFromCharacter(CharacterMessage message)
    {
        log.LogInformation($"Message from {message.Character}: {message.Message}");
        notificationClient.SendPushNotification("Message", message.Character, message.Message);
    }

    private bool DequeueMostValuable(out ItemTradeRequest tradeRequest)
    {
        tradeRequest = null;
        if (itemTradeQueue.Count == 0)
            return false;

        lock (queueLock)
        {
            tradeRequest = itemTradeQueue.OrderByDescending(item => item.Price.PriceInChaos(item.DivineRate)).First();
            itemTradeQueue.Remove(tradeRequest);
            return true;
        }
    }
}

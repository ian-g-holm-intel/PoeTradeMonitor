using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using PoeLib;
using PoeLib.Proto;
using PoeLib.Tools;
using PoeTradeMonitorProto;
using Microsoft.Extensions.Logging;
using TradeBotLib;

namespace PoeTradeMonitor.Service.Services;

public class PoeProxyService : PoeProxy.PoeProxyBase
{
    private readonly ITradeCommands tradeCommands;
    private readonly IChatMessageCache messageCache;
    private readonly INotificationClient notificationClient;
    private readonly IPoeChatWatcher chatWatcher;
    private readonly ILogger<PoeProxyService> log;
    private ConcurrentBag<PoeLib.ItemTradeRequest> tradeRequestList = new ConcurrentBag<PoeLib.ItemTradeRequest>();

    public PoeProxyService(ITradeCommands tradeCommands, IChatMessageCache messageCache, INotificationClient notificationClient, IPoeChatWatcher chatWatcher, ILogger<PoeProxyService> log)
    {
        this.tradeCommands = tradeCommands;
        this.messageCache = messageCache;
        this.notificationClient = notificationClient;
        this.chatWatcher = chatWatcher;
        this.log = log;
    }

    public override async Task<AddCharacterMessageReply> AddCharacterMessage(AddCharacterMessageRequest request, ServerCallContext context)
    {
        var message = request.Message.FromProto();
        await notificationClient.SendPushNotification("Message", message.Character, message.Message);
        messageCache.AddMessage(message);
        return new AddCharacterMessageReply();
    }

    public override async Task<AddTradeRequestReply> AddTradeRequest(AddTradeRequestRequest request, ServerCallContext context)
    {
        var tradeRequest = request.TradeRequest.FromProto();
        try
        {
            log.LogInformation($"Adding trade request: {tradeRequest}");
            await notificationClient.SendPushNotification("Trade", tradeRequest.CharacterName, tradeRequest.ToString(), "complete.caf");
            tradeRequestList.Add(tradeRequest);
        }
        catch (Exception ex)
        {
            log.LogError("Got error sending trade request: {ex}", ex);
        }
        return new AddTradeRequestReply();
    }

    public override async Task<AntiAFKReply> AntiAFK(AntiAFKRequest request, ServerCallContext context)
    {
        await tradeCommands.AntiAFK().StartSTATask();
        return new AntiAFKReply();
    }

    public override async Task<CheckPartyInviteReply> CheckPartyInvite(CheckPartyInviteRequest request, ServerCallContext context)
    {
        log.LogInformation("CheckPartyInvite: {timeoutMs}ms", request.Timeout);
        var inviteReceived = false;
        using (var ctSource = new CancellationTokenSource(request.Timeout))
        {
            try
            {
                if (!await tradeCommands.WaitPartyRequest(request.AccountName, ctSource.Token))
                {
                    log.LogWarning("Timed out waiting for party invite from: {account}", request.AccountName);
                }

                log.LogInformation("Got party invite");
                inviteReceived = true;
            }
            catch (OperationCanceledException)
            {
                log.LogWarning("Timed out waiting for party invite from: {account}", request.AccountName);
            }
            catch (Exception ex)
            {
                log.LogError("Got error waiting for party invite: {ex}", ex);
            }
        }
        return new CheckPartyInviteReply { InviteReceived = inviteReceived };
    }

    public override async Task<SendMessageReply> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        try
        {
            log.LogInformation($"Sending command: {request.Message}");
            await tradeCommands.SendTextCommand(request.Message).StartSTATask();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception in SendMessage");
        }
        return new SendMessageReply();
    }

    public override async Task<SendPushNotificationReply> SendPushNotification(SendPushNotificationRequest request, ServerCallContext context)
    {
        await notificationClient.SendPushNotification(request.Title, request.Subtitle, request.Body, request.Sound);
        return new SendPushNotificationReply();
    }

    public override Task<SetAutoReplyReply> SetAutoReply(SetAutoReplyRequest request, ServerCallContext context)
    {
        log.LogInformation($"SetAutoReply: {request.Enabled}");
        chatWatcher.AutoReplyEnabled = request.Enabled;
        return Task.FromResult(new SetAutoReplyReply());
    }
}

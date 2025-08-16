using Grpc.Core;
using PoeLib.Tools;
using PoeLib.Common;
using PoeLib.Extensions;
using PoeLib.Proto;

namespace PoeTradeMonitor.Service.Services;

/// <summary>
/// gRPC service implementation for trade bot operations.
/// Provides remote access to trading functionality via gRPC calls.
/// </summary>
public class TradeBotService : PoeLib.Proto.TradeBot.TradeBotBase
{
    private readonly ILogger<TradeBotService> log;
    private readonly ITradeBot tradeBot;
    private readonly ITradeCommands tradeCommands;
    private readonly IChatMessageCache messageCache;
    private readonly INotificationClient notificationClient;
    private readonly IPoeChatWatcher chatWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeBotService"/> class.
    /// </summary>
    /// <param name="log">Logger for the trade bot service.</param>
    /// <param name="tradeBot">The trade bot implementation to use.</param>
    /// <param name="tradeCommands">Trade commands service for game interaction.</param>
    /// <param name="messageCache">Message cache for character messages.</param>
    /// <param name="notificationClient">Client for sending push notifications.</param>
    /// <param name="chatWatcher">Chat watcher for monitoring game chat.</param>
    public TradeBotService(ILogger<TradeBotService> log, ITradeBot tradeBot, ITradeCommands tradeCommands, IChatMessageCache messageCache, INotificationClient notificationClient, IPoeChatWatcher chatWatcher)
    {
        this.log = log;
        this.tradeBot = tradeBot;
        this.tradeCommands = tradeCommands;
        this.messageCache = messageCache;
        this.notificationClient = notificationClient;
        this.chatWatcher = chatWatcher;
    }

    /// <summary>
    /// Queues a trade request for execution by the trade bot.
    /// </summary>
    /// <param name="request">The trade request to queue.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
    public override Task<QueueTradeReply> QueueTrade(QueueTradeRequest request, ServerCallContext context)
    {
        var tradeRequest = request.TradeRequest.FromProto();
        log.LogInformation($"Queuing Trade Request: {tradeRequest}");
        tradeBot.QueueTradeRequest(tradeRequest);
        return Task.FromResult(new QueueTradeReply());
    }

    /// <summary>
    /// Gets the current execution status of the trade bot.
    /// </summary>
    /// <param name="request">The status request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply containing the current execution status.</returns>
    public override Task<GetTradeStatusReply> GetTradeStatus(GetTradeStatusRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetTradeStatusReply { ExecutingTrade = tradeBot.IsExecutingTrade });
    }

    /// <summary>
    /// Adds a character message to the cache and sends a push notification.
    /// </summary>
    /// <param name="request">The character message request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
    public override async Task<AddCharacterMessageReply> AddCharacterMessage(AddCharacterMessageRequest request, ServerCallContext context)
    {
        var message = request.Message.FromProto();
        await notificationClient.SendPushNotification("Message", message.Character, message.Message);
        messageCache.AddMessage(message);
        return new AddCharacterMessageReply();
    }

    /// <summary>
    /// Performs anti-AFK action in the game.
    /// </summary>
    /// <param name="request">The anti-AFK request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
    public override async Task<AntiAFKReply> AntiAFK(AntiAFKRequest request, ServerCallContext context)
    {
        await tradeCommands.AntiAFK().StartSTATask();
        return new AntiAFKReply();
    }

    /// <summary>
    /// Checks for party invite from specified account with timeout.
    /// </summary>
    /// <param name="request">The party invite check request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating whether invite was received.</returns>
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

    /// <summary>
    /// Sends a text command to the game.
    /// </summary>
    /// <param name="request">The message to send.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
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

    /// <summary>
    /// Sends a push notification.
    /// </summary>
    /// <param name="request">The push notification request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
    public override async Task<SendPushNotificationReply> SendPushNotification(SendPushNotificationRequest request, ServerCallContext context)
    {
        await notificationClient.SendPushNotification(request.Title, request.Subtitle, request.Body, request.Sound);
        return new SendPushNotificationReply();
    }

    /// <summary>
    /// Sets the auto-reply status for the chat watcher.
    /// </summary>
    /// <param name="request">The auto-reply setting request.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <returns>A reply indicating the operation completed.</returns>
    public override Task<SetAutoReplyReply> SetAutoReply(SetAutoReplyRequest request, ServerCallContext context)
    {
        log.LogInformation($"SetAutoReply: {request.Enabled}");
        chatWatcher.AutoReplyEnabled = request.Enabled;
        return Task.FromResult(new SetAutoReplyReply());
    }
}

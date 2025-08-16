using PoeLib.Proto;
using PoeLib.Extensions;
using PoeTradeMonitor.GUI.Interfaces;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Grpc.Net.ClientFactory;

namespace PoeTradeMonitor.GUI.Clients;

public class TradeBotClient : ITradeBotClient
{
    private GrpcClientFactory clientFactory;
    private ILogger<TradeBotClient> logger;

    public TradeBotClient(ILogger<TradeBotClient> logger, GrpcClientFactory clientFactory)
    {
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task QueueTrade(PoeLib.Common.ItemTradeRequest tradeRequest, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.QueueTradeAsync(new QueueTradeRequest() { TradeRequest = tradeRequest.ToProto() });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
        }
    }

    public async Task<bool> ExecutingTrade(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            var response = await client.GetTradeStatusAsync(new GetTradeStatusRequest());
            return response.ExecutingTrade;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
            return false;
        }
    }

    public async Task AddCharacterMessageAsync(PoeLib.Common.CharacterMessage message, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.AddCharacterMessageAsync(new AddCharacterMessageRequest() { Message = message.ToProto() });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
        }
    }

    public async Task SendMessageAsync(string message, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.SendMessageAsync(new SendMessageRequest() { Message = message });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
        }
    }

    public async Task AntiAFK(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.AntiAFKAsync(new AntiAFKRequest());
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
        }
    }

    public async Task<bool> CheckPartyInviteAsync(string accountName, int timeoutMs, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            var response = await client.CheckPartyInviteAsync(new CheckPartyInviteRequest() { AccountName = accountName, Timeout = timeoutMs });
            return response.InviteReceived;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
            return false;
        }
    }

    public async Task SetAutoReplyAsync(bool enabled, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.SetAutoReplyAsync(new SetAutoReplyRequest() { Enabled = enabled });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to TradeBot");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to TradeBot");
        }
    }
}

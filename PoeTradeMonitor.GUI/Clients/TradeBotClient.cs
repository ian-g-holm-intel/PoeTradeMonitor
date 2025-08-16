using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;
using PoeLib.Extensions;
using PoeLib.Proto;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Interfaces;

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

    public async Task<Dictionary<TradeCurrencyType, CurrencyInfo>> GetCurrencyAsync(string clientName)
    {
        var request = new GetCurrencyRequest();
        var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
        var response = await client.GetCurrencyAsync(request);

        var result = new Dictionary<TradeCurrencyType, CurrencyInfo>();

        foreach (var kvp in response.Currencies)
        {
            if (Enum.TryParse<TradeCurrencyType>(kvp.Key.ToString(), out var keyType) &&
                Enum.TryParse<TradeCurrencyType>(kvp.Value.Type.ToString(), out var valueType))
            {
                result[keyType] = new CurrencyInfo
                {
                    Type = valueType,
                    Amount = Convert.ToDecimal(kvp.Value.Amount)
                };
            }
        }

        return result;
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

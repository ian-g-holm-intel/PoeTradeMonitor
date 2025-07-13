using PoeLib.Proto;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitorProto;
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

    public async Task QueueTrade(PoeLib.ItemTradeRequest tradeRequest, string clientName)
    {
        var itemTradeRequest = new ItemTradeRequest()
        {
            CharacterName = tradeRequest.CharacterName,
            AccountName = tradeRequest.AccountName,
            Price = tradeRequest.Price.ToProtoPrice(),
            Timestamp = tradeRequest.Timestamp.ToBinary(),
            Item = tradeRequest.Item.ToProtoItem(),
            DivineRate = Convert.ToDouble(tradeRequest.DivineRate)
        };

        try
        {
            var client = clientFactory.CreateClient<TradeBot.TradeBotClient>($"{clientName}{typeof(TradeBot.TradeBotClient).Name}");
            await client.QueueTradeAsync(new QueueTradeRequest() { TradeRequest = itemTradeRequest });
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
}

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using PoeTradeMonitorProto;
using PoeLib.Proto;

namespace PoeTradeMonitor.Service.Services;

public class TradeBotService : PoeTradeMonitorProto.TradeBot.TradeBotBase
{
    private readonly ILogger<TradeBotService> log;
    private readonly ITradeBot tradeBot;

    public TradeBotService(ILogger<TradeBotService> log, ITradeBot tradeBot)
    {
        this.log = log;
        this.tradeBot = tradeBot;
    }

    public override Task<QueueTradeReply> QueueTrade(QueueTradeRequest request, ServerCallContext context)
    {
        var tradeRequest = request.TradeRequest.FromProto();
        log.LogInformation($"Queuing Trade Request: {tradeRequest}");
        tradeBot.QueueTradeRequest(tradeRequest);
        return Task.FromResult(new QueueTradeReply());
    }

    public override Task<GetTradeStatusReply> GetTradeStatus(GetTradeStatusRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetTradeStatusReply { ExecutingTrade = tradeBot.IsExecutingTrade });
    }
}

using Grpc.Core;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitorProto;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;

namespace PoeTradeMonitor.GUI.Services;

public class CallbackService : Callback.CallbackBase
{
    private readonly ILogger<CallbackService> logger;
    private readonly ITradeRequestScheduler tradeRequestScheduler;

    public CallbackService(ITradeRequestScheduler tradeRequestScheduler, ILogger<CallbackService> logger)
    {
        this.tradeRequestScheduler = tradeRequestScheduler;
        this.logger = logger;
    }

    public override async Task<Empty> JoinedParty(JoinedPartyRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received joined party callback");
        try
        {
            await tradeRequestScheduler.JoinedParty(request.Account, request.CharacterName);
        }
        catch (Exception ex)
        {
            logger.LogError($"Unhandled exception in JoinedParty: {ex}");
        }
        return new Empty();
    }

    public override Task<Empty> CompletedTrade(CompletedTradeRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received completed trade callback");
        try
        {
            tradeRequestScheduler.TradeComplete(request.Account);
        }
        catch (Exception ex)
        {
            logger.LogError($"Unhandled exception in CompletedTrade: {ex}");
        }
        return Task.FromResult(new Empty());
    }
}

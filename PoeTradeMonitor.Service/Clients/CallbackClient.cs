using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PoeTradeMonitor.Services.Interfaces;
using PoeTradeMonitorProto;

namespace PoeTradeMonitor.Service.Clients;

public class CallbackClient : ICallbackClient
{
    private Callback.CallbackClient client;
    private ILogger<CallbackClient> logger;

    public CallbackClient(ILogger<CallbackClient> logger, Callback.CallbackClient client)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task JoinedPartyAsync(string account, string characterName)
    {
        var request = new JoinedPartyRequest { Account = account, CharacterName = characterName };

        try
        {
            await client.JoinedPartyAsync(request);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to CallbackService");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to CallbackService");
        }
    }

    public async Task CompletedTradeAsync(string account)
    {
        var request = new CompletedTradeRequest { Account = account };

        try
        {
            await client.CompletedTradeAsync(request);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to CallbackService");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to CallbackService");
        }
    }
}

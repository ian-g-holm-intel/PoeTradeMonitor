using Grpc.Core;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitorProto;
using Microsoft.Extensions.Logging;
using Grpc.Net.ClientFactory;

namespace PoeTradeMonitor.GUI.Clients;

public class PartyManagerClient : IPartyManagerClient
{
    private GrpcClientFactory clientFactory;
    private ILogger<PartyManagerClient> logger;

    public PartyManagerClient(ILogger<PartyManagerClient> logger, GrpcClientFactory clientFactory)
    {
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task<bool> Start(string accountName, string characterName, int value, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PartyManager.PartyManagerClient>($"{clientName}{typeof(PartyManager.PartyManagerClient).Name}");
            var response = await client.StartAsync(new StartRequest { AccountName = accountName, CharacterName = characterName, Value = value });
            return response.Started;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PartyManager");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unhandled exception in PartyManager");
            return false;
        }
    }

    public async Task<bool> Stop(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PartyManager.PartyManagerClient>($"{clientName}{typeof(PartyManager.PartyManagerClient).Name}");
            var response = await client.StopAsync(new StopRequest());
            return response.Stopped;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PartyManager");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unhandled exception in PartyManager");
            return false;
        }
    }

    public async Task LeaveParty(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PartyManager.PartyManagerClient>($"{clientName}{typeof(PartyManager.PartyManagerClient).Name}");
            await client.LeavePartyAsync(new LeavePartyRequest());
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PartyManager");
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Unhandled exception in PartyManager: {ex.Message}");
        }
    }

    public async Task<string[]> GetPartyMembers(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PartyManager.PartyManagerClient>($"{clientName}{typeof(PartyManager.PartyManagerClient).Name}");
            var response = await client.GetPartyMembersAsync(new GetPartyMembersRequest());
            return response.PartyMembers.ToArray();
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PartyManager");
            return new string[0];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unhandled exception in PartyManager");
            return new string[0];
        }
    }
}

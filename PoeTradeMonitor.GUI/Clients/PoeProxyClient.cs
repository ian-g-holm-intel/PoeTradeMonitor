using PoeLib.Proto;
using PoeTradeMonitor.GUI.Interfaces;
using PoeTradeMonitorProto;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Grpc.Net.ClientFactory;

namespace PoeTradeMonitor.GUI.Clients;

public class PoeProxyClient : IPoeProxyClient
{
    private GrpcClientFactory clientFactory;
    private ILogger<PoeProxyClient> logger;

    public PoeProxyClient(ILogger<PoeProxyClient> logger, GrpcClientFactory clientFactory)
    {
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task AddCharacterMessageAsync(PoeLib.CharacterMessage message, string clientName)
    {
        var characterMessage = new CharacterMessage() 
        { 
            Character = message.Character,
            Message = message.Message,
            Timestamp = message.Timestamp.ToBinary(),
            Source = (CharacterMessage.Types.MessageSource)message.Source
        };

        try
        {
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            await client.AddCharacterMessageAsync(new AddCharacterMessageRequest() { Message = characterMessage });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy}");
        }
    }

    public async Task AddTradeRequestAsync(PoeLib.ItemTradeRequest tradeRequest, string clientName)
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
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            await client.AddTradeRequestAsync(new AddTradeRequestRequest(){ TradeRequest = itemTradeRequest });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy");
        }
    }

    public async Task SendMessageAsync(string message, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            await client.SendMessageAsync(new SendMessageRequest() { Message = message });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy");
        }
    }

    public async Task AntiAFK(string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            await client.AntiAFKAsync(new AntiAFKRequest());
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy");
        }
    }

    public async Task<bool> CheckPartyInviteAsync(string accountName, int timeoutMs, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            var response = await client.CheckPartyInviteAsync(new CheckPartyInviteRequest() { AccountName = accountName, Timeout = timeoutMs });
            return response.InviteReceived;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy");
            return false;
        }
    }

    public async Task SetAutoReplyAsync(bool enabled, string clientName)
    {
        try
        {
            var client = clientFactory.CreateClient<PoeProxy.PoeProxyClient>($"{clientName}{typeof(PoeProxy.PoeProxyClient).Name}");
            await client.SetAutoReplyAsync(new SetAutoReplyRequest() { Enabled = enabled });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Failed to connect to PoeProxy");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to PoeProxy");
        }
    }
}

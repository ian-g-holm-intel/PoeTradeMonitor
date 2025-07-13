using System.Threading.Tasks;

namespace PoeTradeMonitor.Services.Interfaces;

public interface ICallbackClient
{
    Task JoinedPartyAsync(string account, string characterName);
    Task CompletedTradeAsync(string account);
}

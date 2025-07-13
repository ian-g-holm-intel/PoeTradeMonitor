using PoeLib;
using System.Threading;
using System.Threading.Tasks;

namespace PoeTradeMonitor.Service.Interfaces;

public interface ITradeBotStateMachine
{
    Task ChangeState(Trigger trigger, CancellationToken ct = default);
    Task OpenTrade(string character, CancellationToken ct = default);
    Task InviteToParty(string character, CancellationToken ct = default);
    Task HideoutJoinTimeout(string character, CancellationToken ct = default);
    Task GotoPlayerHideout(string character, CancellationToken ct = default);
    Task KickPlayer(string character, CancellationToken ct = default);
}

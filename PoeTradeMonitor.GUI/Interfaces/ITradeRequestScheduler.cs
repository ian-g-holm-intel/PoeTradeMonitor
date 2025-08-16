using PoeLib.Common;
using PoeTradeMonitor.GUI.Models;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface ITradeRequestScheduler
{
    Task ScheduleRequestAsync(StashGuiItem item);
    Task JoinedParty(string account, string characterName);
    void TradeComplete(string account);
    Task UpdateSettings(bool unattendedEnabled, ServiceLocation serviceLocation);
    bool AlertsEnabled { get; set; }
}

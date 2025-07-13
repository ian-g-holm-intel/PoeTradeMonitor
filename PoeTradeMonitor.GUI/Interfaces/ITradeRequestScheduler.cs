using PoeLib;
using PoeLib.GuiDataClasses;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface ITradeRequestScheduler
{
    Task ScheduleRequestAsync(StashGuiItem item);
    Task JoinedParty(string account, string characterName);
    void TradeComplete(string account);
    Task UpdateSettings(bool unattendedEnabled, bool tradeConfirmationEnabled, ServiceLocation serviceLocation);
    bool AlertsEnabled { get; set; }
}

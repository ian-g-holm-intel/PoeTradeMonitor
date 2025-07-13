using PoeLib;
using PoeLib.GuiDataClasses;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeItemLiveSearchManager : IDisposable
{
    bool IsConnected { get; }
    Task UpdateLiveSearchesAsync(Strictness strictness, string league, IEnumerable<SearchGuiItem> searchGuiItems);
    Task StopAllLiveSearchesAsync();
}

using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.ViewModels;

namespace PoeTradeMonitor.GUI.Interfaces;

public interface IStashDataUpdater
{
    void SetViewModel(MainWindowViewModel vm);
    Task UpdateStash(TradeSearchResult itemSearchResult);
}

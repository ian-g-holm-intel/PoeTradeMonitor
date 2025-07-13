using System.Windows.Input;
using PoeTradeMonitor.GUI.Commands;
using PoeTradeMonitor.GUI.Views;

namespace PoeTradeMonitor.GUI.ViewModels;

public class SetMinPriceWindowViewModel : ViewModelBase
{
    public ICommand SetCommand { get; set; }
    public SetMinPriceWindowViewModel(int price)
    {
        minPrice = price;
        SetCommand = new DelegateCommand<SetMinPriceWindow>(ExecuteSetCommand);
    }

    private int minPrice;
    public int MinPrice
    {
        get { return minPrice; }
        set
        {
            minPrice = value;
            RaisePropertyChanged();
        }
    }

    private void ExecuteSetCommand(SetMinPriceWindow window)
    {
        window?.Close();
    }
}

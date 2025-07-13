using System.Net;
using System.Windows.Input;
using PoeTradeMonitor.GUI.Commands;
using PoeTradeMonitor.GUI.Views;

namespace PoeTradeMonitor.GUI.ViewModels;

public class SetServerIPWindowViewModel : ViewModelBase
{
    public ICommand SetCommand { get; set; }
    public SetServerIPWindowViewModel(IPAddress IP)
    {
        serverIP = IP.ToString();
        SetCommand = new DelegateCommand<SetServerIPWindow>(ExecuteSetCommand);
    }

    private string serverIP;
    public string ServerIP
    {
        get { return serverIP; }
        set
        {
            serverIP = value;
            RaisePropertyChanged();
        }
    }

    private void ExecuteSetCommand(SetServerIPWindow window)
    {
        IPAddress ipAddress;
        if (!IPAddress.TryParse(serverIP, out ipAddress))
            serverIP = "127.0.0.1";
        window?.Close();
    }
}

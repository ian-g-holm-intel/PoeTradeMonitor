using System.Net;
using PoeTradeMonitor.GUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PoeTradeMonitor.GUI.ViewModels;

public partial class SetServerIPWindowViewModel : ObservableObject
{
    [RelayCommand]
    private void Set(SetServerIPWindow? window)
    {
        if (!IPAddress.TryParse(ServerIP, out _))
            ServerIP = "127.0.0.1";

        window?.Close();
    }

    public SetServerIPWindowViewModel(IPAddress ip)
    {
        ServerIP = ip.ToString();
    }

    [ObservableProperty]
    private string serverIP = "";
}
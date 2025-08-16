using CommunityToolkit.Mvvm.ComponentModel;
using PoeLib.Common;
using PoeTradeMonitor.GUI.Models;

namespace PoeTradeMonitor.GUI.Settings;

/// <summary>
/// Main application settings for PoeTradeMonitor GUI.
/// </summary>

public partial class PoeSettings : ObservableObject
{
    [ObservableProperty]
    private string league = "Dawn%20of%20the%20Hunt";

    [ObservableProperty]
    private string hideoutName = "Felled Hideout";

    [ObservableProperty]
    private string windowsVersion = "19.0.0";

    [ObservableProperty]
    private bool alertsEnabled;

    [ObservableProperty]
    private bool antiAFKEnabled;

    [ObservableProperty]
    private bool running;

    [ObservableProperty]
    private bool priceLoggerEnabled;

    [ObservableProperty]
    private bool unattendedModeEnabled;

    [ObservableProperty]
    private bool autoreplyEnabled;

    [ObservableProperty]
    private ServiceLocation serviceLocation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnabledSearchCount))]
    private List<SearchGuiItem> searchItems = new();

    public int EnabledSearchCount => SearchItems?.Count(item => item.Enabled) ?? 0;

    [ObservableProperty]
    private List<string> ignoredAccounts = new();

    [ObservableProperty]
    private BrowserSettings browserSettings = BrowserSettings.Default;
}

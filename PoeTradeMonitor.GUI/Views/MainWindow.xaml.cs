using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using PoeLib.GuiDataClasses;
using PoeTradeMonitor.GUI.ViewModels;

namespace PoeTradeMonitor.GUI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        if (GetScreenCount() == 1)
        {
            if (SystemParameters.PrimaryScreenWidth == 2560)
            {
                Top = 707;
            }
            if (SystemParameters.PrimaryScreenWidth == 1920)
            {
                Top = 327;
            }
        }
        else
        {
            if (SystemParameters.PrimaryScreenWidth == 2560)
            {
                Left = 2553;
                Top = 707;
            }
            if (SystemParameters.PrimaryScreenWidth == 1920)
            {
                Left = 1913;
                Top = 327;
            }
        }

        // Assign to the data context so binding can be used.
        DataContext = viewModel;
        viewModel.ScrollIntoView += ScrollIntoView;
    }

    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

    delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    public static int GetScreenCount()
    {
        int count = 0;
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
            {
                count++;
                return true;
            }, IntPtr.Zero);
        return count;
    }

    private void ScrollIntoView(StashGuiItem objEvent)
    {
        searchItemsGrid.ScrollIntoView(objEvent);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Application.Current.DispatcherUnhandledException += (dispatcher, args) => File.WriteAllText($"UnhandledException_{DateTime.Now.ToFileTime()}.txt", args.Exception.ToString()); 
        await ((MainWindowViewModel) DataContext).InitializeAsync();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Application.Current.Shutdown();
    }
}

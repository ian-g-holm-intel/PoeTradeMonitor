﻿using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PoeTradeMonitor.GUI.ViewModels;

namespace PoeTradeMonitor.GUI.Views;

/// <summary>
/// Interaction logic for SetServerIPWindow.xaml
/// </summary>
public partial class SetServerIPWindow : Window
{
    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public SetServerIPWindow(SetServerIPWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
}

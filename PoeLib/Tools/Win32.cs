using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace PoeLib.Tools;

public class Win32
{
    private const short SWP_NOMOVE = 0X2;
    private const short SWP_NOSIZE = 1;
    private const short SWP_NOACTIVATE = 0x0010;

    private enum WindowPosition
    {
        HWND_NOTOPMOST = -2,
        HWND_TOPMOST = -1,
        HWND_TOP = 0,
        HWND_BOTTOM = 1
    }

    [DllImport("user32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string WindowName);

    // Activate an application window.
    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("USER32.DLL")]
    private static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

    [DllImport("USER32.DLL")]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    [DllImport("KERNEL32.DLL")]
    private static extern IntPtr GetCurrentThreadId();

    [DllImport("USER32.DLL")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("USER32.DLL")]
    private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    public static IntPtr FindWindow(string windowName)
    {
        return FindWindow(null, windowName);
    }

    public static IntPtr GetPoeWindowPointer()
    {
        Process[] processes = Process.GetProcesses();
        foreach (Process proc in processes)
        {
            if (proc.ProcessName.Equals("PathOfExile"))
                return proc.MainWindowHandle;
        }
        return IntPtr.Zero;
    }

    public static IntPtr GetCurrentWindow()
    {
        return GetForegroundWindow();
    }

    public static void ActivateWindow(IntPtr hWnd)
    {
        if (hWnd == GetForegroundWindow())
            return;

        IntPtr mainThreadId = GetCurrentThreadId();
        IntPtr foregroundThreadID = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
        if (foregroundThreadID != mainThreadId)
        {
            AttachThreadInput(mainThreadId, foregroundThreadID, true);
            SetForegroundWindow(hWnd);
            AttachThreadInput(mainThreadId, foregroundThreadID, false);
        }
        else
            SetForegroundWindow(hWnd);
    }

    public static bool WaitWindowActive(IntPtr hWnd, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while(hWnd != GetForegroundWindow() && stopwatch.Elapsed < timeout)
            Thread.Sleep(10);
        return hWnd == GetForegroundWindow();
    }

    public static void SendWindowToBack(IntPtr hWnd)
    {
        SetWindowPos(hWnd, (int)WindowPosition.HWND_BOTTOM, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
    }
}

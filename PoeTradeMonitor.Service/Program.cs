using Microsoft.AspNetCore.Server.Kestrel.Core;
using PoeTradeMonitor.Service.Services;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PoeTradeMonitor.Service;

/// <summary>
/// Entry point class for the PoeTradeMonitor Service application.
/// Provides background trading automation and bot services for Path of Exile.
/// </summary>
public class Program
{
    const int SWP_NOSIZE = 0x0000;

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    private static readonly IntPtr MyConsole = GetConsoleWindow();

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    /// <summary>
    /// Main entry point for the service application. Sets up logging, window positioning, and starts the gRPC services.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static async Task Main(string[] args)
    {
        if (Process.GetProcessesByName("PathOfExile").Length == 0)
            return;

#if DEBUG
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}";
#else
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";
#endif
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PoeTradeMonitor", assembly.GetName().Name + ".log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("Grpc", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(logPath, restrictedToMinimumLevel: LogEventLevel.Debug, rollOnFileSizeLimit: true, fileSizeLimitBytes: 52428800, retainedFileCountLimit: 1, outputTemplate: logFormat, flushToDiskInterval: TimeSpan.FromSeconds(1), shared: true)
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: logFormat)
            .CreateLogger();

        if (Screen.AllScreens.Length == 1)
        {
            SetWindowPos(MyConsole, 0, 0, 0, 1500, 300, SWP_NOSIZE);
        }
        else
        {
            if (Screen.PrimaryScreen!.Bounds.Width == 2560)
            {
                SetWindowPos(MyConsole, 0, 2553, 0, 1500, 300, SWP_NOSIZE);
            }
            if (Screen.PrimaryScreen.Bounds.Width == 1920)
            {
                SetWindowPos(MyConsole, 0, 1911, 0, 900, 300, SWP_NOSIZE);
            }
        }
        Console.CursorLeft = 0;
        Console.OutputEncoding = Encoding.UTF8;

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddPoeTradeMonitorService();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5002, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        var app = builder.Build();

        app.MapGrpcService<TradeBotService>();
        app.MapGrpcService<PartyManagerService>();

        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        var pathOfExileProcess = Process.GetProcessesByName("PathOfExile").First();
        pathOfExileProcess.Exited += (sender, e) =>
        {
            var hostApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            hostApplicationLifetime.StopApplication();
        };
        pathOfExileProcess.EnableRaisingEvents = true;

        await app.RunAsync();
    }
}
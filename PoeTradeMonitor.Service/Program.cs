using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using System.Reflection;
using System;
using Serilog;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PoeLib.Settings;
using System.Text.Json;
using PoeLib.Common;

namespace PoeTradeMonitor.Service;

public class Program
{
    const int SWP_NOSIZE = 0x0000;

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    private static readonly IntPtr MyConsole = GetConsoleWindow();

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    public static async Task Main(string[] args)
    {
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
            if (Screen.PrimaryScreen.Bounds.Width == 2560)
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

        var settingsFilePath = Path.Combine(Constants.DataDirectory, "settings.json");
        if (!File.Exists(settingsFilePath))
        {
            var settings = new PoeSettings();
            Directory.CreateDirectory(Constants.DataDirectory);
            using FileStream createStream = File.Create(settingsFilePath);
            await JsonSerializer.SerializeAsync(createStream, settings, new JsonSerializerOptions { WriteIndented = true });
        }
        var configuration = new ConfigurationBuilder().AddJsonFile(settingsFilePath, false).Build();

        using var host = CreateHostBuilder(args, configuration).Build();

        var logger = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();

        await host.RunAsync();
    }

    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseConfiguration(configuration);
                webBuilder.ConfigureKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5002, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
                webBuilder.UseStartup<Startup>();
            });
}

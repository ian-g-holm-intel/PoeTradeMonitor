using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoeLib.Common;
using PoeTradeMonitor.GUI.Services;
using PoeTradeMonitor.GUI.Settings;
using PoeTradeMonitor.GUI.Views;
using Serilog;
using Serilog.Events;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace PoeTradeMonitor.GUI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private WebApplication? webApplication;

    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}";
#else
        var logFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";
#endif
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        var logPath = Path.Combine(Constants.DataDirectory, assembly.GetName().Name + ".log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("Grpc", LogEventLevel.Error)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.Extensions.Hosting.Internal.Host", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.File(logPath, restrictedToMinimumLevel: LogEventLevel.Debug, rollOnFileSizeLimit: true, fileSizeLimitBytes: 52428800, retainedFileCountLimit: 1, outputTemplate: logFormat, flushToDiskInterval: TimeSpan.FromSeconds(1), shared: true)
            .CreateLogger();

        var settingsFilePath = Path.Combine(Constants.DataDirectory, "settings.json");
        if (!File.Exists(settingsFilePath))
        {
            var settings = new PoeSettings();
            Directory.CreateDirectory(Constants.DataDirectory);
            using FileStream createStream = File.Create(settingsFilePath);
            JsonSerializer.Serialize(createStream, settings, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            });
        }
        var configuration = new ConfigurationBuilder().AddJsonFile(settingsFilePath, false, true).Build();

        var builder = WebApplication.CreateBuilder();

        // Configure Kestrel to use HTTP/2
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
        });

        builder.Services.AddPoeTradeMonitorGui();

        webApplication = builder.Build();

        // Initialize the SettingsManager to start listening for changes
        webApplication.Services.GetRequiredService<SettingsManager>();

        webApplication.MapGrpcService<CallbackService>();
        webApplication.MapGet("/", () => "gRPC service is running");

        webApplication.Start();

        var mainWindow = webApplication.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (webApplication != null)
            {
                await webApplication.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Unexpected error during shutdown");
        }
        base.OnExit(e);
    }
}


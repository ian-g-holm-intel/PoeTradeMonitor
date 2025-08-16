using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PoeTradeMonitor.GUI.Views;
using Serilog;
using Serilog.Events;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PoeLib.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using PoeTradeMonitor.GUI.Settings;

namespace PoeTradeMonitor.GUI;

/// <summary>
/// Entry point class for the PoeTradeMonitor GUI application.
/// Handles application startup, logging configuration, and host setup.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point for the application. Configures logging, settings, and starts the WPF application.
    /// </summary>
    [STAThread]
    public static void Main()
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

        var hostBuilder = CreateHostBuilder(configuration);
        using var host = hostBuilder.Build();

        host.Start();

        RunApplication(host.Services);

        host.StopAsync().Wait();
    }

    /// <summary>
    /// Runs the WPF application with the provided service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider containing application dependencies.</param>
    private static void RunApplication(IServiceProvider serviceProvider)
    {
        try
        {
            var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                File.WriteAllText($"UnhandledException_{DateTime.Now.ToFileTime()}.txt", args.ExceptionObject.ToString());
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                args.SetObserved();
            };

            // Initialize the SettingsManager to start listening for changes
            serviceProvider.GetRequiredService<SettingsManager>();
            
            var app = new App();
            var mainWindow = serviceProvider.GetService<MainWindow>();
            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Log.Logger.Error("Failed to start application: {ex}", ex);
        }
    }

    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
    /// <summary>
    /// Creates and configures the host builder for the application.
    /// </summary>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>Configured host builder.</returns>
    public static IHostBuilder CreateHostBuilder(IConfiguration configuration) =>
        Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseConfiguration(configuration);
                webBuilder.ConfigureKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5001, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
                webBuilder.UseStartup<Startup>();
            });
}

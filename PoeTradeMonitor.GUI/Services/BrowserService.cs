using Microsoft.Extensions.Logging;
using PoeTradeMonitor.GUI.Settings;
using System.Diagnostics;

namespace PoeTradeMonitor.GUI.Services;

/// <summary>
/// Interface for browser operations.
/// </summary>
public interface IBrowserService
{
    /// <summary>
    /// Opens a URL in the configured browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OpenUrlAsync(string url);
}

/// <summary>
/// Service for opening URLs in web browsers with configurable settings.
/// </summary>
public class BrowserService : IBrowserService
{
    private readonly BrowserSettings _browserSettings;
    private readonly ILogger<BrowserService> _logger;

    public BrowserService(PoeSettings poeSettings, ILogger<BrowserService> logger)
    {
        _browserSettings = poeSettings.BrowserSettings;
        _logger = logger;
    }

    /// <summary>
    /// Opens a URL in the configured browser or system default.
    /// </summary>
    public async Task OpenUrlAsync(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        try
        {
            var startInfo = CreateProcessStartInfo(url);
            var process = Process.Start(startInfo);

            if (process != null)
            {
                _logger.LogDebug("Opened URL {Url} in browser", url);
                await process.WaitForExitAsync().ConfigureAwait(false);
            }
            else
            {
                _logger.LogWarning("Failed to start browser process for URL {Url}", url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open URL {Url} in browser", url);
            
            // Fallback to system default browser
            await OpenWithSystemDefaultAsync(url);
        }
    }

    /// <summary>
    /// Creates the appropriate ProcessStartInfo based on configuration.
    /// </summary>
    private ProcessStartInfo CreateProcessStartInfo(string url)
    {
        if (string.IsNullOrWhiteSpace(_browserSettings.ExecutablePath))
        {
            // Use system default browser
            return new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
        }

        // Use configured browser
        var arguments = url;
        if (!string.IsNullOrWhiteSpace(_browserSettings.ProfileDirectory))
        {
            arguments += $" --profile-directory=\"{_browserSettings.ProfileDirectory}\"";
        }

        return new ProcessStartInfo
        {
            FileName = _browserSettings.ExecutablePath,
            Arguments = arguments,
            UseShellExecute = true
        };
    }

    /// <summary>
    /// Fallback method to open URL with system default browser.
    /// </summary>
    private async Task OpenWithSystemDefaultAsync(string url)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            var process = Process.Start(startInfo);
            if (process != null)
            {
                _logger.LogDebug("Opened URL {Url} with system default browser", url);
                await process.WaitForExitAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open URL {Url} even with system default browser", url);
            throw;
        }
    }
}
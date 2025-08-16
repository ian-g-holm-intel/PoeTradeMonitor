namespace PoeTradeMonitor.GUI.Settings;

/// <summary>
/// Configuration settings for browser integration.
/// </summary>
public class BrowserSettings
{
    /// <summary>
    /// Path to the browser executable. If null or empty, uses system default browser.
    /// </summary>
    public string? ExecutablePath { get; set; }

    /// <summary>
    /// Profile directory for browser instances.
    /// </summary>
    public string? ProfileDirectory { get; set; }

    /// <summary>
    /// Gets the default browser settings.
    /// </summary>
    public static BrowserSettings Default => new()
    {
        ExecutablePath = null, // Use system default
        ProfileDirectory = null
    };

    /// <summary>
    /// Gets browser settings configured for Brave browser.
    /// </summary>
    public static BrowserSettings Brave => new()
    {
        ExecutablePath = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
        ProfileDirectory = "Profile 1"
    };

    /// <summary>
    /// Gets browser settings configured for Chrome browser.
    /// </summary>
    public static BrowserSettings Chrome => new()
    {
        ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
        ProfileDirectory = "Default"
    };
}
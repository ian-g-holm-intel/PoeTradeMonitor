using PoeTrade.Contracts;

namespace PoeLib.Common;

/// <summary>
/// Application-wide constants for PoeLib.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The directory where application data is stored.
    /// </summary>
    public static string DataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PoeTradeMonitor");
    public const TradeCurrencyType BaseCurrencyType = TradeCurrencyType.Chaos;
    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36";
    public const string SecChUa = "\"Not;A=Brand\";v=\"99\", \"Brave\";v=\"139\", \"Chromium\";v=\"139\"";
    public const string SecChUaFullVersionList = "\"Not;A=Brand\";v=\"99.0.0.0\", \"Brave\";v=\"139.0.0.0\", \"Chromium\";v=\"139.0.0.0\"";
}

namespace PoeTradeMonitor.GUI.Common;

public enum Indexer
{
    None,
    Public
}

public enum CurrencyExchangeType
{
    Buy,
    Sell
}

public enum SocketColor
{
    B,
    G,
    R,
    W,
    DV,
    A
}

public enum MonitorResolution
{
    Res_Unknown,
    Res_1280x1024,
    Res_1920x1080,
    Res_2560x1440
}

public static class EnumHelper
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

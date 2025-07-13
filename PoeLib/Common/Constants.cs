using System.IO;
using System;

namespace PoeLib.Common;

public static class Constants
{
    public static string DataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PoeTradeMonitor");
    public static string WebshareApiKey => "3dbc746a3fd601835dce8726561264d7fbc372eb";
    public static int PartyManagerPort = 31417;
    public static int PoeProxyPort = 31418;
    public static int TradeBotPort = 31419;
    public static int JoinedPartyPort = 31420;
    public static double Latitude = 45.512230;
    public static double Longitude = -122.658722;
}

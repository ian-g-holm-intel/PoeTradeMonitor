using PoeLib.GuiDataClasses;
using System;
using System.Collections.Generic;

namespace PoeLib.Settings;

public class PoeSettings
{
    public string League { get; set; } = "Mercenaries";
    public string HideoutName { get; set; } = "Immaculate Hideout";
    public string Account { get; set; } = Environment.GetEnvironmentVariable("POE_ACCOUNT");
    public string WindowsVersion { get; set; } = "19.0.0";
    public string GuiAddress { get; set; } = "127.0.0.1";

    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36";
    public const string SecChUa = "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Brave\";v=\"138\"";
    public const string SecChUaFullVersionList = "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.0.0\", \"Brave\";v=\"138.0.0.0\"";
    
    public bool AlertsEnabled { get; set; }
    public bool HighMarginMode { get; set; }
    public bool AntiAFKEnabled { get; set; }
    public bool Running { get; set; }
    public bool DealfinderEnabled { get; set; }
    public bool TradeConfirmationEnabled { get; set; }
    public bool PriceLoggerEnabled { get; set; }
    public bool UndercutNotificationEnabled { get; set; }
    public bool UnattendedModeEnabled { get; set; }
    public bool AutoreplyEnabled { get; set; }
    public Strictness Strictness { get; set; } = Strictness.Chaos7;
    public ServiceLocation ServiceLocation { get; set; } = ServiceLocation.Local;
    public List<SearchGuiItem> SearchItems { get; set; } = new();
    public List<string> IgnoredAccounts { get; set; } = new();
}

namespace PoeTradeMonitor.GUI.Settings;

public record IgnoredItem
{
    public DateTime TimeStamp { get; set; }
    public string ItemName { get; set; } = string.Empty;
}

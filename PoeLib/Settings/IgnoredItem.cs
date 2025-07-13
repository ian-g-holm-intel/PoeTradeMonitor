using System;

namespace PoeLib.Settings;

public record IgnoredItem
{
    public DateTime TimeStamp { get; set; }
    public string ItemName { get; set; }
}

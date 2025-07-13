using System.Collections.Generic;

namespace PoeLib.JSON.PoeNinja;

public class Sparkline
{
    public List<decimal?> data { get; set; }
    public double totalChange { get; set; }
}

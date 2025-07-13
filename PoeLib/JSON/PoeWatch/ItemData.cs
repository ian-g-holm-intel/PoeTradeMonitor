using System.Collections.Generic;

namespace PoeLib.JSON.PoeWatch;

public class ItemData
{
    public int id { get; set; }
    public string name { get; set; }
    public string category { get; set; }
    public string group { get; set; }
    public int frame { get; set; }
    public string influences { get; set; }
    public string icon { get; set; }
    public decimal mean { get; set; }
    public decimal min { get; set; }
    public decimal max { get; set; }
    public decimal exalted { get; set; }
    public int daily { get; set; }
    public int change { get; set; }
    public List<decimal> history { get; set; }
    public bool lowConfidence { get; set; }
    public object implicits { get; set; }
    public object explicits { get; set; }
    public int itemLevel { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int? linkCount { get; set; }
    public double perfectPrice { get; set; }
    public int perfectAmount { get; set; }
    public int? mapTier { get; set; }
    public string reward { get; set; }
    public double? reward_price { get; set; }
    public int? reward_id { get; set; }

    public override string ToString()
    {
        return $"{name}, {min}c";
    }
}

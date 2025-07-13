using System.Collections.Generic;

namespace PoeLib.JSON.PoeWatch;

public class CurrencyData
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
    public List<double> history { get; set; }
    public bool lowConfidence { get; set; }
    public object implicits { get; set; }
    public object explicits { get; set; }
    public int itemLevel { get; set; }
    public int width { get; set; }
    public int height { get; set; }

    public override string ToString()
    {
        return name;
    }
}

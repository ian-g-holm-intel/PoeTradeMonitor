namespace PoeLib.JSON.PoeWatch;

public class CurrencyData
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string category { get; set; } = string.Empty;
    public string group { get; set; } = string.Empty;
    public int frame { get; set; }
    public string influences { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
    public decimal mean { get; set; }
    public decimal min { get; set; }
    public decimal max { get; set; }
    public decimal exalted { get; set; }
    public int daily { get; set; }
    public int change { get; set; }
    public List<double> history { get; set; } = new List<double>();
    public bool lowConfidence { get; set; }
    public object implicits { get; set; } = new object();
    public object explicits { get; set; } = new object();
    public int itemLevel { get; set; }
    public int width { get; set; }
    public int height { get; set; }

    public override string ToString()
    {
        return name;
    }
}

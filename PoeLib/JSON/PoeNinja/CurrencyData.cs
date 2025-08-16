namespace PoeLib.JSON.PoeNinja;

public class Pay
{
    public int id { get; set; }
    public int league_id { get; set; }
    public int pay_currency_id { get; set; }
    public int get_currency_id { get; set; }
    public DateTime sample_time_utc { get; set; }
    public int count { get; set; }
    public decimal value { get; set; }
    public int data_point_count { get; set; }
    public bool includes_secondary { get; set; }
    public int listing_count { get; set; }
}

public class Receive
{
    public int id { get; set; }
    public int league_id { get; set; }
    public int pay_currency_id { get; set; }
    public int get_currency_id { get; set; }
    public DateTime sample_time_utc { get; set; }
    public int count { get; set; }
    public decimal value { get; set; }
    public int data_point_count { get; set; }
    public bool includes_secondary { get; set; }
    public int listing_count { get; set; }
}

public class Line
{
    public string currencyTypeName { get; set; } = string.Empty;
    public Pay? pay { get; set; }
    public Receive? receive { get; set; }
    public Sparkline paySparkLine { get; set; } = new Sparkline();
    public Sparkline receiveSparkLine { get; set; } = new Sparkline();
    public decimal chaosEquivalent { get; set; }
    public Sparkline lowConfidencePaySparkLine { get; set; } = new Sparkline();
    public Sparkline lowConfidenceReceiveSparkLine { get; set; } = new Sparkline();
    public string detailsId { get; set; } = string.Empty;

    public override string ToString()
    {
        return currencyTypeName;
    }
}

public class CurrencyDetail
{
    public int id { get; set; }
    public string icon { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string tradeId { get; set; } = string.Empty;
}

public class CurrencyData
{
    public List<Line> lines { get; set; } = new List<Line>();
    public List<CurrencyDetail> currencyDetails { get; set; } = new List<CurrencyDetail>();
    public Language language { get; set; } = new Language();
}

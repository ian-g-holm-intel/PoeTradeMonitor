using System.Collections.Generic;

namespace PoeLib.JSON.Currency;

public class CurrencyExchangeRequest
{
    public Query query { get; set; }
    public Sort sort { get; set; }
    public string engine { get; set; }
}

public class Query
{
    public Status status { get; set; }
    public List<string> have { get; set; }
    public List<string> want { get; set; }
    public string account { get; set; }
    public int? minimum { get; set; }
}

public class Sort
{
    public string have { get; set; }
}

public class Status
{
    public string option { get; set; }
}

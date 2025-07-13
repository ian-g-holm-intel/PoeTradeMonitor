using System.Collections.Generic;

namespace PoeLib.JSON;

public class PoeTradeResponseData
{
    public int count { get; set; }
    public string data { get; set; }
    public int newid { get; set; }
    public List<string> uniqs { get; set; }
}

using System.Collections.Generic;

namespace PoeLib.Trade;

public class PoeItemSearchResponse
{
    public List<string> result { get; set; }
    public string id { get; set; }
    public int total { get; set; }
}

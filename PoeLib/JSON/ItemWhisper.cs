using System.Collections.Generic;

namespace PoeLib.JSON;

public record ItemWhisper
{
    public string token { get; set; }
    public List<int> values { get; set; }
}

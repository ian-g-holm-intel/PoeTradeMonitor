using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.OrbWatchTrade;

public class OrbWatchData
{
    [JsonPropertyName("currencies")]
    public List<OrbWatchCurrency>? Currencies { get; set; }
}

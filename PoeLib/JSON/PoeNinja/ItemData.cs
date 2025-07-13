using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.PoeNinja;

public class ItemLine
{
    public int id { get; set; }
    public string name { get; set; }
    public string icon { get; set; }
    public int? mapTier { get; set; }
    public string baseType { get; set; }
    public int? stackSize { get; set; }
    public string artFilename { get; set; }
    public string variant { get; set; }
    public string prophecyText { get; set; }
    public int? links { get; set; }
    [JsonPropertyName("itemClass")]
    public Rarity rarity { get; set; }
    public Sparkline sparkline { get; set; }
    public Sparkline lowConfidenceSparkline { get; set; }
    public List<object> implicitModifiers { get; set; }
    public List<ExplicitModifier> explicitModifiers { get; set; }
    public string flavourText { get; set; }
    public bool? corrupted { get; set; }
    public string itemType { get; set; }
    public decimal chaosValue { get; set; }
    public decimal exaltedValue { get; set; }
    public decimal divineValue { get; set; }
    public int count { get; set; }
    public string detailsId { get; set; }
    public int listingCount { get; set; }
    public int? levelRequired { get; set; }

    public override string ToString()
    {
        return $"{name}, {chaosValue}c";
    }
}

public class ItemData
{
    public List<ItemLine> lines { get; set; }
    public Language language { get; set; }
}

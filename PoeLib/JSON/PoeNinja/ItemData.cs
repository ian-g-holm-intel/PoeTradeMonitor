using PoeTrade.Contracts;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.PoeNinja;

public class ItemLine
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
    public int? mapTier { get; set; }
    public string baseType { get; set; } = string.Empty;
    public int? stackSize { get; set; }
    public string artFilename { get; set; } = string.Empty;
    public string variant { get; set; } = string.Empty;
    public string prophecyText { get; set; } = string.Empty;
    public int? links { get; set; }
    [JsonPropertyName("itemClass")]
    public ItemRarity rarity { get; set; }
    public Sparkline sparkline { get; set; } = new Sparkline();
    public Sparkline lowConfidenceSparkline { get; set; } = new Sparkline();
    public List<object> implicitModifiers { get; set; } = new List<object>();
    public List<ExplicitModifier> explicitModifiers { get; set; } = new List<ExplicitModifier>();
    public string flavourText { get; set; } = string.Empty;
    public bool? corrupted { get; set; }
    public string itemType { get; set; } = string.Empty;
    public decimal chaosValue { get; set; }
    public decimal exaltedValue { get; set; }
    public decimal divineValue { get; set; }
    public int count { get; set; }
    public string detailsId { get; set; } = string.Empty;
    public int listingCount { get; set; }
    public int? levelRequired { get; set; }

    public override string ToString()
    {
        return $"{name}, {chaosValue}c";
    }
}

public class ItemData
{
    public List<ItemLine> lines { get; set; } = new List<ItemLine>();
    public Language language { get; set; } = new Language();
}

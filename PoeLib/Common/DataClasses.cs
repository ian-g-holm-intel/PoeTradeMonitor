using System.Drawing;
using System.Text.Json.Serialization;
using PoeTrade.Contracts;

namespace PoeLib.Common;

public record CharacterMessage(string Character, string Message, MessageSource Source, DateTime Timestamp);

public record CurrencyStack(TradeCurrencyType Type, Point Location)
{
    public Point Slot { get; set; } = new Point();

    public decimal Amount { get; set; }

    public override string ToString()
    {
        return $"{Type} - {Amount}";
    }
}

public record EssenceStack(string Type, Point Location, decimal Amount);

public class SearchItemGroup
{
    public string GroupName = "";
    public List<SearchItem> SearchItems { get; set; } = new List<SearchItem>();

    public override string ToString()
    {
        return $"{GroupName} ({SearchItems.Count})";
    }
}

public record SearchItem
{
    public string Name { get; set; } = string.Empty;
    public string BaseType { get; set; } = string.Empty;
    private string variant = string.Empty;
    public string Variant
    {
        get => variant;
        set => variant = value.Replace("socket", "Jewel");
    }
    public decimal Price { get; set; }
    public int Links { get; set; }
    public ItemRarity Rarity { get; set; } = ItemRarity.Normal;
    public int MapTier { get; set; }
    public bool Volatile { get; set; }
    public bool AllowCorrupted { get; set; }
    public string Source { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{(Rarity == ItemRarity.UniqueFoil ? $"Foil {Name}" : Name)}{(Links > 4 ? ", " + Links + "L" : "")}{(!string.IsNullOrEmpty(Variant) ? ", " + Variant : "")}, {Price}c";
    }
}

public record CurrencyPrice
{
    public TradeCurrencyType Type { get; set; }
    public decimal BuyPrice { get; set; } = 1;
    public decimal SellPrice { get; set; } = 1;
    public decimal AvgPrice { get; set; } = 1;

    public override string ToString()
    {
        return $"{Type}: {Math.Round(SellPrice, 1)}";
    }
}

public record ItemTradeRequest(string CharacterName, string AccountName, TradeItem Item, PriceInfo Price, List<CurrencyInfo> Currencies, DateTime Timestamp, decimal DivineRate)
{
    public string CharacterName { get; set; } = CharacterName;

    public override string ToString()
    {
        return $"{CharacterName}: {Item} for {Price}, DivineRate: {DivineRate}, StackSize: {Item.StackSize}";
    }
}

public record ItemTradeOffer
{
    public string CharacterName { get; set; } = string.Empty;
    public TradeItem? Item { get; set; }
    public PriceInfo? Offer { get; set; }
    public PriceInfo? Price { get; set; }

    public override string ToString()
    {
        return $"{CharacterName}: {Offer} for {Item}, Price: {Price}";
    }
}

public static class AffixMods
{
    public static string[] GetMods()
    {
        string[] mods = Array.Empty<string>();
        if (File.Exists(@"AffixMods.txt"))
            mods = File.ReadAllLines(@"AffixMods.txt");
        return mods;
    }
}

public record Fraction
{
    public Fraction() { }
    public Fraction(int n, int d)
    {
        N = n;
        D = d;
        SimplifyFraction();
    }

    public int N;
    public int D;

    [JsonIgnore]
    public decimal DecimalValue => Convert.ToDecimal(N) / Convert.ToDecimal(D);
    [JsonIgnore]
    public decimal InverseDecimalValue => Convert.ToDecimal(D) / Convert.ToDecimal(N);

    private void SimplifyFraction()
    {
        var gcd = 0;
        for (int x = 1; x <= D; x++)
        {
            if (N % x == 0 && D % x == 0)
                gcd = x;
        }

        if (gcd == 0)
            return;

        N = N / gcd;
        D = D / gcd;
    }

    public override string ToString()
    {
        return $"{N}:{D}";
    }
}

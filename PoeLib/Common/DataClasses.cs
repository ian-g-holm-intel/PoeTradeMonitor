using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PoeLib.Annotations;
using PoeLib.JSON;

namespace PoeLib;

[DataContract]
public class CharacterMessage
{
    public CharacterMessage() { }
    public CharacterMessage(string character, string message, MessageSource source)
    {
        Character = character;
        Message = message;
        Source = source;
    }

    [DataMember]
    public string Character { get; set; }
    [DataMember]
    public string Message { get; set; }
    [DataMember]
    public MessageSource Source { get; set; }
    [DataMember]
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class CurrencyStack
{
    public CurrencyType Type { get; set; }
    public Point Location { get; set; }
    public Point Slot { get; set; }
    public decimal Amount { get; set; }

    public override string ToString()
    {
        return $"{Type} - {Amount}";
    }
}

public class EssenceStack
{
    public string Type { get; set; }
    public Point Location { get; set; }
    public decimal Amount { get; set; }
}

public class SearchItemGroup
{
    public string GroupName = "";
    public List<SearchItem> SearchItems { get; set; } = new List<SearchItem>();

    public override string ToString()
    {
        return $"{GroupName} ({SearchItems.Count})";
    }
}

public class SearchItem
{
    public string Name { get; set; }
    public string BaseType { get; set; }
    private string variant;
    public string Variant
    {
        get => variant;
        set => variant = value?.Replace("socket", "Jewel");
    }
    public decimal Price { get; set; }
    public int Links { get; set; }
    public Rarity Rarity { get; set; } = Rarity.Any;
    public int MapTier { get; set; }
    public bool Volatile { get; set; }
    public bool AllowCorrupted { get; set; }
    public string Source { get; set; }

    public override string ToString()
    {
        return $"{(Rarity == Rarity.UniqueFoil ? $"Foil {Name}" : Name)}{(Links > 4 ? ", " + Links + "L" : "")}{(!string.IsNullOrEmpty(Variant) ? ", " + Variant : "")}, {Price}c";
    }

    public override bool Equals(object obj)
    {
        return obj is SearchItem item &&
               Name == item.Name &&
               Variant == item.Variant &&
               Links == item.Links &&
               Rarity == item.Rarity;
    }

    public override int GetHashCode()
    {
        int hashCode = 715246148;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Variant);
        hashCode = hashCode * -1521134295 + Links.GetHashCode();
        hashCode = hashCode * -1521134295 + Rarity.GetHashCode();
        return hashCode;
    }
}

[DataContract]
[Serializable]
public class Currency : IEquatable<Currency>
{
    [DataMember]
    public CurrencyType Type { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public decimal ChaosEquiv { get; set; }
    [DataMember]
    public bool HasPriceSet { get; set; }
    [DataMember]
    public Fraction Price { get; set; }

    public override string ToString()
    {
        return $"{Amount} {Type.GetCurrencyDescription()}";
    }

    public bool Equals(Currency other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Amount == other.Amount && ChaosEquiv == other.ChaosEquiv && HasPriceSet == other.HasPriceSet && Equals(Price, other.Price);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Currency) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (int) Type;
            hashCode = (hashCode * 397) ^ Amount.GetHashCode();
            hashCode = (hashCode * 397) ^ ChaosEquiv.GetHashCode();
            hashCode = (hashCode * 397) ^ HasPriceSet.GetHashCode();
            hashCode = (hashCode * 397) ^ (Price != null ? Price.GetHashCode() : 0);
            return hashCode;
        }
    }
}

public class CurrencyPrice : IEquatable<CurrencyPrice>
{
    public CurrencyType Type { get; set; }
    public decimal BuyPrice { get; set; } = 1;
    public decimal SellPrice { get; set; } = 1;
    public decimal AvgPrice { get; set; } = 1;

    private Fraction buyFraction;
    public Fraction BuyFraction
    {
        get => buyFraction ?? (buyFraction = BuyPrice.ToFraction());
        set => buyFraction = value;
    }

    private Fraction sellFraction;
    public Fraction SellFraction
    {
        get => sellFraction ?? (sellFraction = SellPrice.ToFraction());
        set => sellFraction = value;
    }

    public bool Equals(CurrencyPrice other)
    {
        if (other == null) return false;
        return BuyPrice == other.BuyPrice && SellPrice == other.SellPrice && AvgPrice == other.AvgPrice;
    }

    public override bool Equals(object obj)
    {
        CurrencyPrice other = obj as CurrencyPrice;
        return other != null && Equals(other);
    }

    public override int GetHashCode()
    {
        return BuyPrice.GetHashCode() ^ SellPrice.GetHashCode() ^ AvgPrice.GetHashCode();
    }

    public override string ToString()
    {
        return Type.ToString();
    }
}

[DataContract]
public class ItemTradeRequest
{
    [DataMember]
    public string CharacterName { get; set; }
    [DataMember]
    public string AccountName { get; set; }
    [DataMember]
    public Item Item { get; set; }
    [DataMember]
    public Parsers.Price Price { get; set; }
    [DataMember]
    public DateTime Timestamp { get; set; }
    [DataMember]
    public decimal DivineRate { get; set; }

    public override string ToString()
    {
        return $"{CharacterName}: {Item} for {Price}, DivineRate: {DivineRate}, StackSize: {Item.stackSize}";
    }
}

[DataContract]
public class ItemTradeOffer
{
    [DataMember]
    public string CharacterName { get; set; }
    [DataMember]
    public Item Item { get; set; }
    [DataMember]
    public Parsers.Price Offer { get; set; }
    [DataMember]
    public Parsers.Price Price { get; set; }

    public override string ToString()
    {
        return $"{CharacterName}: {Offer} for {Item}, Price: {Price}";
    }
}


public class CurrencyTradeOffer : INotifyPropertyChanged
{
    private string _characterName;
    private Currency _theirs;
    private Currency _mine;
    private DateTime _timestamp;
    private bool _executeEnabled = true;

    public string CharacterName
    {
        get => _characterName;
        set
        {
            if (value == _characterName) return;
            _characterName = value;
            OnPropertyChanged();
        }
    }

    public Currency Theirs
    {
        get => _theirs;
        set
        {
            if (Equals(value, _theirs)) return;
            _theirs = value;
            OnPropertyChanged();
        }
    }

    public Currency Mine
    {
        get => _mine;
        set
        {
            if (Equals(value, _mine)) return;
            _mine = value;
            OnPropertyChanged();
        }
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set
        {
            if (value.Equals(_timestamp)) return;
            _timestamp = value;
            OnPropertyChanged();
        }
    }

    public bool ExecuteEnabled
    {
        get => _executeEnabled;
        set
        {
            if (value == _executeEnabled) return;
            _executeEnabled = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return $"From {CharacterName}: Their {Theirs} for my {Mine}";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public static class AffixMods
{
    public static string[] GetMods()
    {
        string[] mods = new string[0];
        if (File.Exists(@"AffixMods.txt"))
            mods = File.ReadAllLines(@"AffixMods.txt");
        return mods;
    }
}

[DataContract]
[Serializable]
public class Fraction : IEquatable<Fraction>
{
    public Fraction() { }
    public Fraction(int n, int d)
    {
        N = n;
        D = d;
        SimplifyFraction();
    }

    [DataMember]
    public int N;
    [DataMember]
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

    public bool Equals(Fraction other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return N == other.N && D == other.D;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Fraction) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (N * 397) ^ D;
        }
    }
}

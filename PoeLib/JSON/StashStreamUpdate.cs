using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using PoeLib.Annotations;
using System.Text.Json.Serialization;

namespace PoeLib.JSON;

public partial class Requirement
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("values")]
    public object Values { get; set; }

    [JsonPropertyName("displayMode")]
    public int DisplayMode { get; set; }
}

public partial class Property
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("values")]
    public object Value { get; set; }

    [JsonPropertyName("displayMode")]
    public int DisplayMode { get; set; }

    [JsonPropertyName("type")]
    public int? Type { get; set; }
}

public class AdditionalProperty
{
    public string name { get; set; }

    [JsonPropertyName("values")]
    public object Values { get; set; }

    public int displayMode { get; set; }

    public double progress { get; set; }
}

public class Magnitude
{
    public string hash { get; set; }
    public string min { get; set; }
    public string max { get; set; }
	
	[JsonIgnore]
    public decimal MinValue => decimal.Parse(min);
    [JsonIgnore]
    public decimal MaxValue => decimal.Parse(max);
}

public class Implicit
{
    public string name { get; set; }
    public string tier { get; set; }
    public List<Magnitude> magnitudes { get; set; }
}

public class Explicit
{
    public string name { get; set; }
    public string tier { get; set; }
    public List<Magnitude> magnitudes { get; set; }
}

public class Mods
{
    public List<Implicit> @implicit { get; set; }
    public List<Explicit> @explicit { get; set; }
}

public class Hashes
{
    public object @implicit { get; set; }
    public object @explicit { get; set; }
}

public class Extended
{
    public decimal dps { get; set; }
    public decimal pdps { get; set; }
    public decimal edps { get; set; }
    public Mods mods { get; set; }
    public Hashes hashes { get; set; }
    public string text { get; set; }
}

public class Influences
{
    public bool shaper { get; set; }
    public bool elder { get; set; }
    public bool crusader { get; set; }
    public bool redeemer { get; set; }
    public bool hunter { get; set; }
    public bool warlord { get; set; }
}

public class IncubatedItem
{
    public string name { get; set; }
    public int level { get; set; }
    public int progress { get; set; }
    public int total { get; set; }
}

public class Item : IEquatable<Item>
{
    private static Regex cleanupPattern = new Regex(@"<<[\w:]*>>", RegexOptions.Compiled);
    public bool verified { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public string icon { get; set; }
    public string league { get; set; }
    public string id { get; set; }
    public Influences influences { get; set; }
    [JsonPropertyName("sockets")]
    public List<Socket> Sockets { get; set; } = new List<Socket>();
    public string artFilename { get; set; }
    [IgnoreDataMember]
    [JsonIgnore]
    public string whisper_token { get; set; }
    public int NumSockets => Sockets.Count;

    public int NumLinks
    {
        get
        {
            if (Sockets == null || Sockets.Count == 0) return 0;
            return Sockets.GroupBy(x => x.group).Select(g => g.Count()).Max();
        }
    }
    private string name;
    [JsonPropertyName("name")]
    public string Name
    {
        get => string.IsNullOrEmpty(name) ? TypeLine : name.Replace("Superior", "").Replace("Synthesised", "").RemoveDiacritics().Trim();
        set => name = cleanupPattern.Replace(value, "").Trim();
    }

    [JsonPropertyName("typeLine")]
    public string TypeLine { get; set; }

    [JsonPropertyName("baseType")]
    public string BaseType { get; set; } = string.Empty;

    [JsonIgnore]
    public string Term { get; set; }

    public bool identified { get; set; }
    [JsonPropertyName("ilvl")]
    public int ItemLevel { get; set; }
    public bool corrupted { get; set; }
    public bool shaper { get; set; }
    public bool elder { get; set; }
    public bool abyssJewel { get; set; }
    public bool lockedToCharacter { get; set; }
    [JsonPropertyName("note")]
    public string Note { get; set; }
    public bool split { get; set; }
    public List<Property> properties { get; set; }
    public List<Requirement> requirements { get; set; }
    [JsonPropertyName("implicitMods")]
    public List<string> rawImplicitMods { get; set; } = new();
    public List<Mod> ImplicitMods => ItemMods.GetMods(rawImplicitMods);
    [JsonPropertyName("explicitMods")]
    public List<string> rawExplicitMods { get; set; } = new();
    public List<Mod> ExplicitMods => ItemMods.GetMods(rawExplicitMods);
    [JsonPropertyName("fracturedMods")]
    public List<string> rawFracturedMods { get; set; } = new();
    public List<Mod> FracturedMods => ItemMods.GetMods(rawFracturedMods, true);
    public Crucible crucible { get; set; }
    public List<string> veiledMods { get; set; }
    public bool veiled { get; set; }
    public bool fractured { get; set; }
    public bool synthesised { get; set; }
    [JsonPropertyName("frameType")]
    public int frameType { get; set; }
    [JsonIgnore]
    public Rarity rarity
    {
        get => (Rarity)frameType;
        set => frameType = (int)value;
    }
    public bool isRelic { get; set; }
    private int _stackSize;
    public int stackSize
    {
        get => _stackSize == 0 ? 1 : _stackSize;
        set => _stackSize = value;
    }
    public int maxStackSize { get; set; }
    public Dictionary<string, object[]> category { get; set; }
    public Extended extended { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string inventoryId { get; set; }
    public Item[] socketedItems { get; set; }
    public string descrText { get; set; }
    public List<AdditionalProperty> additionalProperties { get; set; }
    public string secDescrText { get; set; }
    public List<string> flavourText { get; set; }
    public List<string> enchantMods { get; set; }
    public List<string> craftedMods { get; set; }
    public IncubatedItem incubatedItem { get; set; }

    public bool ILvlTooLow
    {
        get
        {
            switch (BaseType)
            {
                case "Shabby Jerkin":
                case "Simple Robe":
                case "Padded Vest":
                case "Chainmail Vest":
                case "Scale Vest":
                case "Chestplate":
                case "Light Brigandine":
                case "Chainmail Tunic":
                case "Oiled Vest":
                case "Strapped Leather":
                case "Silken Vest":
                case "Buckskin Tunic":
                case "Copper Plate":
                case "Scale Doublet":
                case "Ringmail Coat":
                case "Scholar's Robe":
                case "Padded Jacket":
                case "Golden Mantle":
                case "Infantry Brigandine":
                case "Chainmail Doublet":
                case "War Plate":
                case "Oiled Coat":
                case "Silken Garb":
                case "Wild Leather":
                case "Full Leather":
                case "Scarlet Raiment":
                case "Mage's Vestment":
                case "Full Plate":
                case "Full Scale Armour":
                case "Full Ringmail":
                case "Full Chainmail":
                case "Soldier's Brigandine":
                case "Arena Plate":
                case "Silk Robe":
                case "Waxed Garb":
                case "Sun Leather":
                case "Holy Chainmail":
                case "Thief's Garb":
                case "Bone Armour":
                case "Cabalist Regalia":
                case "Lordly Plate":
                case "Field Lamellar":
                case "Eelskin Tunic":
                case "Sage's Robe":
                case "Bronze Plate":
                case "Wyrmscale Doublet":
                case "Latticed Ringmail":
                case "Quilted Jacket":
                case "Battle Plate":
                case "Frontier Leather":
                case "Silken Wrap":
                case "Hussar Brigandine":
                case "Crusader Chainmail":
                case "Sleek Coat":
                case "Glorious Leather":
                case "Conjurer's Vestment":
                case "Sun Plate":
                case "Full Wyrmscale":
                case "Ornate Ringmail":
                case "Crimson Raiment":
                case "Colosseum Plate":
                case "Spidersilk Robe":
                case "Coronal Leather":
                case "Commander's Brigandine":
                case "Chain Hauberk":
                case "Lacquered Garb":
                case "Cutthroat's Garb":
                case "Destroyer Regalia":
                case "Majestic Plate":
                case "Battle Lamellar":
                case "Devout Chainmail":
                case "Savant's Robe":
                case "Crypt Armour":
                case "Sharkskin Tunic":
                case "Golden Plate":
                case "Dragonscale Doublet":
                case "Loricated Ringmail":
                case "Crusader Plate":
                case "Sentinel Jacket":
                case "Destiny Leather":
                case "Necromancer Silks":
                case "Desert Brigandine":
                case "Conquest Chainmail":
                case "Exquisite Leather":
                case "Varnished Coat":
                case "Astral Plate":
                case "Occultist's Vestment":
                case "Full Dragonscale":
                case "Elegant Ringmail":
                case "Widowsilk Robe":
                case "Zodiac Leather":
                case "Blood Raiment":
                case "Gladiator Plate":
                case "General's Brigandine":
                case "Saint's Hauberk":
                case "Glorious Plate":
                case "Vaal Regalia":
                case "Sadist Garb":
                case "Assassin's Garb":
                case "Triumphant Lamellar":
                case "Saintly Chainmail":
                case "Carnal Armour":
                case "Sacrificial Garb":
                    if (ItemLevel < 50)
                        return true;
                    break;
                case "Short Bow":
                case "Long Bow":
                case "Composite Bow":
                case "Recurve Bow":
                case "Bone Bow":
                case "Royal Bow":
                case "Hedron Bow":
                case "Death Bow":
                case "Grove Bow":
                case "Reflex Bow":
                case "Decurve Bow":
                case "Compound Bow":
                case "Sniper Bow":
                case "Ivory Bow":
                case "Foundry Bow":
                case "Highborn Bow":
                case "Decimation Bow":
                case "Thicket Bow":
                case "Steelwood Bow":
                case "Citadel Bow":
                case "Ranger Bow":
                case "Assassin Bow":
                case "Spine Bow":
                case "Imperial Bow":
                case "Harbinger Bow":
                case "Solarine Bow":
                case "Maraketh Bow":
                    if (ItemLevel < 50)
                        return true;
                    break;
                case "Gnarled Branch":
                case "Primitive Staff":
                case "Long Staff":
                case "Royal Staff":
                case "Transformer Staff":
                case "Crescent Staff":
                case "Woodful Staff":
                case "Quarterstaff":
                case "Reciprocation Staff":
                case "Highborn Staff":
                case "Moon Staff":
                case "Primordial Staff":
                case "Lathi":
                case "Imperial Staff":
                case "Eclipse Staff":
                case "Battery Staff":
                    if (ItemLevel < 50)
                        return true;
                    break;
                case "Stone Axe":
                case "Jade Chopper":
                case "Woodsplitter":
                case "Poleaxe":
                case "Double Axe":
                case "Gilded Axe":
                case "Prime Cleaver":
                case "Shadow Axe":
                case "Dagger Axe":
                case "Jasper Chopper":
                case "Timber Axe":
                case "Headsman Axe":
                case "Labrys":
                case "Honed Cleaver":
                case "Noble Axe":
                case "Abyssal Axe":
                case "Karui Chopper":
                case "Talon Axe":
                case "Sundering Axe":
                case "Ezomyte Axe":
                case "Vaal Axe":
                case "Despot Axe":
                case "Void Axe":
                case "Fleshripper":
                case "Apex Cleaver":
                    if (ItemLevel < 50)
                        return true;
                    break;
                case "Driftwood Maul":
                case "Tribal Maul":
                case "Mallet":
                case "Sledgehammer":
                case "Jagged Maul":
                case "Brass Maul":
                case "Blunt Force Condenser":
                case "Fright Maul":
                case "Morning Star":
                case "Totemic Maul":
                case "Great Mallet":
                case "Steelhead":
                case "Spiny Maul":
                case "Crushing Force Magnifier":
                case "Plated Maul":
                case "Dread Maul":
                case "Solar Maul":
                case "Karui Maul":
                case "Colossus Mallet":
                case "Piledriver":
                case "Meatgrinder":
                case "Imperial Maul":
                case "Terror Maul":
                case "Coronal Maul":
                case "Impact Force Propagator":
                    if (ItemLevel < 50)
                        return true;
                    break;
                case "Corroded Blade":
                case "Longsword":
                case "Bastard Sword":
                case "Two-Handed Sword":
                case "Etched Greatsword":
                case "Ornate Sword":
                case "Rebuking Blade":
                case "Spectral Sword":
                case "Curved Blade":
                case "Butcher Sword":
                case "Footman Sword":
                case "Highland Blade":
                case "Engraved Greatsword":
                case "Blasting Blade":
                case "Tiger Sword":
                case "Wraith Sword":
                case "Lithe Blade":
                case "Headman's Sword":
                case "Reaver Sword":
                case "Ezomyte Blade":
                case "Vaal Greatsword":
                case "Lion Sword":
                case "Infernal Sword":
                case "Exquisite Blade":
                case "Banishing Blade":
                    if (ItemLevel < 50)
                        return true;
                    break;
            }
            return false;
        }
    }

    [JsonIgnore]
    public string Seller { get; set; }

    [JsonIgnore]
    public string SearchID { get; set; }

    public override string ToString()
    {
        var outString = Name;
        if (!string.IsNullOrEmpty(Note))
            outString += $", {Note}";
        if (!string.IsNullOrEmpty(Seller))
            outString += $", {Seller}";
        return outString;
    }

    public bool Equals(Item other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(name, other.name) && string.Equals(league, other.league) && string.Equals(id, other.id) && string.Equals(Note, other.Note);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Item) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (name != null ? name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (league != null ? league.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (id != null ? id.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Note != null ? Note.GetHashCode() : 0);
            return hashCode;
        }
    }
}

public static class ItemMods
{
    public static List<Mod> GetMods(IEnumerable<string> modStrings, bool isFractured = false)
    {
        List<Mod> mods = new List<Mod>();
        var numberPattern = new Regex(@"[\d]+(\.)?[\d]+");

        foreach (string modString in modStrings)
        {
            Mod newMod = new Mod {RawModText = isFractured ? $"(Fractured){modString}" : modString};
            var modText = modString;
            while (numberPattern.IsMatch(modText))
            {
                var match = numberPattern.Match(modText);
                if (double.TryParse(match.ToString(), out var value))
                {
                    newMod.Values.Add(value);
                }
                modText = numberPattern.Replace(modText, "#", 1);
            }
            newMod.ModText = modText.Trim('?');
            mods.Add(newMod);
        }
        return mods;
    }
}

public class Mod : INotifyPropertyChanged, IEquatable<Mod>
{
    private string modText = "";
    public string ModText
    {
        get { return modText; }
        set
        {
            modText = value;
            OnPropertyChanged();
        }
    }

    
    private string rawModText = "";
    public string RawModText
    {
        get { return rawModText; }
        set
        {
            rawModText = value;
            OnPropertyChanged();
        }
    }
    
    private List<double> values = new List<double>();
    public List<double> Values
    {
        get
        {
            if (!string.IsNullOrEmpty(valueString))
            {
                values.Clear();
                var valArray = valueString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var val in valArray)
                    if (double.TryParse(val, out var result))
                        values.Add(result);
            }
            return values;
        }
        set
        {
            this.values = value;
        }
    }

    private string valueString;
    public string ValueString
    {
        get => valueString;
        set
        {
            valueString = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return ModText;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool Equals(Mod other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(modText, other.modText);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Mod) obj);
    }

    public override int GetHashCode()
    {
        return (modText != null ? modText.GetHashCode() : 0);
    }
}

public class Socket
{
    public int group { get; set; }
    [JsonPropertyName("attr")]
    public string type { get; set; }
    [JsonPropertyName("sColour")]
    public string sColour { get; set; }
    [JsonIgnore]
    public SocketColor color
    {
        get => (SocketColor)Enum.Parse(typeof(SocketColor), sColour);
        set => sColour = value.ToString();
    }
}

public class Stash
{
    public string accountName { get; set; }
    public string lastCharacterName { get; set; }
    public string id { get; set; }
    public string stash { get; set; }
    public string stashType { get; set; }
    public List<Item> items { get; set; }
    public bool @public { get; set; }
}

public class PoeAccountOnlineStatus
{
    public string league { get; set; }
}

public class PoeItemStash
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class PoeAccount
{
    public string name { get; set; }
    public string lastCharacterName { get; set; }
    public PoeAccountOnlineStatus online { get; set; }
    public string language { get; set; }
}

public class PoeItemPrice
{
    public string type { get; set; }
    public decimal amount { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyType currency { get; set; }

    public override string ToString()
    {
        return $"{amount} {currency}";
    }
}

public class PoeItemListing
{
    public string method { get; set; }
    public DateTime indexed { get; set; }
    public PoeItemStash stash { get; set; }
    public string whisper { get; set; }
    public string whisper_token { get; set; }
    public PoeAccount account { get; set; }
    public PoeItemPrice price { get; set; }
}

public class PoeItemSearchResult
{
    public string id { get; set; }
    public PoeItemListing listing { get; set; }
    public Item item { get; set; }
}

public class PoeItemSearchResults
{
    [JsonPropertyName("result")]
    public List<PoeItemSearchResult> result { get; set; }
}

public class PoeItemNewResults
{
    public bool? auth { get; set; }
    public List<string> @new { get; set; }
}

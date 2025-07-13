using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PoeLib.JSON;
using PoeLib.Parsers;

namespace PoeLib.GuiDataClasses;

[Serializable]
public class SearchGuiItem : INotifyPropertyChanged
{
    public SearchGuiItem() { }

    public SearchGuiItem(string name)
    {
        this.name = name;
    }

    private string name;
    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    private Rarity rarity;
    public Rarity Rarity
    {
        get => rarity;
        set
        {
            rarity = value;
            OnPropertyChanged();
        }
    }

    private bool enabled;
    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            OnPropertyChanged();
        }
    }

    public string DisplayName => string.IsNullOrEmpty(Name) ? BaseType : Name;

    private string searchId = "";
    public string SearchID
    {
        get => searchId;
        set
        {
            searchId = value;
            OnPropertyChanged();
        }
    }

    private string baseType;
    public string BaseType
    {
        get => baseType;
        set
        {
            baseType = value;
            OnPropertyChanged();
        }
    }

    private Price offerPrice;
    public Price OfferPrice
    {
        get => offerPrice ?? (offerPrice = new Price());
        set
        {
            offerPrice = value;
            OnPropertyChanged();
        }
    }

    private bool corrupted;
    public bool AllowCorrupted
    {
        get => corrupted;
        set
        {
            corrupted = value;
            OnPropertyChanged();
        }
    }

    private bool veiled;
    public bool Veiled
    {
        get => veiled;
        set
        {
            veiled = value;
            OnPropertyChanged();
        }
    }

    private List<Mod> explicitMods;
    public List<Mod> ExplicitMods
    {
        get => explicitMods ?? (explicitMods = new List<Mod>());
        set
        {
            explicitMods = value;
            OnPropertyChanged();
        }
    }

    private int sockets;
    public int Sockets
    {
        get => sockets;
        set
        {
            sockets = value;
            OnPropertyChanged();
        }
    }

    private int mapTier;
    public int MapTier
    {
        get => mapTier;
        set
        {
            mapTier = value;
            OnPropertyChanged();
        }
    }

    private int links;
    public int Links
    {
        get => links;
        set
        {
            links = value;
            OnPropertyChanged();
        }
    }

    private int itemLevel;
    public int ItemLevel
    {
        get => itemLevel;
        set
        {
            itemLevel = value;
            OnPropertyChanged();
        }
    }

    private bool checkAffixCount;
    public bool CheckAffixCount
    {
        get => checkAffixCount;
        set
        {
            checkAffixCount = value;
            OnPropertyChanged();
        }
    }

    private int affixCount;
    public int AffixCount
    {
        get => affixCount;
        set
        {
            affixCount = value;
            OnPropertyChanged();
        }
    }

    private string variant;
    public string Variant
    {
        get => variant;
        set
        {
            variant = value;
            OnPropertyChanged();
        }
    }

    private string source;
    public string Source
    {
        get => source;
        set
        {
            source = value;
            OnPropertyChanged();
        }
    }

    private bool isCurrency;
    public bool IsCurrency
    {
        get { return isCurrency; }
        set
        {
            isCurrency = value;
            OnPropertyChanged();
        }
    }

    private CurrencyType currencyType;
    public CurrencyType CurrencyType
    {
        get => currencyType;
        set
        {
            currencyType = value;
            OnPropertyChanged();
        }
    }

    private int minChaos;
    public int MinChaos
    {
        get => minChaos;
        set
        {
            minChaos = value;
            OnPropertyChanged();
        }
    }

    private int buyThreshold;
    public int BuyThreshold
    {
        get => buyThreshold;
        set
        {
            buyThreshold = value;
            OnPropertyChanged();
        }
    }

    private int minStock;
    public int MinStock
    {
        get => minStock;
        set
        {
            minStock = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        return obj is SearchGuiItem item &&
               Rarity == item.Rarity &&
               DisplayName == item.DisplayName &&
               SearchID == item.SearchID &&
               Veiled == item.Veiled &&
               Sockets == item.Sockets &&
               MapTier == item.MapTier &&
               Links == item.Links &&
               ItemLevel == item.ItemLevel &&
               Variant == item.Variant;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(Rarity);
        hash.Add(DisplayName);
        hash.Add(SearchID);
        hash.Add(Veiled);
        hash.Add(Sockets);
        hash.Add(MapTier);
        hash.Add(Links);
        hash.Add(ItemLevel);
        hash.Add(Variant);
        return hash.ToHashCode();
    }
}

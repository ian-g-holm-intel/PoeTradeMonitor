using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using PoeLib.JSON;
using PoeLib.Parsers;

namespace PoeLib.GuiDataClasses;

public class StashGuiItem : INotifyPropertyChanged
{
    private string timestamp;
    public string Timestamp
    {
        get { return timestamp; }
        set
        {
            timestamp = value;
            OnPropertyChanged();
        }
    }

    private string name;
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    private string searchID;
    public string SearchID
    {
        get { return searchID; }
        set
        {
            searchID = value;
            OnPropertyChanged();
        }
    }

    private string itemID;
    public string ItemID
    {
        get => itemID;
        set
        {
            itemID = value;
            OnPropertyChanged();
        }
    }

    private bool highFrequency;
    public bool HighFrequency
    {
        get => highFrequency;
        set
        {
            highFrequency = value;
            OnPropertyChanged();
        }
    }

    private Rarity rarity;
    public Rarity Rarity
    {
        get { return rarity; }
        set
        {
            rarity = value;
            OnPropertyChanged();
        }
    }

    private Price price;
    public Price Price
    {
        get { return price; }
        set
        {
            price = value;
            OnPropertyChanged();
        }
    }

    private Price msrp;

    public Price MSRP
    {
        get { return msrp; }
        set
        {
            msrp = value;
            OnPropertyChanged();
        }
    }

    private int stackSize;
    public int StackSize
    {
        get { return stackSize; }
        set
        {
            stackSize = value;
            OnPropertyChanged();
        }
    }

    private int numSockets;
    public int NumSockets
    {
        get { return numSockets; }
        set
        {
            numSockets = value;
            OnPropertyChanged();
        }
    }

    private int numLinks;
    public int NumLinks
    {
        get { return numLinks; }
        set
        {
            numLinks = value;
            OnPropertyChanged();
        }
    }

    private int itemLevel;
    public int ItemLevel
    {
        get { return itemLevel; }
        set
        {
            itemLevel = value;
            OnPropertyChanged();
        }
    }

    private List<Mod> explicitMods;
    public List<Mod> ExplicitMods
    {
        get
        {
            return explicitMods;
        }
        set
        {
            explicitMods = value;
            OnPropertyChanged();
        }
    }

    private List<Mod> fracturedMods;
    public List<Mod> FracturedMods
    {
        get
        {
            return fracturedMods;
        }
        set
        {
            fracturedMods = value;
            OnPropertyChanged();
        }
    }

    private string whisperToken;
    public string WhisperToken
    {
        get { return whisperToken; }
        set
        {
            whisperToken = value;
            OnPropertyChanged();
        }
    }

    private int whisperValue;
    public int WhisperValue
    {
        get { return whisperValue; }
        set
        {
            whisperValue = value;
            OnPropertyChanged();
        }
    }

    public string ExplicitModsDisplayString => fracturedMods.Concat(ExplicitMods).Aggregate("", (current, mod) => current + Environment.NewLine + mod.RawModText).TrimStart('\r', '\n');

    private string character;
    public string Character
    {
        get { return character; }
        set
        {
            character = value;
            OnPropertyChanged();
        }
    }

    private string account;
    public string Account
    {
        get { return account; }
        set
        {
            account = value;
            OnPropertyChanged();
        }
    }

    private bool executeEnabled;
    public bool ExecuteEnabled
    {
        get => executeEnabled;
        set
        {
            if (value == executeEnabled) return;
            executeEnabled = value;
            OnPropertyChanged();
        }
    }

    private string league;
    public string League
    {
        get => league;
        set
        {
            league = value;
            OnPropertyChanged();
        }
    }

    private string stash;
    public string Stash
    {
        get => stash;
        set
        {
            stash = value;
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

    private Point location;
    public Point Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged();
        }
    }

    private ServiceLocation serviceLocation;
    public ServiceLocation ServiceLocation
    {
        get => serviceLocation;
        set
        {
            serviceLocation = value;
            OnPropertyChanged();
        }
    }

    public ItemTradeRequest TradeRequest { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Character}, {Account}, {Price}, {Source}, {Rarity}, Sockets: {NumSockets}, Links: {NumLinks}, ItemLevel: {ItemLevel}";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override bool Equals(object obj)
    {
        return obj is StashGuiItem item && ItemID == item.ItemID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ItemID);
    }
}

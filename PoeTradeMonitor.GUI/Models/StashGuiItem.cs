using CommunityToolkit.Mvvm.ComponentModel;
using PoeLib.Common;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Models;

/// <summary>
/// Represents a stash item available for trading with observable properties for GUI data binding.
/// Contains item details, pricing information, and trade execution data.
/// </summary>
public partial class StashGuiItem : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StashGuiItem"/> class with the specified trade request.
    /// </summary>
    /// <param name="tradeRequest">The trade request associated with this stash item.</param>
    public StashGuiItem(ItemTradeRequest tradeRequest)
    {
        TradeRequest = tradeRequest;
    }

    [ObservableProperty]
    private string timestamp = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string searchID = string.Empty;

    [ObservableProperty]
    private string itemID = string.Empty;

    [ObservableProperty]
    private ItemRarity rarity;

    [ObservableProperty]
    private PriceInfo price = new();

    public List<CurrencyInfo> Currencies = new();

    [ObservableProperty]
    private int stackSize;

    [ObservableProperty]
    private int numSockets;

    [ObservableProperty]
    private List<Mod> explicitMods = new();

    [ObservableProperty]
    private List<Mod> fracturedMods = new();

    [ObservableProperty]
    private string whisperToken = string.Empty;

    [ObservableProperty]
    private int whisperValue;

    /// <summary>
    /// Gets a display string containing all explicit and fractured mods formatted for UI display.
    /// </summary>
    public string ExplicitModsDisplayString => FracturedMods.Concat(ExplicitMods).Aggregate("", (current, mod) => current + Environment.NewLine + mod.RawModText).TrimStart('\r', '\n');

    [ObservableProperty]
    private string character = string.Empty;

    [ObservableProperty]
    private string account = string.Empty;

    [ObservableProperty]
    private bool executeEnabled;

    [ObservableProperty]
    private string source = string.Empty;

    [ObservableProperty]
    private ServiceLocation serviceLocation;

    /// <summary>
    /// Gets or sets the trade request associated with this stash item.
    /// </summary>
    public ItemTradeRequest TradeRequest { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Character}, {Account}, {Price}, {Source}, {Rarity}, Sockets: {NumSockets}";
    }
}

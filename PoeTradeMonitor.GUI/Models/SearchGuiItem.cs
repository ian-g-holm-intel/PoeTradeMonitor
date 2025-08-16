using CommunityToolkit.Mvvm.ComponentModel;
using PoeTrade.Contracts;

namespace PoeTradeMonitor.GUI.Models;

/// <summary>
/// Represents a search item configuration for the GUI with observable properties for data binding.
/// Contains all the criteria and settings needed to search for items in Path of Exile trading.
/// </summary>
public partial class SearchGuiItem : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchGuiItem"/> class.
    /// </summary>
    public SearchGuiItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchGuiItem"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the search item.</param>
    public SearchGuiItem(string name)
    {
        Name = name;
    }

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool enabled;

    [ObservableProperty]
    private bool isTypeName;

    [ObservableProperty]
    private string searchID = string.Empty;

    [ObservableProperty]
    private string autoSearchID = string.Empty;

    [ObservableProperty]
    private PriceInfo offerPrice = new();

    [ObservableProperty]
    private string source = string.Empty;

    [ObservableProperty]
    private bool isPlusOneCorruption;

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is SearchGuiItem item &&
               Name == item.Name &&
               SearchID == item.SearchID &&
			   OfferPrice.Equals(item.OfferPrice);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(Name);
        hash.Add(SearchID);
		hash.Add(OfferPrice);
        return hash.ToHashCode();
    }
}

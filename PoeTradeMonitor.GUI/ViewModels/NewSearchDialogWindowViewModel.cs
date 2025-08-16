using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoeLib.Common;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.Views;

namespace PoeTradeMonitor.GUI.ViewModels;

public partial class NewSearchDialogWindowViewModel : ObservableObject
{
    [RelayCommand]
    private void ExecuteOK(NewSearchDialogWindow? window)
    {
        if (window == null) return;
        window.DialogResult = true;
        window.Close();
    }

    [RelayCommand]
    private void ExecuteCancel(NewSearchDialogWindow? window)
    {
        if (window == null) return;
        window.DialogResult = false;
        window.Close();
    }

    public NewSearchDialogWindowViewModel()
    {
        AffixList = AffixMods.GetMods();
        SearchEnabled = true;
        IsEditing = false;
    }

    public NewSearchDialogWindowViewModel(SearchGuiItem searchItem)
    {
        AffixList = AffixMods.GetMods();

        ItemName = searchItem.Name;
        SearchEnabled = searchItem.Enabled;
        SearchID = searchItem.SearchID;
        IsEditing = true;
        IsDivineOrbs = searchItem.OfferPrice.CurrencyType == TradeCurrencyType.Divine;
        Amount = searchItem.OfferPrice.Amount;
		IsPlusOneCorruption = searchItem.IsPlusOneCorruption;
    }

    public SearchGuiItem Item => new SearchGuiItem(ItemName)
    {
        Enabled = SearchEnabled,
        SearchID = SearchID,
        Source = "GUI",
        OfferPrice = new PriceInfo()
        {
            CurrencyType = IsDivineOrbs ? TradeCurrencyType.Divine : Constants.BaseCurrencyType,
            Amount = Amount,
            IsRelative = string.IsNullOrEmpty(SearchID)
        },
        IsPlusOneCorruption = IsPlusOneCorruption
    };

    public string[] AffixList { get; set; }

    [ObservableProperty]
    private bool isEditing = false;

    [ObservableProperty]
    private bool isTypeName = false;

    [ObservableProperty]
    private string itemName = "";

    [ObservableProperty]
    private string searchID = "";

    [ObservableProperty]
    private bool searchEnabled;

    [ObservableProperty]
    private bool isPlusOneCorruption;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBaseCurrency))]
    private bool isDivineOrbs = true;

    public bool IsBaseCurrency
    {
        get => !IsDivineOrbs;
        set
        {
            IsDivineOrbs = !value;
            {
                OnPropertyChanged(nameof(IsDivineOrbs));
            }
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Amount))]
    private string amountText = "";

    public decimal Amount
    {
        get
        {
            if (decimal.TryParse(AmountText, out decimal result))
                return result;
            return 0;
        }
        set
        {
            AmountText = value.ToString();
        }
    }
}
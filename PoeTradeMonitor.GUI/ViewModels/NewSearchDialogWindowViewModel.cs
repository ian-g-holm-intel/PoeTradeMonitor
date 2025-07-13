using System.Windows.Input;
using PoeLib;
using PoeLib.GuiDataClasses;
using PoeLib.Parsers;
using PoeTradeMonitor.GUI.Commands;
using PoeTradeMonitor.GUI.Views;

namespace PoeTradeMonitor.GUI.ViewModels;

public class NewSearchDialogWindowViewModel : ViewModelBase
{
    public ICommand OKCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public NewSearchDialogWindowViewModel()
    {
        OKCommand = new DelegateCommand<NewSearchDialogWindow>(ExecuteOKCommand);
        CancelCommand = new DelegateCommand<NewSearchDialogWindow>(ExecuteCancelCommand);
        AffixList = AffixMods.GetMods();

        SearchEnabled = true;
        IsEditing = false;
    }

    public NewSearchDialogWindowViewModel(SearchGuiItem searchItem)
    {
        OKCommand = new DelegateCommand<NewSearchDialogWindow>(ExecuteOKCommand);
        CancelCommand = new DelegateCommand<NewSearchDialogWindow>(ExecuteCancelCommand);
        AffixList = AffixMods.GetMods();

        ItemName = searchItem.Name;
        SearchEnabled = searchItem.Enabled;
        SearchID = searchItem.SearchID;
        IsEditing = true;
        IsCurrency = searchItem.IsCurrency;
        CurrencyType = searchItem.CurrencyType;
        MinChaos = searchItem.MinChaos;
        BuyThreshold = searchItem.BuyThreshold;
        MinStock = searchItem.MinStock;
        IsDivineOrbs = searchItem.OfferPrice.Currencies.Any(c => c.Type == CurrencyType.divine);
        Amount = searchItem.OfferPrice.Currencies.FirstOrDefault()?.Amount ?? 0;
    }

    public SearchGuiItem Item => new SearchGuiItem(ItemName)
    {
        Enabled = SearchEnabled,
        SearchID = SearchID,
        Source = "GUI",
        IsCurrency = IsCurrency,
        OfferPrice = new Price(IsDivineOrbs ? CurrencyType.divine : CurrencyType.chaos, Amount),
        CurrencyType = CurrencyType,
        MinChaos = MinChaos,
        BuyThreshold = BuyThreshold,
        MinStock = MinStock,
    };

    public string[] AffixList { get; set; }

    private bool isEditing = false;
    public bool IsEditing
    {
        get => isEditing;
        set
        {
            isEditing = value;
            RaisePropertyChanged();
        }
    }

    private void ExecuteOKCommand(NewSearchDialogWindow window)
    {
        if (window == null) return;
        window.DialogResult = true;
        window.Close();
    }

    private void ExecuteCancelCommand(NewSearchDialogWindow window)
    {
        if (window == null) return;
        window.DialogResult = false;
        window.Close();
    }

    private string itemName = "";
    public string ItemName
    {
        get { return itemName; }
        set
        {
            itemName = value;
            RaisePropertyChanged();
        }
    }

    private string searchId = "";
    public string SearchID
    {
        get { return searchId; }
        set
        {
            searchId = value;
            RaisePropertyChanged();
        }
    }

    private bool searchEnabled;
    public bool SearchEnabled
    {
        get { return searchEnabled; }
        set
        {
            searchEnabled = value;
            RaisePropertyChanged();
        }
    }

    private bool isDivineOrbs = true;
    public bool IsDivineOrbs
    {
        get => isDivineOrbs;
        set
        {
            isDivineOrbs = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsChaosOrbs));
        }
    }

    public bool IsChaosOrbs
    {
        get => !isDivineOrbs;
        set
        {
            isDivineOrbs = !value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsDivineOrbs));
        }
    }

    private string amountText;
    public string AmountText
    {
        get => amountText;
        set
        {
            amountText = value;
            RaisePropertyChanged();
        }
    }

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
            amountText = value.ToString();
            RaisePropertyChanged("AmountText");
        }
    }

    private bool isCurrency;
    public bool IsCurrency
    {
        get { return isCurrency; }
        set
        {
            isCurrency = value;
            RaisePropertyChanged();
            RaisePropertyChanged("IsCrucibleItemEnabled");
        }
    }

    private bool isCurrencyEnabled = true;
    public bool IsCurrencyEnabled
    {
        get { return isCurrencyEnabled; }
        set
        {
            isCurrencyEnabled = value;
            RaisePropertyChanged();
        }
    }

    private CurrencyType currencyType;
    public CurrencyType CurrencyType
    {
        get { return currencyType; }
        set
        {
            currencyType = value;
            RaisePropertyChanged();
        }
    }

    public IEnumerable<CurrencyType> CurrencyTypeValues
    {
        get { return EnumHelper.GetValues<CurrencyType>(); }
    }

    private int minChaos;
    public int MinChaos
    {
        get { return minChaos; }
        set
        {
            minChaos = value;
            RaisePropertyChanged();
        }
    }

    private int buyThreshold;
    public int BuyThreshold
    {
        get { return buyThreshold; }
        set
        {
            buyThreshold = value;
            RaisePropertyChanged();
        }
    }

    private int minStock;
    public int MinStock
    {
        get { return minStock; }
        set
        {
            minStock = value;
            RaisePropertyChanged();
        }
    }
}
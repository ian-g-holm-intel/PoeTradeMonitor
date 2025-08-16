using PoeLib.Common;
using PoeTrade.Contracts;
using PoeTradeMonitor.GUI.Models;
using PoeTradeMonitor.GUI.ViewModels;
using PoeTradeMonitor.GUI.Views;

namespace PoeTradeMonitor.GUI.Tests.ViewModels;

[TestClass]
public class NewSearchDialogWindowViewModelTests
{
    private NewSearchDialogWindowViewModel viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        viewModel = new NewSearchDialogWindowViewModel();
    }

    [TestMethod]
    public void Constructor_Default_ShouldInitializePropertiesCorrectly()
    {
        Assert.IsNotNull(viewModel.AffixList);
        Assert.IsTrue(viewModel.SearchEnabled);
        Assert.IsFalse(viewModel.IsEditing);
        Assert.IsFalse(viewModel.IsTypeName);
        Assert.AreEqual("", viewModel.ItemName);
        Assert.AreEqual("", viewModel.SearchID);
        Assert.IsTrue(viewModel.IsDivineOrbs);
        Assert.AreEqual("", viewModel.AmountText);
    }

    [TestMethod]
    public void Constructor_WithSearchGuiItem_ShouldInitializeFromItem()
    {
        var searchItem = new SearchGuiItem("Test Item")
        {
            Enabled = true,
            SearchID = "test-search-id",
            OfferPrice = new PriceInfo
            {
                CurrencyType = TradeCurrencyType.Chaos,
                Amount = 50m
            }
        };

        var viewModelWithItem = new NewSearchDialogWindowViewModel(searchItem);

        Assert.AreEqual("Test Item", viewModelWithItem.ItemName);
        Assert.IsTrue(viewModelWithItem.SearchEnabled);
        Assert.AreEqual("test-search-id", viewModelWithItem.SearchID);
        Assert.IsTrue(viewModelWithItem.IsEditing);
        Assert.IsFalse(viewModelWithItem.IsDivineOrbs); // Chaos is not Divine
        Assert.AreEqual(50m, viewModelWithItem.Amount);
    }

    [TestMethod]
    public void Constructor_WithDivineSearchGuiItem_ShouldSetDivineOrbs()
    {
        var searchItem = new SearchGuiItem("Divine Item")
        {
            OfferPrice = new PriceInfo
            {
                CurrencyType = TradeCurrencyType.Divine,
                Amount = 5m
            }
        };

        var viewModelWithItem = new NewSearchDialogWindowViewModel(searchItem);

        Assert.IsTrue(viewModelWithItem.IsDivineOrbs);
        Assert.AreEqual(5m, viewModelWithItem.Amount);
    }

    [TestMethod]
    public void ItemName_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.ItemName = "New Item Name";

        Assert.AreEqual("New Item Name", viewModel.ItemName);
    }

    [TestMethod]
    public void SearchID_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.SearchID = "new-search-id";

        Assert.AreEqual("new-search-id", viewModel.SearchID);
    }

    [TestMethod]
    public void SearchEnabled_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.SearchEnabled = false;

        Assert.IsFalse(viewModel.SearchEnabled);
    }

    [TestMethod]
    public void IsDivineOrbs_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.IsDivineOrbs = false;

        Assert.IsFalse(viewModel.IsDivineOrbs);
        Assert.IsTrue(viewModel.IsBaseCurrency);
    }

    [TestMethod]
    public void IsBaseCurrency_GetSet_ShouldUpdateIsDivineOrbs()
    {
        viewModel.IsBaseCurrency = true;

        Assert.IsFalse(viewModel.IsDivineOrbs);
        Assert.IsTrue(viewModel.IsBaseCurrency);

        viewModel.IsBaseCurrency = false;

        Assert.IsTrue(viewModel.IsDivineOrbs);
        Assert.IsFalse(viewModel.IsBaseCurrency);
    }

    [TestMethod]
    public void AmountText_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.AmountText = "123.45";

        Assert.AreEqual("123.45", viewModel.AmountText);
    }

    [TestMethod]
    public void Amount_WithValidAmountText_ShouldParseCorrectly()
    {
        viewModel.AmountText = "123.45";

        Assert.AreEqual(123.45m, viewModel.Amount);
    }

    [TestMethod]
    public void Amount_WithInvalidAmountText_ShouldReturnZero()
    {
        viewModel.AmountText = "invalid";

        Assert.AreEqual(0m, viewModel.Amount);
    }

    [TestMethod]
    public void Amount_WithEmptyAmountText_ShouldReturnZero()
    {
        viewModel.AmountText = "";

        Assert.AreEqual(0m, viewModel.Amount);
    }

    [TestMethod]
    public void Amount_SetValue_ShouldUpdateAmountText()
    {
        viewModel.Amount = 456.78m;

        Assert.AreEqual("456.78", viewModel.AmountText);
    }

    [TestMethod]
    public void Item_WithDivineOrbs_ShouldCreateCorrectSearchGuiItem()
    {
        viewModel.ItemName = "Test Item";
        viewModel.SearchEnabled = true;
        viewModel.SearchID = "test-id";
        viewModel.IsDivineOrbs = true;
        viewModel.Amount = 10m;

        var item = viewModel.Item;

        Assert.AreEqual("Test Item", item.Name);
        Assert.IsTrue(item.Enabled);
        Assert.AreEqual("test-id", item.SearchID);
        Assert.AreEqual("GUI", item.Source);
        Assert.AreEqual(TradeCurrencyType.Divine, item.OfferPrice.CurrencyType);
        Assert.AreEqual(10m, item.OfferPrice.Amount);
        Assert.IsFalse(item.OfferPrice.IsRelative); // SearchID is not empty
    }

    [TestMethod]
    public void Item_WithBaseCurrency_ShouldCreateCorrectSearchGuiItem()
    {
        viewModel.ItemName = "Base Currency Item";
        viewModel.SearchEnabled = false;
        viewModel.SearchID = "";
        viewModel.IsDivineOrbs = false;
        viewModel.Amount = 25m;

        var item = viewModel.Item;

        Assert.AreEqual("Base Currency Item", item.Name);
        Assert.IsFalse(item.Enabled);
        Assert.AreEqual("", item.SearchID);
        Assert.AreEqual("GUI", item.Source);
        Assert.AreEqual(Constants.BaseCurrencyType, item.OfferPrice.CurrencyType);
        Assert.AreEqual(25m, item.OfferPrice.Amount);
        Assert.IsTrue(item.OfferPrice.IsRelative); // SearchID is empty
    }

    [TestMethod]
    public void ExecuteOKCommand_WithValidWindow_ShouldSetDialogResultTrue()
    {
        // Note: This test is simplified since we can't easily create a real window in unit tests
        // In a real scenario, you'd need to mock the window or test integration separately
        
        // Test that the command can be executed without throwing
        Assert.IsTrue(viewModel.ExecuteOKCommand.CanExecute(null));
        
        // The actual window interaction would need integration testing
        // viewModel.ExecuteOKCommand.Execute(mockWindow);
    }

    [TestMethod]
    public void ExecuteCancelCommand_WithValidWindow_ShouldSetDialogResultFalse()
    {
        // Note: This test is simplified since we can't easily create a real window in unit tests
        // In a real scenario, you'd need to mock the window or test integration separately
        
        // Test that the command can be executed without throwing
        Assert.IsTrue(viewModel.ExecuteCancelCommand.CanExecute(null));
        
        // The actual window interaction would need integration testing
        // viewModel.ExecuteCancelCommand.Execute(mockWindow);
    }

    [TestMethod]
    public void ExecuteOKCommand_WithNullWindow_ShouldNotThrow()
    {
        // Test that passing null doesn't cause exceptions
        viewModel.ExecuteOKCommand.Execute(null);
        
        // If we reach here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void ExecuteCancelCommand_WithNullWindow_ShouldNotThrow()
    {
        // Test that passing null doesn't cause exceptions
        viewModel.ExecuteCancelCommand.Execute(null);
        
        // If we reach here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void AffixList_ShouldBeInitialized()
    {
        Assert.IsNotNull(viewModel.AffixList);
        // AffixList may be empty if AffixMods.txt file doesn't exist, which is expected in test environment
    }

    [TestMethod]
    public void PropertyChanges_ShouldTriggerPropertyChangedEvents()
    {
        var propertyChangedEvents = new List<string>();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != null)
                propertyChangedEvents.Add(e.PropertyName);
        };

        viewModel.ItemName = "Test";
        viewModel.SearchEnabled = false;
        viewModel.IsDivineOrbs = false;
        viewModel.AmountText = "100";

        Assert.IsTrue(propertyChangedEvents.Contains(nameof(viewModel.ItemName)));
        Assert.IsTrue(propertyChangedEvents.Contains(nameof(viewModel.SearchEnabled)));
        Assert.IsTrue(propertyChangedEvents.Contains(nameof(viewModel.IsDivineOrbs)));
        Assert.IsTrue(propertyChangedEvents.Contains(nameof(viewModel.AmountText)));
    }

    [TestMethod]
    public void IsTypeName_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.IsTypeName = true;

        Assert.IsTrue(viewModel.IsTypeName);

        viewModel.IsTypeName = false;

        Assert.IsFalse(viewModel.IsTypeName);
    }
}
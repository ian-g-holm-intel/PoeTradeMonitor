using PoeHudWrapper;
using InputSimulatorStandard.Native;
using Microsoft.Extensions.DependencyInjection;
using PoeTrade.Contracts;
using PoeLib;

namespace PoeTradeMonitor.Service.Tests;

[TestClass]
public class PoeHUDWrapperTests
{
    private IPoeHudWrapper poeHudWrapper;
    private ITradeCommands tradeCommands;
    private ServiceProvider serviceProvider;

    public PoeHUDWrapperTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddPoeLib();
        serviceCollection.AddPoeTradeMonitorService();
        serviceCollection.AddPoeHudWrapper();
        serviceProvider = serviceCollection.BuildServiceProvider();
        poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
        tradeCommands = serviceProvider.GetRequiredService<ITradeCommands>();
    }

    [TestCleanup]

    public void Teardown()
    {
        try
        {
            serviceProvider.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to deinitialize test");
            Console.WriteLine(ex);
        }
    }

    [TestMethod]
    public void DestroyDialogVisible()
    {
        Assert.IsTrue(poeHudWrapper.DestroyConfirmationVisible);
    }

    [TestMethod]
    public void GetDestroyKeepButtonLocation()
    {
        var location = poeHudWrapper.DestroyConfirmationKeepButtonLocation;
        Assert.IsTrue(location.X > 0);
        Assert.IsTrue(location.Y > 0);
    }

    [TestMethod]
    public async Task StashOpenAndClose()
    {
        await tradeCommands.CloseAllPanels();

        Assert.IsFalse(poeHudWrapper.StashOpen);

        await tradeCommands.OpenStash();

        Assert.IsTrue(poeHudWrapper.StashOpen);

        await tradeCommands.CloseAllPanels();

        Assert.IsFalse(poeHudWrapper.StashOpen);
    }

    [TestMethod]
    public async Task SwitchTabs()
    {
        tradeCommands.ActivatePoe();
        await poeHudWrapper.SwitchToTab("Sale");
    }

    [TestMethod]
    public void AreaName()
    {
        Assert.AreEqual("Immaculate Hideout", poeHudWrapper.AreaName);
    }

    [TestMethod]
    public void InGame()
    {
        Assert.IsTrue(poeHudWrapper.InGame);
    }

    [TestMethod]
    public void IsAtLogin()
    {
        Assert.IsTrue(poeHudWrapper.IsAtLogin);
    }

    [TestMethod]
    public void IsLoading()
    {
        Assert.IsTrue(poeHudWrapper.IsLoading);
    }

    [TestMethod]
    public void CharacterInZone()
    {
        using var ctSource = new CancellationTokenSource(10000);
        while (!ctSource.Token.IsCancellationRequested && poeHudWrapper.CharactersInArea.Length == 0)
        {
            Thread.Sleep(10);
        }

        foreach (var player in poeHudWrapper.CharactersInArea)
            Assert.IsTrue(poeHudWrapper.PlayerInZone(player));
    }

    [TestMethod]
    public void GetCraftingSlotMods()
    {
        var mods = poeHudWrapper.CraftingSlotMods;
        Assert.IsTrue(mods.Length > 0);
    }

    [TestMethod]
    public void PlayerItems()
    {
        var items = poeHudWrapper.PlayerInventoryItems.Select(inventoryItem => inventoryItem.Item);
        foreach (var item in items)
        {
            var name = poeHudWrapper.GetItemName(item);
            var baseType = poeHudWrapper.GetBaseType(item);
            var numSockets = item.GetNumberSockets();
            var numLinks = item.GetNumberLinks();
            var corrupted = item.IsCorrupted();
            var itemLevel = item.GetItemLevel();
            var className = poeHudWrapper.GetClassName(item);
        }
    }

    [TestMethod]
    public void PlayerInventoryCurrency()
    {
        var currency = poeHudWrapper.PlayerInventoryCurrency;
        Assert.IsTrue(currency.Count >= 1);
    }

    [TestMethod]
    public void GetPlayerInventoryItems()
    {
        var items = poeHudWrapper.PlayerInventoryItems.ToList();
        Assert.IsTrue(items.Count() >= 1);
    }

    [TestMethod]
    public void IsItemCurrency()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isItemCurrency = poeHudWrapper.IsItemCurrency(item.Item);
            Assert.IsTrue(isItemCurrency);
        }
    }

    [TestMethod]
    public void IsItemUniqueRing()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isUniqueRing = poeHudWrapper.IsItemUniqueRing(item.Item);
            Assert.IsTrue(isUniqueRing);
        }
    }

    [TestMethod]
    public void IsItemFragment()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isFragment = poeHudWrapper.IsItemFragment(item.Item);
            Assert.IsTrue(isFragment);
        }
    }

    [TestMethod]
    public void IsItemMap()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isMap = poeHudWrapper.IsItemMap(item.Item);
            Assert.IsTrue(isMap);
        }
    }

    [TestMethod]
    public async Task SocialPanel()
    {
        await tradeCommands.CloseAllPanels();

        Assert.IsFalse(poeHudWrapper.SocialPanelOpen);

        await tradeCommands.OpenPartyScreen();

        Assert.IsTrue(poeHudWrapper.PartyTabVisible);

        Assert.IsTrue(poeHudWrapper.SocialPanelOpen);

        await tradeCommands.CloseAllPanels();
    }

    [TestMethod]
    public async Task GetCurrencyLocation()
    {
        await tradeCommands.CloseAllPanels();

        await tradeCommands.OpenStash();
        await poeHudWrapper.SwitchToTab("$");

        var total = poeHudWrapper.GetStashCurrencyOfType(TradeCurrencyType.Chaos).Sum(s => s.Amount);
        Assert.IsTrue(total >= 1);

        await tradeCommands.CloseAllPanels();
    }

    [TestMethod]
    public async Task Chat()
    {
        if (poeHudWrapper.ChatVisible)
        {
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.IsFalse(poeHudWrapper.ChatVisible);
        }
        else
        {
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.IsTrue(poeHudWrapper.ChatVisible);
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.IsFalse(poeHudWrapper.ChatVisible);
        }
    }

    [TestMethod]
    public void ChatMessage()
    {
        var chatMessage = poeHudWrapper.ChatText;
        Assert.AreEqual("test", chatMessage);
    }

    [TestMethod]
    public async Task GetFreeItemSlot()
    {
        var freeItemSlot = poeHudWrapper.FreeItemSlot;
        Assert.IsTrue(freeItemSlot.X >= 0);
        Assert.IsTrue(freeItemSlot.Y >= 0);

        await tradeCommands.CloseAllPanels();
    }

    [TestMethod]
    public void GetCraftingSlotLocation()
    {
        var craftingSlotLocation = poeHudWrapper.CraftingSlotLocation;
        Assert.IsTrue(craftingSlotLocation.X >= 0);
        Assert.IsTrue(craftingSlotLocation.Y >= 0);
    }

    [TestMethod]
    public void CraftingSlotItem()
    {
        var craftingSlotItem = poeHudWrapper.CraftingSlotItem;
        Assert.IsTrue(craftingSlotItem?.IsValid == true);
    }

    [TestMethod]
    public void ItemOnCursor()
    {
        var itemOnCursor = poeHudWrapper.ItemOnCursor;
        Assert.IsTrue(itemOnCursor);
    }

    [TestMethod]
    public void PartyTabLocation()
    {
        var partyTabLocation = poeHudWrapper.PartyTabLocation;
        Assert.IsTrue(partyTabLocation.X > 0);
        Assert.IsTrue(partyTabLocation.Y > 0);
    }

    [TestMethod]
    public void IsInventorySelected()
    {
        var isSelected = poeHudWrapper.IsInventorySelected(new Point(0, 0));
        Assert.IsTrue(isSelected);
    }

    [TestMethod]
    public void StashCurrencies()
    {
        var stashCurrencies = poeHudWrapper.StashCurrencies;
        Assert.IsTrue(stashCurrencies.Count > 0);
    }

    [TestMethod]
    public void PendingInvites()
    {
        var pendingInvites = poeHudWrapper.PendingInvites;
        Assert.IsTrue(pendingInvites.Any());
    }

    [TestMethod]
    public void PartyInvites()
    {
        var invites = poeHudWrapper.PartyInvites;
        var characterName = invites.First().Name;
        var accountName = invites.First().AccountName;
        Assert.IsTrue(invites.Length > 0);
    }

    [TestMethod]
    public void GetPartyInviteAcceptLocation()
    {
        var partyInviteLocation = poeHudWrapper.GetPartyInviteAcceptLocation("FishTester");
        Assert.IsTrue(partyInviteLocation.X > 0);
        Assert.IsTrue(partyInviteLocation.Y > 0);
    }

    [TestMethod]
    public void AcceptPartyButtonLocation()
    {
        var partyInvites = poeHudWrapper.PartyInvites;
        var acceptButtonLocation = partyInvites.FirstOrDefault()?.AcceptButton.Center;
        Assert.AreNotEqual(new Point(-1, -1), acceptButtonLocation);
    }

    [TestMethod]
    public void FindAcceptPartyButton()
    {
        var findAcceptPartyButtonLocation = poeHudWrapper.GetPartyInviteAcceptLocation("FishTester");
        Assert.IsTrue(findAcceptPartyButtonLocation.X >= 0);
        Assert.IsTrue(findAcceptPartyButtonLocation.Y >= 0);
    }

    [TestMethod]
    public void PlayerInParty()
    {
        var partyMembers = poeHudWrapper.PartyMemberNames;
        Assert.IsTrue(poeHudWrapper.PlayerInParty());
        Assert.IsFalse(poeHudWrapper.PlayerInParty("NonPerson"));
        Assert.IsTrue(poeHudWrapper.PlayerInParty("FishTester"));
    }

    [TestMethod]
    public void GetPartyMembers()
    {
        var partyMembers = poeHudWrapper.PartyMembers;
        Assert.IsTrue(partyMembers.Count >= 1);
    }

    [TestMethod]
    public void PlayerInZone()
    {
        Assert.IsTrue(poeHudWrapper.PlayerInZone("FishTester"));
    }

    [TestMethod]
    public void AcceptTradeButtonLocation()
    {
        var tradeInvites = poeHudWrapper.TradeInvites;
        var acceptButtonLocation = tradeInvites.FirstOrDefault()?.AcceptButton.Center;
        Assert.AreNotEqual(new Point(-1, -1), acceptButtonLocation);
    }

    [TestMethod]
    public void AcceptTradeVisible()
    {
        var acceptClickable = poeHudWrapper.AcceptButtonClickable;
        Assert.IsTrue(acceptClickable);
    }

    [TestMethod]
    public void CancelAcceptTradeVisible()
    {
        var acceptClicked = poeHudWrapper.AcceptButtonClicked;
        Assert.IsTrue(acceptClicked);
    }

    [TestMethod]
    public void TradeWindowOpen()
    {
        Assert.IsTrue(poeHudWrapper.TradeWindowOpen);
    }

    [TestMethod]
    public void TradeWindowSellerName()
    {
        var sellerName = poeHudWrapper.TradeWindowSellerName;
        Assert.IsFalse(string.IsNullOrEmpty(sellerName));
    }

    [TestMethod]
    public void TradeItems()
    {
        var items = poeHudWrapper.TheirTradeItems.Select(inventoryItem => inventoryItem.Item);
        foreach (var item in items)
        {
            var name = poeHudWrapper.GetItemName(item);
            var baseType = poeHudWrapper.GetBaseType(item);
            var numSockets = item.GetNumberSockets();
            var numLinks = item.GetNumberLinks();
            var corrupted = item.IsCorrupted();
            var className = poeHudWrapper.GetClassName(item);
        }
    }

    [TestMethod]
    public void GetTheirTradeItems()
    {
        Assert.IsTrue(poeHudWrapper.TheirTradeItems.Count > 0);
        foreach (var item in poeHudWrapper.TheirTradeItems)
        {
            var location = item.Center;
            Assert.IsTrue(location.X > 0);
            Assert.IsTrue(location.Y > 0);
        }
    }

    [TestMethod]
    public void GetTheirTradeCurrency()
    {
        var tradeCurrency = poeHudWrapper.TheirTradeCurrency;
        Assert.IsTrue(tradeCurrency.Length >= 1);
    }
}

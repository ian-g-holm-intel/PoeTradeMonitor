using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PoeLib;
using TradeBotLib;
using PoeHudWrapper;
using System.Drawing;
using InputSimulatorStandard.Native;
using Microsoft.Extensions.DependencyInjection;
using PoeLib.Settings;

namespace TradeBot.Tests;

[TestFixture]
public class PoeHUDWrapperTests
{
    private IPoeHudWrapper poeHudWrapper;
    private ITradeCommands tradeCommands;
    private ServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        try
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<PoeSettings>();
            new PoeLib.Bootstrapper().RegisterServices(serviceCollection);
            new TradeBotLib.Bootstrapper().RegisterServices(serviceCollection);
            new PoeHudWrapper.Bootstrapper().RegisterServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService<IGameWrapper>().Initialize();
            poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
            tradeCommands = serviceProvider.GetRequiredService<ITradeCommands>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initialize test");
            Console.WriteLine(ex);
        }
    }

    [TearDown]

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

    [Test]
    public void DestroyDialogVisible()
    {
        Assert.That(poeHudWrapper.DestroyConfirmationVisible);
    }

    [Test]
    public void GetDestroyKeepButtonLocation()
    {
        var location = poeHudWrapper.DestroyConfirmationKeepButtonLocation;
        Assert.That(location.X > 0);
        Assert.That(location.Y > 0);
    }

    [Test]
    public async Task StashOpenAndClose()
    {
        await tradeCommands.CloseAllPanels();

        Assert.That(!poeHudWrapper.StashOpen);

        await tradeCommands.OpenStash();

        Assert.That(poeHudWrapper.StashOpen);

        await tradeCommands.CloseAllPanels();

        Assert.That(!poeHudWrapper.StashOpen);
    }

    [Test]
    public async Task SwitchTabs()
    {
        tradeCommands.ActivatePoe();
        await poeHudWrapper.SwitchToTab("Sale");
    }

    [Test]
    public void AreaName()
    {
        Assert.That("Immaculate Hideout" == poeHudWrapper.AreaName);
    }

    [Test]
    public void InGame()
    {
        Assert.That(poeHudWrapper.InGame);
    }

    [Test]
    public void IsAtLogin()
    {
        Assert.That(poeHudWrapper.IsAtLogin);
    }

    [Test]
    public void IsLoading()
    {
        Assert.That(poeHudWrapper.IsLoading);
    }

    [Test]
    public void CharacterInZone()
    {
        var characters = poeHudWrapper.CharactersInArea;
        Assert.That(characters.Length >= 1);

        foreach (var player in characters)
            Assert.That(poeHudWrapper.PlayerInZone(player));
    }

    [Test]
    public void GetCraftingSlotMods()
    {
        var mods = poeHudWrapper.CraftingSlotMods;
        Assert.That(mods.Length > 0);
    }

    [Test]
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

    [Test]
    public void PlayerInventoryCurrency()
    {
        var currency = poeHudWrapper.PlayerInventoryCurrency;
        Assert.That(currency.Count >= 1);
    }

    [Test]
    public void GetPlayerInventoryItems()
    {
        var items = poeHudWrapper.PlayerInventoryItems.ToList();
        Assert.That(items.Count() >= 1);
    }

    [Test]
    public void IsItemCurrency()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isItemCurrency = poeHudWrapper.IsItemCurrency(item.Item);
            Assert.That(isItemCurrency);
        }
    }

    [Test]
    public void IsItemUniqueRing()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isUniqueRing = poeHudWrapper.IsItemUniqueRing(item.Item);
            Assert.That(isUniqueRing);
        }
    }

    [Test]
    public void IsItemFragment()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isFragment = poeHudWrapper.IsItemFragment(item.Item);
            Assert.That(isFragment);
        }
    }

    [Test]
    public void IsItemMap()
    {
        var items = poeHudWrapper.PlayerInventoryItems;
        foreach (var item in items)
        {
            var isMap = poeHudWrapper.IsItemMap(item.Item);
            Assert.That(isMap);
        }
    }

    [Test]
    public async Task SocialPanel()
    {
        await tradeCommands.CloseAllPanels();

        Assert.That(!poeHudWrapper.SocialPanelOpen);

        await tradeCommands.OpenPartyScreen();

        Assert.That(poeHudWrapper.PartyTabVisible);

        Assert.That(poeHudWrapper.SocialPanelOpen);

        await tradeCommands.CloseAllPanels();
    }

    [Test]
    public async Task GetCurrencyLocation()
    {
        await tradeCommands.CloseAllPanels();

        await tradeCommands.OpenStash();
        await poeHudWrapper.SwitchToTab("$");

        var total = poeHudWrapper.GetStashCurrencyOfType(CurrencyType.chaos).Sum(s => s.Amount);
        Assert.That(total >= 1);

        await tradeCommands.CloseAllPanels();
    }

    [Test]
    public async Task Chat()
    {
        if (poeHudWrapper.ChatVisible)
        {
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.That(!poeHudWrapper.ChatVisible);
        }
        else
        {
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.That(poeHudWrapper.ChatVisible);
            await tradeCommands.SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(50);
            Assert.That(!poeHudWrapper.ChatVisible);
        }
    }

    [Test]
    public void ChatMessage()
    {
        var chatMessage = poeHudWrapper.ChatText;
        Assert.That(chatMessage == "test");
    }

    [Test]
    public async Task GetFreeItemSlot()
    {
        var freeItemSlot = poeHudWrapper.FreeItemSlot;
        Assert.That(freeItemSlot.X >= 0);
        Assert.That(freeItemSlot.Y >= 0);

        await tradeCommands.CloseAllPanels();
    }

    [Test]
    public void GetCraftingSlotLocation()
    {
        var craftingSlotLocation = poeHudWrapper.CraftingSlotLocation;
        Assert.That(craftingSlotLocation.X >= 0);
        Assert.That(craftingSlotLocation.Y >= 0);
    }

    [Test]
    public void CraftingSlotItem()
    {
        var craftingSlotItem = poeHudWrapper.CraftingSlotItem;
        Assert.That(craftingSlotItem.IsValid);
    }

    [Test]
    public void ItemOnCursor()
    {
        var itemOnCursor = poeHudWrapper.ItemOnCursor;
        Assert.That(itemOnCursor);
    }

    [Test]
    public void PartyTabLocation()
    {
        var partyTabLocation = poeHudWrapper.PartyTabLocation;
        Assert.That(partyTabLocation.X > 0);
        Assert.That(partyTabLocation.Y > 0);
    }

    [Test]
    public void IsInventorySelected()
    {
        var isSelected = poeHudWrapper.IsInventorySelected(new Point(0, 0));
        Assert.That(isSelected);
    }

    [Test]
    public void StashCurrencies()
    {
        var stashCurrencies = poeHudWrapper.StashCurrencies;
        Assert.That(stashCurrencies.Count > 0);
    }

    [Test]
    public void PendingInvites()
    {
        var pendingInvites = poeHudWrapper.PendingInvites;
        Assert.That(pendingInvites.Any());
    }

    [Test]
    public void PartyInvites()
    {
        var invites = poeHudWrapper.PartyInvites;
        var characterName = invites.First().CharacterName;
        var accountName = invites.First().AccountName;
        Assert.That(invites.Length > 0);
    }

    [Test]
    public void GetPartyInviteAcceptLocation()
    {
        var partyInviteLocation = poeHudWrapper.GetPartyInviteAcceptLocation("FishTester");
        Assert.That(partyInviteLocation.X > 0);
        Assert.That(partyInviteLocation.Y > 0);
    }

    [Test]
    public void AcceptPartyButtonLocation()
    {
        var partyInvites = poeHudWrapper.PartyInvites;
        var acceptButtonLocation = partyInvites.FirstOrDefault()?.AcceptButtonLocation;
        Assert.That(acceptButtonLocation != new Point(-1, -1));
    }

    [Test]
    public void FindAcceptPartyButton()
    {
        var findAcceptPartyButtonLocation = poeHudWrapper.GetPartyInviteAcceptLocation("FishTester");
        Assert.That(findAcceptPartyButtonLocation.X >= 0);
        Assert.That(findAcceptPartyButtonLocation.Y >= 0);
    }

    [Test]
    public void PlayerInParty()
    {
        var partyMembers = poeHudWrapper.PartyMemberNames;
        Assert.That(poeHudWrapper.PlayerInParty());
        Assert.That(!poeHudWrapper.PlayerInParty("NonPerson"));
        Assert.That(poeHudWrapper.PlayerInParty("FishTester"));
    }

    [Test]
    public void GetPartyMembers()
    {
        var partyMembers = poeHudWrapper.PartyMembers;
        Assert.That(partyMembers.Length >= 1);
    }

    [Test]
    public void PlayerInZone()
    {
        Assert.That(poeHudWrapper.PlayerInZone("FishTester"));
    }

    [Test]
    public void AcceptTradeButtonLocation()
    {
        var tradeInvites = poeHudWrapper.TradeInvites;
        var acceptButtonLocation = tradeInvites.FirstOrDefault()?.AcceptButtonLocation;
        Assert.That(acceptButtonLocation != new Point(-1, -1));
    }

    [Test]
    public void AcceptTradeVisible()
    {
        var acceptClickable = poeHudWrapper.AcceptButtonClickable;
        Assert.That(acceptClickable);
    }

    [Test]
    public void CancelAcceptTradeVisible()
    {
        var acceptClicked = poeHudWrapper.AcceptButtonClicked;
        Assert.That(acceptClicked);
    }

    [Test]
    public void TradeWindowOpen()
    {
        Assert.That(poeHudWrapper.TradeWindowOpen);
    }

    [Test]
    public void TradeWindowSellerName()
    {
        var sellerName = poeHudWrapper.TradeWindowSellerName;
        Assert.That(!string.IsNullOrEmpty(sellerName));
    }

    [Test]
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

    [Test]
    public void GetTheirTradeItems()
    {
        Assert.That(poeHudWrapper.TheirTradeItems.Count > 0);
        foreach (var item in poeHudWrapper.TheirTradeItems)
        {
            var location = item.Center;
            Assert.That(location.X > 0);
            Assert.That(location.Y > 0);
        }
    }

    [Test]
    public void GetTheirTradeCurrency()
    {
        var tradeCurrency = poeHudWrapper.TheirTradeCurrency;
        Assert.That(tradeCurrency.Length >= 1);
    }

    [Test]
    public void LeavePartyButtonLocation()
    {
        var leavePartyButtonLocation = poeHudWrapper.LeavePartyButtonLocation;
        Assert.That(leavePartyButtonLocation.X >= 0);
        Assert.That(leavePartyButtonLocation.Y >= 0);
    }
}

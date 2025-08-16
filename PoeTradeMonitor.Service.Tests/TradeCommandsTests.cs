using PoeHudWrapper;
using Microsoft.Extensions.DependencyInjection;
using PoeLib.Tools;
using PoeTrade.Contracts;
using PoeLib;
using Microsoft.Extensions.Configuration;

namespace PoeTradeMonitor.Service.Tests;

[TestClass]
public class TradeCommandsTests
{
    private IPoeHudWrapper poeHudWrapper;
    private ITradeCommands tradeCommands;
    private IPoeChatWatcher chatWatcher;
    private ServiceProvider serviceProvider;

    public TradeCommandsTests()
    {
        var configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddHttpClient();
        serviceCollection.AddPoeLib();
        serviceCollection.AddPoeHudWrapper();
        serviceCollection.AddPoeTradeMonitorService();
        serviceProvider = serviceCollection.BuildServiceProvider();

        poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
        tradeCommands = serviceProvider.GetRequiredService<ITradeCommands>();
        chatWatcher = serviceProvider.GetRequiredService<IPoeChatWatcher>();
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
    public async Task RemoveCurrency()
    {
        await tradeCommands.RemoveInventoryCurrency();
    }

    [TestMethod]
    public async Task OpenStash()
    {
        await tradeCommands.OpenStash();
    }

    [TestMethod]
    public async Task AcceptParty()
    {
        await tradeCommands.AcceptParty("FishTester");
    }

    [TestMethod]
    public async Task WaitPlayerJoinArea()
    {
        var joined = await tradeCommands.WaitPlayerJoinArea("FishTester", 15000);
        Assert.IsTrue(joined);
    }

    [TestMethod]
    public async Task TradeWith()
    {
        await tradeCommands.TradeWith("FishTester");
    }

    [TestMethod]
    public async Task SendCommand()
    {
        await tradeCommands.SendTextCommand("/played");
    }

    [TestMethod]
    public async Task SelectTab()
    {
        await tradeCommands.SelectTab("$");
        await tradeCommands.SelectTab("Sale");
        await tradeCommands.SelectTab("$");
    }

    [TestMethod]
    public async Task OpenCloseChat()
    {
        await tradeCommands.OpenChat();
        await tradeCommands.CloseChat();
    }

    [TestMethod]
    public async Task SellAllItems()
    {
        await tradeCommands.SellAllItems();
    }

    [TestMethod]
    public async Task AntiAFK()
    {
        await tradeCommands.AntiAFK();
    }

    [TestMethod]
    public async Task TryClearClipboard()
    {
        await tradeCommands.TryClearClipboard();
        Assert.IsTrue(string.IsNullOrEmpty(await tradeCommands.GetClipboard()));
    }

    [TestMethod]
    public async Task GetItemInfo()
    {
        var itemInfo = await tradeCommands.GetItemInfo(new Point(378, 420));
        Assert.IsFalse(string.IsNullOrEmpty(itemInfo));
    }

    [TestMethod]
    public async Task GotoHideout()
    {
        await chatWatcher.StartAsync(CancellationToken.None);
        using var ctSource = new CancellationTokenSource(10000);
        Assert.IsTrue(await tradeCommands.GotoHideout("FishyTester", ctSource.Token));
    }

    [TestMethod]
    public async Task SendTextCommand()
    {
        Assert.IsFalse(poeHudWrapper.ChatVisible);
        await tradeCommands.SendTextCommand("/played");
    }

    [TestMethod]
    public async Task LeaveParty()
    {
        await tradeCommands.LeaveParty();
    }

    [TestMethod]
    public async Task WithdrawCurrency()
    {
        for (int i = 0; i < 1; i++)
        {
            await tradeCommands.RemoveInventoryCurrency();
            await tradeCommands.WithdrawCurrency(new[] { new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = 9 }, new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 124 } });
            var currency = poeHudWrapper.PlayerInventoryCurrency;
        }
        await tradeCommands.RemoveInventoryCurrency();
    }

    [TestMethod]
    public async Task RemoveInventoryCurrency_Withdraw()
    {
        var currency = new[] { new CurrencyInfo { Type = TradeCurrencyType.Chaos, Amount = 65 }, new CurrencyInfo { Type = TradeCurrencyType.Divine, Amount = 24 } };
        await tradeCommands.RemoveInventoryItems();
        for (int i = 0; i < 1; i++)
        {
            await tradeCommands.WithdrawCurrency(currency);
            await tradeCommands.RemoveInventoryCurrency(currency, false, false);
        }
    }

    [TestMethod]
    public async Task RemoveInventoryCurrency_Stash()
    {
        await tradeCommands.RemoveInventoryCurrency(new CurrencyInfo[] { new CurrencyInfo() { Type = TradeCurrencyType.Transmute, Amount = 120 } });
    }

    [TestMethod]
    public async Task RemoveInventoryCurrency()
    {
        await tradeCommands.RemoveInventoryCurrency();
    }

    [TestMethod]
    public async Task RemoveInventoryItems()
    {
        await tradeCommands.RemoveInventoryItems();
    }

    [TestMethod]
    public async Task MouseOverItems()
    {
        await tradeCommands.MouseOverItems();
    }

    [TestMethod]
    public async Task WaitForPartyRequest()
    {
        var result = await tradeCommands.WaitPartyRequest("Stormtaco");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task PartyLoop()
    {
        var partyInvites = poeHudWrapper.PartyInvites;
        await tradeCommands.LeftClickMouse(partyInvites.First().AcceptButton.Center);
        await Task.Delay(200);
        var partyMembers = poeHudWrapper.PartyMembers;
    }
}

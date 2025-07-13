using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;
using Bootstrapper = TradeBotLib.Bootstrapper;
using Microsoft.Extensions.DependencyInjection;
using PoeLib.Settings;
using System.Threading;
using PoeLib.Tools;

namespace TradeBot.Tests;

[TestFixture]
public class TradeCommandsTests
{
    private IPoeHudWrapper poeHudWrapper;
    private ITradeCommands tradeCommands;
    private IPoeChatWatcher chatWatcher;
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
            new PoeHudWrapper.Bootstrapper().RegisterServices(serviceCollection);
            new Bootstrapper().RegisterServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
            
            serviceProvider.GetRequiredService<IGameWrapper>().Initialize();
            poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
            tradeCommands = serviceProvider.GetRequiredService<ITradeCommands>();
            chatWatcher = serviceProvider.GetRequiredService<IPoeChatWatcher>();
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to initialize test");
            throw;
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
    public async Task RemoveCurrency()
    {
        await tradeCommands.RemoveInventoryCurrency();
    }

    [Test]
    public async Task OpenStash()
    {
        await tradeCommands.OpenStash();
    }

    [Test]
    public async Task AcceptParty()
    {
        await tradeCommands.AcceptParty("FishTester");
    }

    [Test]
    public async Task WaitPlayerJoinArea()
    {
        var joined = await tradeCommands.WaitPlayerJoinArea("FishTester", 15000);
        Assert.That(joined);
    }

    [Test]
    public async Task TradeWith()
    {
        await tradeCommands.TradeWith("FishTester");
    }

    [Test]
    public async Task SendCommand()
    {
        await tradeCommands.SendTextCommand("/played");
    }

    [Test]
    public async Task SelectTab()
    {
        await tradeCommands.SelectTab("$");
        await tradeCommands.SelectTab("Sale");
        await tradeCommands.SelectTab("$");
    }

    [Test]
    public async Task OpenCloseChat()
    {
        await tradeCommands.OpenChat();
        await tradeCommands.CloseChat();
    }

    [Test]
    public async Task SellAllItems()
    {
        await tradeCommands.SellAllItems();
    }

    [Test]
    public async Task AntiAFK()
    {
        await tradeCommands.AntiAFK();
    }

    [Test]
    public async Task TryClearClipboard()
    {
        await tradeCommands.TryClearClipboard();
        Assert.That(string.IsNullOrEmpty(await tradeCommands.GetClipboard()));
    }

    [Test]
    public async Task GetItemInfo()
    {
        var itemInfo = await tradeCommands.GetItemInfo(new Point(378, 420));
        Assert.That(!string.IsNullOrEmpty(itemInfo));
    }

    [Test]
    public async Task GotoHideout()
    {
        await chatWatcher.StartAsync(CancellationToken.None);
        using var ctSource = new CancellationTokenSource(10000);
        Assert.That(await tradeCommands.GotoHideout("FishyTester", ctSource.Token));
    }

    [Test]
    public async Task SendTextCommand()
    {
        Assert.That(!poeHudWrapper.ChatVisible);
        await tradeCommands.SendTextCommand("/played");
    }

    [Test]
    public async Task LeaveParty()
    {
        await tradeCommands.LeaveParty();
    }

    [Test]
    public async Task WithdrawCurrency()
    {
        for (int i = 0; i < 10; i++)
        {
            await tradeCommands.RemoveInventoryCurrency();
            await tradeCommands.WithdrawCurrency(new[] { new Currency { Type = CurrencyType.divine, Amount = 9 }, new Currency { Type = CurrencyType.chaos, Amount = 124 } });
            var currency = poeHudWrapper.PlayerInventoryCurrency;
        }
        await tradeCommands.RemoveInventoryCurrency();
    }

    [Test]
    public async Task RemoveInventoryCurrency_Withdraw()
    {
        var currency = new[] { new Currency { Type = CurrencyType.chaos, Amount = 65 }, new Currency { Type = CurrencyType.divine, Amount = 24 } };
        await tradeCommands.RemoveInventoryItems();
        for (int i = 0; i < 1; i++)
        {
            await tradeCommands.WithdrawCurrency(currency);
            await tradeCommands.RemoveInventoryCurrency(currency, false, false);
        }
    }

    [Test]
    public async Task RemoveInventoryCurrency_Stash()
    {
        await tradeCommands.RemoveInventoryCurrency(new Currency[] { new Currency() { Type = CurrencyType.transmute, Amount = 120 } });
    }

    [Test]
    public async Task RemoveInventoryCurrency()
    {
        await tradeCommands.RemoveInventoryCurrency();
    }

    [Test]
    public async Task RemoveInventoryItems()
    {
        await tradeCommands.RemoveInventoryItems();
    }

    [Test]
    public async Task MouseOverItems()
    {
        await tradeCommands.MouseOverItems();
    }

    [Test]
    public async Task WaitForPartyRequest()
    {
        var result = await tradeCommands.WaitPartyRequest("Stormtaco");
        Assert.That(result);
    }

    [Test]
    public async Task PartyLoop()
    {
        var partyInvites = poeHudWrapper.PartyInvites;
        await tradeCommands.LeftClickMouse(partyInvites.First().AcceptButtonLocation);
        await Task.Delay(200);
        var partyMembers = poeHudWrapper.PartyMembers;
    }
}

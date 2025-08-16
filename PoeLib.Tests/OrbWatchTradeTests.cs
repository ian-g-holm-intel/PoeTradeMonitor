using Microsoft.Extensions.DependencyInjection;
using PoeLib.PriceFetchers.OrbWatchTrade;
using PoeTradeMonitor.GUI.Settings;

namespace PoeLib.Tests;

[TestClass]
public class OrbWatchTradeTests
{
    private OrbWatchTradeWrapper target;
    private string league = new PoeSettings().League;

    public OrbWatchTradeTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddLogging();
        serviceCollection.AddTransient<OrbWatchTradeWrapper>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        target = serviceProvider.GetRequiredService<OrbWatchTradeWrapper>();
    }

    [TestMethod]
    public async Task GetCurrencyData()
    {
        var data = await target.GetCurrencyData(league);
        Assert.IsNotNull(data);
    }
}

using Microsoft.Extensions.DependencyInjection;
using PoeLib.PriceFetchers.Poe2Scout;
using PoeTradeMonitor.GUI.Settings;

namespace PoeLib.Tests;

[TestClass]
public class Poe2ScoutWrapperTests
{
    private Poe2ScoutWrapper target;
    private string league = new PoeSettings().League;

    public Poe2ScoutWrapperTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddLogging();
        serviceCollection.AddTransient<Poe2ScoutWrapper>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        target = serviceProvider.GetRequiredService<Poe2ScoutWrapper>();
    }

    [TestMethod]
    public async Task GetCurrencyData()
    {
        var data = await target.GetCurrencyData(league);
        Assert.IsNotNull(data);
    }
}

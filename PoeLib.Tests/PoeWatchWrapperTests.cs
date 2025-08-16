using Microsoft.Extensions.DependencyInjection;
using PoeLib.PriceFetchers.PoeWatch;

namespace PoeLib.Tests;

[TestClass]
public class PoeWatchWrapperTests
{
    private PoeWatchWrapper poeWatch = null!;
    private const string league = "Mercenaries";

    [TestInitialize]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddLogging();
        serviceCollection.AddTransient<PoeWatchWrapper>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        poeWatch = serviceProvider.GetService<PoeWatchWrapper>()!;
    }

    [TestMethod]
    public async Task GetCategories()
    {
        var data = await poeWatch.GetCategories();
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.Length > 0);
    }

    [TestMethod]
    public async Task GetCurrencyData()
    {
        var data = await poeWatch.GetCurrencyData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.Count > 0);
    }

    [TestMethod]
    public async Task GetFragmentData()
    {
        var data = await poeWatch.GetFragmentData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetDivinationCardData()
    {
        var data = await poeWatch.GetDivinationCardData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueMapData()
    {
        var data = await poeWatch.GetUniqueMapData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueJewelData()
    {
        var data = await poeWatch.GetUniqueJewelData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueFlaskData()
    {
        var data = await poeWatch.GetUniqueFlaskData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueWeaponData()
    {
        var data = await poeWatch.GetUniqueWeaponData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueArmorData()
    {
        var data = await poeWatch.GetUniqueArmorData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetUniqueAccessoryData()
    {
        var data = await poeWatch.GetUniqueAccessoryData(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }

    [TestMethod]
    public async Task GetFossils()
    {
        var data = await poeWatch.GetFossils(league);
        Assert.IsNotNull(data);
        Assert.IsTrue(data?.SearchItems?.Count > 0);
    }
}

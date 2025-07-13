using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PoeLib.PriceFetchers.PoeWatch;
using PoeLib.Settings;

namespace PoeLib.Tests;

[TestFixture]
public class PoeWatchWrapperTests
{
    private PoeWatchWrapper poeWatch;
    private string league = new PoeSettings().League;

    [SetUp]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddLogging();
        serviceCollection.AddTransient<PoeWatchWrapper>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        poeWatch = serviceProvider.GetService<PoeWatchWrapper>();
    }

    [Test]
    public async Task GetCategories()
    {
        var data = await poeWatch.GetCategories();
        Assert.That(data != null);
        Assert.That(data.Length > 0);
    }

    [Test]
    public async Task GetCurrencyData()
    {
        var data = await poeWatch.GetCurrencyData(league);
        Assert.That(data != null);
        Assert.That(data.Count > 0);
    }

    [Test]
    public async Task GetFragmentData()
    {
        var data = await poeWatch.GetFragmentData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetDivinationCardData()
    {
        var data = await poeWatch.GetDivinationCardData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueMapData()
    {
        var data = await poeWatch.GetUniqueMapData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueJewelData()
    {
        var data = await poeWatch.GetUniqueJewelData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueFlaskData()
    {
        var data = await poeWatch.GetUniqueFlaskData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueWeaponData()
    {
        var data = await poeWatch.GetUniqueWeaponData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueArmorData()
    {
        var data = await poeWatch.GetUniqueArmorData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetUniqueAccessoryData()
    {
        var data = await poeWatch.GetUniqueAccessoryData(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }

    [Test]
    public async Task GetFossils()
    {
        var data = await poeWatch.GetFossils(league);
        Assert.That(data != null);
         Assert.That(data.SearchItems.Count > 0);
    }
}

using NUnit.Framework;
using PoeHudWrapper;
using PoeLib;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using PoeLib.Settings;
using System.Text.Json;

namespace TradeBotLib.Tests;

[TestFixture]
public class PriceValidatorTests
{
    private readonly IPoeHudWrapper poeHudWrapper;
    private readonly IPriceValidator target;
    public PriceValidatorTests()
    {

        try
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<PoeSettings>();
            new PoeLib.Bootstrapper().RegisterServices(serviceCollection);
            new TradeBotLib.Bootstrapper().RegisterServices(serviceCollection);
            new PoeHudWrapper.Bootstrapper().RegisterServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService<IGameWrapper>().Initialize();
            poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
            target = serviceProvider.GetRequiredService<IPriceValidator>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initialize test");
            Console.WriteLine(ex);
        }
    }

    [Test]
    public void IsCorrectItem()
    {
        var expectedItem = JsonSerializer.Deserialize<PoeLib.JSON.Item>("{\"verified\":true,\"w\":1,\"h\":1,\"icon\":\"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvRGl2aW5hdGlvbi9JbnZlbnRvcnlJY29uIiwidyI6MSwiaCI6MSwic2NhbGUiOjF9XQ/f34bf8cbb5/InventoryIcon.png\",\"stackSize\":1,\"maxStackSize\":9,\"league\":\"Mercenaries\",\"id\":\"7c26137da812a292a66ecb61ca0b4ca4818696d3ac044dd5b39d05586eeaa4f5\",\"name\":\"\",\"typeLine\":\"House of Mirrors\",\"baseType\":\"House of Mirrors\",\"ilvl\":0,\"identified\":true,\"properties\":[{\"name\":\"Stack Size\",\"values\":[[\"1/9\",0]],\"displayMode\":0,\"type\":32}],\"explicitMods\":[\"<currencyitem>{Mirror of Kalandra}\"],\"flavourText\":[\"What do you see in the mirror?\"],\"frameType\":6,\"artFilename\":\"HouseOfMirrors\",\"extended\":{\"text\":\"SXRlbSBDbGFzczogRGl2aW5hdGlvbiBDYXJkcw0KUmFyaXR5OiBEaXZpbmF0aW9uIENhcmQNCkhvdXNlIG9mIE1pcnJvcnMNCi0tLS0tLS0tDQpTdGFjayBTaXplOiAxLzkNCi0tLS0tLS0tDQpNaXJyb3Igb2YgS2FsYW5kcmENCi0tLS0tLS0tDQpXaGF0IGRvIHlvdSBzZWUgaW4gdGhlIG1pcnJvcj8NCg==\"}}");
        var item = poeHudWrapper.PlayerInventoryItems.First();
        target.IsCorrectItem(new ItemTradeRequest { CharacterName = "SecondBestTuffl", Item = expectedItem }, "House of Mirrors", item.Item, out var stackSize);
    }
}

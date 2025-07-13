using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class FocusedAmuletCrafter : CrafterBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RingCrafter));
    private readonly INotificationClient notificationClient;

    public FocusedAmuletCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm, INotificationClient notificationClient) : base(phw, tc, rsm)
    {
        this.notificationClient = notificationClient;
    }

    public override async Task Craft()
    {
        await Setup();

        var amCount = 0;
        try
        {
            while (HasCurrency(CurrencyType.regal))
            {
                if (await CheckModsAsync())
                {
                    break;
                }

                await MakeNormal();

                await Task.Delay(25);

                await MakeRare();

                await Task.Delay(25);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine($"Saw AM {amCount} times");
            Console.ReadLine();
        }
    }

    private async Task<bool> CheckModsAsync()
    {
        var mods = GetCraftingMods().ToArray();

        if (HasCritMulti)
        {
            Console.WriteLine("SUCCESS! Make yourself a sandwich");
            await notificationClient.SendPushNotification("Craft Complete", "Found Mods", "Found Mods");
            return true;
        }

        return false;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 0;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 0;
    }

    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedLife" && mod.Tier == 1) != null;

    private bool HasMana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedMana" && mod.Tier == 1) != null;

    private bool HasCritMulti => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "CriticalStrikeMultiplier" && mod.Tier == 1) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class AssassinsMarkRingCrafter : CrafterBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RingCrafter));

    public AssassinsMarkRingCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    public override async Task Craft()
    {
        await Setup();

        // Check for chaos spam
        if (!HasCurrency(CurrencyType.chaos))
            return;

        // Check for path to rare
        if (!HasPathToRare)
            return;

        var amCount = 0;
        try
        {
            await MakeMagic();
            await StartUsingCurrency(CurrencyType.alt);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(CurrencyType.alt))
                    await ClickItem();

                if (HasCurrency(CurrencyType.aug) && (HasAssassinsMark && GetNumberOfPrefixes() == 0) || ((HasLife || HasPhys) && GetNumberOfSuffixes() == 0))
                    await UseCurrency(CurrencyType.aug);

                if (HasAssassinsMark)
                    amCount++;

                if (HasAssassinsMark)
                {
                    Console.WriteLine("Acceptable mods found");
                    Console.WriteLine("Press <Enter> to continue");
                    Console.ReadLine();
                }
            }
        }
        catch (NotEnoughCurrencyToRareException)
        {
            log.Info("Ran out of currency, exiting");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await StopUsingCurrency();
            Console.WriteLine($"Saw AM {amCount} times");
            Console.ReadLine();
        }
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "IncreasedLife");
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "CriticalStrikeMultiplier" || mod.Record.Group == "FireResistance" || mod.Record.Group == "ColdResistance" || mod.Record.Group == "LightningResistance" || mod.Record.Key.Contains("CurseOnHitWarlordsMark"));
    }
    
    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedLife" && mod.Tier <= 3 && mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix) != null;
    private bool HasPhys => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "PhysicalDamage" && mod.Tier <= 2 && mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix) != null;
    private bool HasAssassinsMark => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("CurseOnHitAssassinsMark")) != null;
}

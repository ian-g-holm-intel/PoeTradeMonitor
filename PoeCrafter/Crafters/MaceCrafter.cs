using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class MaceCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public MaceCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
        tradeCommands = tc;
    }

    public override async Task Craft()
    {
        try
        {
            while (true)
            {
                if (!HasCurrency(CurrencyType.alt) || !HasCurrency(CurrencyType.aug))
                {
                    Console.WriteLine("Out of currency, exiting");
                    break;
                }

                if (await CheckMods())
                {
                    break;
                }

                if (GetNumberOfPrefixes() == 0)
                    await UseCurrency(CurrencyType.aug);

                await Task.Delay(25);

                if (await CheckMods())
                {
                    break;
                }

                await UseCurrency(CurrencyType.alt);

                await Task.Delay(25);
            }
        }
        catch (CurrencyNotFoundException)
        {
            Console.WriteLine("Out of currency, exiting");
        }
        catch (Exception ex)
        {

        }
        finally
        {
            await StopUsingCurrency();
        }
    }

    private async Task<bool> CheckMods()
    {
        var mods = GetCraftingMods().ToArray();

        if (HasIPD)
        {
            if (GetNumberOfSuffixes() == 0)
            {
                Console.WriteLine("SUCCESS! Make yourself a sandwich");
                return true;
            }
            else
            {
                await UseCurrency(CurrencyType.annul);

                return await CheckMods();
            }
        }

        return false;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix);
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }

    private bool HasIPD => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalIncreasedPhysicalDamagePercent") && mod.Tier <= 2) != null;

    private bool HasCrit => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalCriticalStrikeChance6")) != null;

    private bool HasFlatPhys => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalAddedPhysicalDamageTwoHand9")) != null;
}

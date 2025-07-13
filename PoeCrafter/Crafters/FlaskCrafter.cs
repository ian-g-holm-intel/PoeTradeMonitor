using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class FlaskCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public FlaskCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

                if (CheckMods())
                {
                    break;
                }

                if ((HasIncEffect || HasCrit || HasStun || HasCurse) && (GetNumberOfPrefixes() == 0 || GetNumberOfSuffixes() == 0))
                    await UseCurrency(CurrencyType.aug);

                await Task.Delay(25);

                if (CheckMods())
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
        finally
        {
            await StopUsingCurrency();
        }
    }

    private bool CheckMods()
    {
        var mods = GetCraftingMods().ToArray();

        if ((HasCrit || HasStun || HasCurse || HasResists))
        {
            Console.WriteLine("SUCCESS! Make yourself a sandwich");
            return true;
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
    
    private bool HasCrit => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("FlaskBuffCriticalChanceWhileHealing5")) != null;

    private bool HasStun => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("FlaskBuffAvoidStunWhileHealing5")) != null;

    private bool HasResists => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("FlaskBuffResistancesWhileHealing5")) != null;

    private bool HasIncEffect => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("FlaskEffectReducedDuration")) != null;

    private bool HasCurse => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("FlaskBuffCurseEffect5")) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;

namespace PoeCrafter.Crafters;

public class GlovesCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public GlovesCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
        tradeCommands = tc;
    }

    public override async Task Craft()
    {
        try
        {
            while (true)
            {
                if (!HasCurrency(TradeCurrencyType.Alt) || !HasCurrency(TradeCurrencyType.Aug))
                {
                    Console.WriteLine("Out of currency, exiting");
                    break;
                }

                if (CheckMods())
                {
                    break;
                }

                if (GetNumberOfPrefixes() == 0)
                    await UseCurrency(TradeCurrencyType.Aug);

                await Task.Delay(25);

                if (CheckMods())
                {
                    break;
                }

                await UseCurrency(TradeCurrencyType.Alt);

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

        if (!HasChaosRes && !HasCullingStrike)
            return false;

        Console.WriteLine("SUCCESS! Make yourself a sandwich");
        return true;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix);
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }

    private bool HasChaosRes => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("ChaosResistance") && mod.Tier == 1) != null;

    private bool HasCullingStrike => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("cull", StringComparison.OrdinalIgnoreCase)) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class ArmorCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public ArmorCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
        tradeCommands = tc;
    }

    public override async Task Craft()
    {
        try
        {
            var random = new Random();
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

                if (GetNumberOfPrefixes() == 0 || GetNumberOfSuffixes() == 0)
                    await UseCurrency(CurrencyType.aug);

                await Task.Delay(random.Next(25,50));

                if (CheckMods())
                {
                    break;
                }

                await UseCurrency(CurrencyType.alt);

                await Task.Delay(random.Next(25, 50));
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

        if (HasSuppression || HasChaosRes || HasDefensesFlat)
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

    private bool HasDefensesFlat => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("BaseLocalDefences") && mod.Tier == 1) != null;

    private bool HasDefensesHybrid => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("DefencesPercentAndStunRecovery") && mod.Tier == 1) != null;

    private bool HasDefensesPercent => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("DefencesPercent") && mod.Tier == 1) != null;

    private bool HasSuppression => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("ChanceToSuppressSpellsHigh5")) != null;

    private bool HasChaosRes => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Contains("ChaosResistance") && mod.Tier == 1) != null;
}

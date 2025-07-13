using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class BootCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public BootCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

                if (GetNumberOfPrefixes() == 0 || GetNumberOfSuffixes() == 0)
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

        if (!HasCooldown && !HasLife)
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

    private bool HasCooldown => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Equals("CooldownRecoveryInfluence2")) != null;

    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("IncreasedLife") && mod.Tier == 1) != null;

    private bool HasMana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("IncreasedMana") && mod.Tier == 1) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class WandCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public WandCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

                if (GetNumberOfPrefixes() < 1)
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

        if (!HasSpellDamage && !HasMana && !HasHybrid) return false;

        if (!HasT1SpellDamage && !HasT1Mana && !HasT1Hybrid) return false;
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

    private bool HasSpellDamage => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("SpellDamageOnWeapon")) != null;

    private bool HasMana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedMana") != null;

    private bool HasHybrid => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "SpellDamageAndMana") != null;

    private bool HasT1SpellDamage => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("SpellDamageOnWeapon") && mod.Tier == 1) != null;

    private bool HasT1Mana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedMana" && mod.Tier == 1) != null;

    private bool HasT1Hybrid => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "SpellDamageAndMana" && mod.Tier == 1) != null;
}

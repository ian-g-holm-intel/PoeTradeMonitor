using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class AstrolabeCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public AstrolabeCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

                if (await CheckMods())
                {
                    break;
                }

                if (GetNumberOfAffixes() == 1 && GetCraftingMods().Any(m => m.Record.InfluenceType != ExileCore.Shared.Enums.InfluenceTypes.None))
                {
                    await UseCurrency(CurrencyType.aug);
                    await Task.Delay(random.Next(25, 50));
                }

                if (await CheckMods())
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

    private async Task<bool> CheckMods()
    {
        var mods = GetCraftingMods().ToArray();
        var random = new Random();

        if (HasLife || HasCritMulti)
        {
            if (NonInfluencedMods == 2)
            {
                await UseCurrency(CurrencyType.annul);
                await Task.Delay(random.Next(25, 50));
                return await CheckMods();
            }
            else if (GetNumberOfAffixes() == 1)
            {
                await UseCurrency(CurrencyType.aug);
                await Task.Delay(random.Next(25, 50));
                return await CheckMods();
            }

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

    private int NonInfluencedMods => GetCraftingMods().Count(m => m.Record.InfluenceType == ExileCore.Shared.Enums.InfluenceTypes.None && (m.AffixType == ExileCore.Shared.Enums.ModType.Prefix || m.AffixType == ExileCore.Shared.Enums.ModType.Suffix));

    private int InfluencedMods => GetCraftingMods().Count(m => m.Record.InfluenceType != ExileCore.Shared.Enums.InfluenceTypes.None && (m.AffixType == ExileCore.Shared.Enums.ModType.Prefix || m.AffixType == ExileCore.Shared.Enums.ModType.Suffix));

    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedLife" && mod.Tier == 1) != null;

    private bool HasMana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedMana" && mod.Tier == 1) != null;

    private bool HasCritMulti => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "CriticalStrikeMultiplier" && mod.Tier == 1) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class SwordCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public SwordCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

        if (HasFire || HasCold || HasLightning)
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

    private bool HasLightning => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalAddedLightningDamageTwoHand10")) != null;

    private bool HasCold => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalAddedColdDamageTwoHand10")) != null;

    private bool HasFire => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key.Contains("LocalAddedFireDamageTwoHand10")) != null;

    private bool HasAtkSpeed => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group.Equals("IncreasedAttackSpeed") && mod.Tier == 1) != null;
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class DeathBowAltRegalCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public DeathBowAltRegalCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
        tradeCommands = tc;
    }

    public override async Task Craft()
    {
        try
        {
            await MakeMagic();
            await StartUsingCurrency(CurrencyType.alt);
            
            while (true)
            {
                if (!HasCurrency(CurrencyType.alt))
                {
                    Console.WriteLine("Out of currency, exiting");
                    break;
                }

                await ClickItem();

                if (await CheckMods())
                {
                    break;
                }
                
                if (GetNumberOfSuffixes() < 1)
                    await UseCurrency(CurrencyType.aug);

                if (await CheckMods())
                {
                    break;
                }

                await Task.Delay(100);
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
        if (HasAttackSpeed || HasCritMulti)
        {
            await StopUsingCurrency();

            if (GetNumberOfPrefixes() < 1)
                await UseCurrency(CurrencyType.aug);

            await MakeRare();
            if (HasAttackSpeed && HasCritMulti)
            {
                Console.WriteLine("SUCCESS! Make yourself a sandwich");
                return true;
            }
            
            await MakeMagic();

            await StartUsingCurrency(CurrencyType.alt);
        }
        return false;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }

    private bool HasAttackSpeed => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key == "LocalIncreasedAttackSpeedRangedDoubleDamageUber2" && mod.AffixType == ExileCore.Shared.Enums.ModType.Suffix) != null;
    private bool HasCritMulti => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key == "CriticalMultiplierSupportedTwoHandedUber2" && mod.AffixType == ExileCore.Shared.Enums.ModType.Suffix) != null;
}

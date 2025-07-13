using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class StaffCrafter : CrafterBase
{
    private readonly ITradeCommands tradeCommands;
    public StaffCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
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

                if (CheckMods())
                {
                    break;
                }
                
                if (GetNumberOfPrefixes() < 1)
                    await UseCurrency(CurrencyType.aug);

                if (CheckMods())
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

    private bool CheckMods()
    {
        if (!HasT1DotMulti) return false;
        Console.WriteLine("SUCCESS! Make yourself a sandwich");
        return true;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }

    private bool HasT1DotMulti => GetCraftingMods().SingleOrDefault(mod => mod.Record.Key == "ChaosNonAilmentDamageOverTimeMultiplier2h5") != null;
}

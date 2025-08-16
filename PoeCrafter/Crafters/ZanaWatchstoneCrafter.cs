using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;

namespace PoeCrafter.Crafters;

public class ZanaWatchstoneCrafter : CrafterBase
{
    public ZanaWatchstoneCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    public override async Task Craft()
    {
        await Setup();

        try
        {
            await MakeMagic();
            await StartUsingCurrency(TradeCurrencyType.Alt);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(TradeCurrencyType.Alt))
                    await ClickItem();
                else
                    throw new NotEnoughCurrencyException(TradeCurrencyType.Alt);

                if (HasCurrency(TradeCurrencyType.Aug))
                { 
                    if(GetNumberOfPrefixes() == 0)
                        await UseCurrency(TradeCurrencyType.Aug);
                }
                else
                    throw new NotEnoughCurrencyException(TradeCurrencyType.Aug);

                if (HasZana || HasExtraMaps)
                {
                    Console.WriteLine("Acceptable mods found");
                    return;
                }
            }
        }
        catch (NotEnoughCurrencyToRareException)
        {
            Console.WriteLine("Ran out of currency, exiting");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await StopUsingCurrency();
            Console.ReadLine();
        }
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 1 - GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix);
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 1 - GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Suffix);
    }
    
    private bool HasZana => GetCraftingMods().Any(mod => mod.Record.TypeName == "WatchstoneZanaChance" && mod.Tier == 1); 

    private bool HasExtraMaps => GetCraftingMods().Any(mod => mod.Record.TypeName == "WatchstoneZanaExtraOptions");
}

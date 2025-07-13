using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class ZanaWatchstoneCrafter : CrafterBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RingCrafter));

    public ZanaWatchstoneCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    public override async Task Craft()
    {
        await Setup();

        try
        {
            await MakeMagic();
            await StartUsingCurrency(CurrencyType.alt);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(CurrencyType.alt))
                    await ClickItem();
                else
                    throw new NotEnoughCurrencyException(CurrencyType.alt);

                if (HasCurrency(CurrencyType.aug))
                { 
                    if(GetNumberOfPrefixes() == 0)
                        await UseCurrency(CurrencyType.aug);
                }
                else
                    throw new NotEnoughCurrencyException(CurrencyType.aug);

                if (HasZana || HasExtraMaps)
                {
                    Console.WriteLine("Acceptable mods found");
                    return;
                }
            }
        }
        catch (NotEnoughCurrencyToRareException)
        {
            log.Info("Ran out of currency, exiting");
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

using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeCrafter.ModGroups;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public abstract class JewelCrafter : CrafterBase
{
    public static readonly ILog log = LogManager.GetLogger(typeof(JewelCrafter));
    public JewelCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    protected abstract ModGroupBase[] ModGroups { get; }

    public override async Task Craft()
    {
        await Setup();

        // Check for chaos spam
        if (!HasCurrency(CurrencyType.chaos))
            return;

        // Check for path to rare
        if (!HasPathToRare)
            return;

        var wmCount = 0;
        try
        {
            await MakeRare();
            await StartUsingCurrency(CurrencyType.chaos);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(CurrencyType.chaos))
                    await ClickItem();

                while (HasCurrency(CurrencyType.exalted) && GroupContainsMods() && HasRemainingMods())
                    await UseCurrency(CurrencyType.exalted);
                
                if (GroupContainsMods())
                    break;
            }

            await StopUsingCurrency();
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
            Console.WriteLine($"Saw WM {wmCount} times");
            Console.ReadLine();
        }
    }

    protected override bool HasRemainingMods()
    {
        return GetNumberOfPrefixes() < 2 && GetNumberOfRemainingPrefixes() > 0 || GetNumberOfSuffixes() < 2 && GetNumberOfRemainingSuffixes() > 0;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 2;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 2;
    }

    private bool GroupContainsMods()
    {
        var mods = GetCraftingMods();
        foreach (var group in ModGroups)
        {
            if (mods.All(mod => group.ContainsMod(mod.Record.UserFriendlyName)))
                return true;
        }

        return false;
    }
}

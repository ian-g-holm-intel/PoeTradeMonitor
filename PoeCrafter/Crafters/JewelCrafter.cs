using System;
using System.Linq;
using System.Threading.Tasks;
using PoeCrafter.ModGroups;
using PoeHudWrapper;

namespace PoeCrafter.Crafters;

public abstract class JewelCrafter : CrafterBase
{
    public JewelCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    protected abstract ModGroupBase[] ModGroups { get; }

    public override async Task Craft()
    {
        await Setup();

        // Check for chaos spam
        if (!HasCurrency(TradeCurrencyType.Chaos))
            return;

        // Check for path to rare
        if (!HasPathToRare)
            return;

        var wmCount = 0;
        try
        {
            await MakeRare();
            await StartUsingCurrency(TradeCurrencyType.Chaos);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(TradeCurrencyType.Chaos))
                    await ClickItem();

                while (HasCurrency(TradeCurrencyType.Exalted) && GroupContainsMods() && HasRemainingMods())
                    await UseCurrency(TradeCurrencyType.Exalted);
                
                if (GroupContainsMods())
                    break;
            }

            await StopUsingCurrency();
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

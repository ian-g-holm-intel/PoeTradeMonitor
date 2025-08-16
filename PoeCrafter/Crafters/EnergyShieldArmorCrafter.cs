using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;

namespace PoeCrafter.Crafters;

public abstract class EnergyShieldArmorCrafter : CrafterBase
{
    public EnergyShieldArmorCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm) { }

    protected abstract int BaseEnergyShield { get; }
    protected abstract int MinEnergyShield { get; }

    protected int CalculateEnergyShield()
    {
        var mods = GetCraftingMods();

        var flatEsMod = mods.SingleOrDefault(mod => mod.Record.Group == "BaseLocalDefences");
        var flatES = flatEsMod?.StatValue[0] ?? 0;

        var hybridFlatEsMod = mods.SingleOrDefault(mod => mod.Record.Group == "BaseLocalDefencesAndLife");
        var hybridFlatEs = hybridFlatEsMod?.StatValue[0] ?? 0;

        var percentEsMod = mods.SingleOrDefault(mod => mod.Record.Group == "DefencesPercent");
        var percentEs = percentEsMod?.StatValue[0] ?? 0;

        var hybridPercentEsMod = mods.SingleOrDefault(mod => mod.Record.Group == "DefencesPercentAndStunRecovery");
        var hybridPercentEs = hybridPercentEsMod?.StatValue[0] ?? 0;

        var es = (BaseEnergyShield + flatES + hybridFlatEs) * (1 + percentEs / 100.0 + hybridPercentEs / 100.0  + 0.2);
        return Convert.ToInt32(es);
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "BaseLocalDefences" || mod.Record.Group == "BaseLocalDefencesAndLife" || mod.Record.Group == "DefencesPercent" || mod.Record.Group == "DefencesPercentAndStunRecovery");
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }

    public override async Task Craft()
    {
        await Setup();

        // Check for chaos spam
        if (!HasCurrency(TradeCurrencyType.Chaos))
            return;

        // Check for path to rare
        if (!HasPathToRare)
            return;

        int maxEs = 0;
        try
        {
            await MakeRare();
            await StartUsingCurrency(TradeCurrencyType.Chaos);
            for(int i = 0; i < 200; i++)
            {
                if(HasCurrency(TradeCurrencyType.Chaos))
                    await ClickItem();
                
                while (HasCurrency(TradeCurrencyType.Exalted) && HasRemainingMods() && (GetNumberOfSuffixes() < 2 || GetNumberOfPrefixes() < 3) && CalculateEnergyShield() > MinEnergyShield-50)
                    await UseCurrency(TradeCurrencyType.Exalted);

                maxEs = Math.Max(maxEs, CalculateEnergyShield());

                if (CalculateEnergyShield() > MinEnergyShield)
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
            Console.WriteLine($"Finished with maximum energy shield of {maxEs}");
            Console.ReadLine();
        }
    }
}

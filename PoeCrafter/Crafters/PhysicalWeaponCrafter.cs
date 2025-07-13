using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public abstract class PhysicalWeaponCrafter : CrafterBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(PhysicalWeaponCrafter));

    protected PhysicalWeaponCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm) { }
    
    protected abstract double AttackSpeed { get; }
    protected abstract double MinDamage { get; }
    protected abstract double MaxDamage { get; }

    private double CalculateWeaponDPS()
    {
        var mods = GetCraftingMods();

        var physicalDamage = mods.SingleOrDefault(mod => mod.Record.Group == "PhysicalDamage");
        var physicalDamageMin = physicalDamage?.StatValue[0] ?? 0;
        var physicalDamageMax = physicalDamage?.StatValue[1] ?? 0;

        var ias = mods.SingleOrDefault(mod => mod.Record.Group == "IncreasedAttackSpeed")?.StatValue[0] ?? 0;
        var modifiedAttackSpeed = AttackSpeed * (1 + ias / 100.0);

        var ipd = mods.SingleOrDefault(mod => mod.Record.Group == "LocalPhysicalDamagePercent")?.StatValue[0] ?? 0;
        var hybridipd = mods.SingleOrDefault(mod => mod.Record.Group == "LocalIncreasedPhysicalDamagePercentAndAccuracyRating")?.StatValue[0] ?? 0;
        var modifiedPhysicalDamage = 1 + 0.2 + ipd / 100.0 + hybridipd / 100.0;

        var dps = (MinDamage + MaxDamage + physicalDamageMin + physicalDamageMax) / 2 * modifiedAttackSpeed * modifiedPhysicalDamage;
        return dps;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "PhysicalDamage" || mod.Record.Group == "LocalPhysicalDamagePercent" || mod.Record.Group == "LocalIncreasedPhysicalDamagePercentAndAccuracyRating");
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 1 - GetCraftingMods().Count(mod => mod.Record.Group == "IncreasedAttackSpeed");
    }

    private bool HasT2PhysicalDamageMod()
    {
        var physicalDamageMod = GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "PhysicalDamage");
        return physicalDamageMod?.Tier >= 2;
    }

    private bool HasT2PercentPhysicalDamageMod()
    {
        var percentPhysicalDamageMod = GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "LocalPhysicalDamagePercent");
        return percentPhysicalDamageMod?.Tier <= 2;
    }

    private bool HasT2HybridPhysicalDamageMod()
    {
        var physicalDamageMod = GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "LocalIncreasedPhysicalDamagePercentAndAccuracyRating");
        return physicalDamageMod?.Tier <= 2;
    }

    private bool HasT2AttackSpeedMod()
    {
        var percentPhysicalDamageMod = GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedAttackSpeed");
        return percentPhysicalDamageMod?.Tier <= 2;
    }

    private bool EnoughT2Mods()
    {
        List<Func<bool>> checkerList = new List<Func<bool>> {HasT2PhysicalDamageMod, HasT2PercentPhysicalDamageMod, HasT2HybridPhysicalDamageMod, HasT2AttackSpeedMod};
        return checkerList.Count(checker => checker.Invoke()) >= 2;
    }

    public override async Task Craft()
    {
        // Check for chaos spam
        if (!HasCurrency(CurrencyType.chaos))
            return;

        // Check for path to rare
        if (!HasPathToRare)
            return;

        double maxDps = 0;
        try
        {
            int attempts = 0;
            await MakeRare();
            await StartUsingCurrency(CurrencyType.chaos);
            while (CalculateWeaponDPS() < 350 && attempts < 100)
            {
                await ClickItem();
                while (HasCurrency(CurrencyType.exalted) && HasRemainingMods() && EnoughT2Mods())
                {
                    await UseCurrency(CurrencyType.exalted);
                }

                var weaponDps = CalculateWeaponDPS();
                maxDps = Math.Max(maxDps, weaponDps);
                attempts++;
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
            Console.WriteLine($"Finished with maximum DPS of {maxDps}");
            Console.ReadLine();
        }
    }
}

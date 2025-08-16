using System;
using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;

namespace PoeCrafter.Crafters;

public class MoveSpeedBoots : CrafterBase
{
    public MoveSpeedBoots(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    private int CalculateLife()
    {
        var mods = GetCraftingMods();
        var lifeMod = mods.SingleOrDefault(mod => mod.Record.Group == "IncreasedLife");
        var life = lifeMod?.StatValue[0] ?? 0;

        var hybridLifeMod = mods.SingleOrDefault(mod => mod.Record.Group == "BaseLocalDefencesAndLife");
        var hybridLife = hybridLifeMod?.StatValue[1] ?? 0;

        var strengthMod = mods.SingleOrDefault(mod => mod.Record.Group == "Strength");
        var strength = strengthMod?.StatValue[0] ?? 0;
        
        return life + hybridLife + strength/2;
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

        int maxLife = 0;
        try
        {
            await MakeRare();
            await StartUsingCurrency(TradeCurrencyType.Chaos);
            for (int i = 0; i < 200; i++)
            {
                if (HasCurrency(TradeCurrencyType.Chaos))
                    await ClickItem();

                while (HasCurrency(TradeCurrencyType.Exalted) && HasMoveSpeed && (GetNumberOfSuffixes() < 2 || GetNumberOfPrefixes() < 3) && CalculateLife() > 50)
                    await UseCurrency(TradeCurrencyType.Exalted);

                maxLife = Math.Max(maxLife, CalculateLife());

                if (CalculateLife() > 70 && HasMoveSpeed)
                {
                    Console.WriteLine("Found acceptable item");
                    Console.WriteLine("Press <Enter> to retry");
                    Console.ReadLine();
                }
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
            Console.WriteLine($"Finished with maximum life of {maxLife}");
            Console.ReadLine();
        }
    }
    
    private bool HasMoveSpeed => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "MovementVelocity" && mod.Tier <= 3 && mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix) != null;

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3;
    }
}

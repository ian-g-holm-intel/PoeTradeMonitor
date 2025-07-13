using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class AmuletCrafter : CrafterBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RingCrafter));

    public AmuletCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    public override async Task Craft()
    {
        await Setup();

        // Check for chaos spam
        if (!HasCurrency(CurrencyType.chaos))
            return;

        try
        {
            while (HasCurrency(CurrencyType.chaos))
            {
                if (CheckMods())
                {
                    break;
                }

                await UseCurrency(CurrencyType.chaos);

                await Task.Delay(25);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            Console.ReadLine();
        }
    }

    private bool CheckMods()
    {
        var mods = GetCraftingMods().ToArray();

        //if (InfluencedPrefixes == 3 || (InfluencedPrefixes >= 2 && InfluencedSuffixes >= 2))
        //{
        //    Console.WriteLine("SUCCESS! Make yourself a sandwich");
        //    return true;
        //}

        if (HasCritMulti && InfluencedMods >= 2)
        {
            Console.WriteLine("SUCCESS! Make yourself a sandwich");
            return true;
        }

        return false;
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 0;
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 0;
    }

    private bool HasInfluence => NonInfluencedMods <= 1;

    private int NonInfluencedMods => GetCraftingMods().Count(m => m.Record.InfluenceType == ExileCore.Shared.Enums.InfluenceTypes.None && (m.AffixType == ExileCore.Shared.Enums.ModType.Prefix || m.AffixType == ExileCore.Shared.Enums.ModType.Suffix));
    
    private int InfluencedMods => GetCraftingMods().Count(m => m.Record.InfluenceType != ExileCore.Shared.Enums.InfluenceTypes.None && (m.AffixType == ExileCore.Shared.Enums.ModType.Prefix || m.AffixType == ExileCore.Shared.Enums.ModType.Suffix));

    private int InfluencedPrefixes => GetCraftingMods().Count(m => m.Record.InfluenceType != ExileCore.Shared.Enums.InfluenceTypes.None && m.AffixType == ExileCore.Shared.Enums.ModType.Prefix);

    private int InfluencedSuffixes => GetCraftingMods().Count(m => m.Record.InfluenceType != ExileCore.Shared.Enums.InfluenceTypes.None && m.AffixType == ExileCore.Shared.Enums.ModType.Suffix);

    private int NonInfluencedPrefixes => GetCraftingMods().Count(m => m.Record.InfluenceType == ExileCore.Shared.Enums.InfluenceTypes.None && m.AffixType == ExileCore.Shared.Enums.ModType.Prefix);

    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedLife" && mod.Tier == 1) != null;

    private bool HasMana => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedMana" && mod.Tier == 1) != null;

    private bool HasCritMulti => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "CriticalStrikeMultiplier" && mod.Tier == 1) != null;
}

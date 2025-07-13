using System.Linq;
using System.Threading.Tasks;
using PoeHudWrapper;
using PoeLib;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class ManaGearCrafter : CrafterBase
{
    public ManaGearCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm) { }

    public override async Task Craft()
    {
        await StartUsingCurrency(CurrencyType.alt);
        for (int i = 0; i < 799; i++)
        {
            await ClickItem();

            if (HasLife && HasManaRecovery)
                break;

            if (HasOneMod && (HasManaRecovery || HasLife))
                await UseCurrency(CurrencyType.aug);

            if (HasLife && HasManaRecovery)
                break;

            await Task.Delay(100);
        }
        await StopUsingCurrency();
    }

    protected override int GetNumberOfRemainingPrefixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "IncreasedLife" && mod.Tier <= 3);
    }

    protected override int GetNumberOfRemainingSuffixes()
    {
        return 3 - GetCraftingMods().Count(mod => mod.Record.Group == "ManaRecoveryRate" && mod.Record.Tier == "Tier 1");
    }

    private bool HasLife => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "IncreasedLife" && mod.Tier <= 3 && mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix) != null;
    private bool HasManaRecovery => GetCraftingMods().SingleOrDefault(mod => mod.Record.Group == "ManaRecoveryRate" && mod.Record.Tier == "Tier 1" && mod.AffixType == ExileCore.Shared.Enums.ModType.Suffix) != null;
    private bool HasOneMod => GetCraftingMods().Count() < 3;
}

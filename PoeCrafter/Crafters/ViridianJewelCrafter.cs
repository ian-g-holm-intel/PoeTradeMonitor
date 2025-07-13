using PoeCrafter.ModGroups;
using PoeHudWrapper;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class ViridianJewelCrafter : JewelCrafter
{
    public ViridianJewelCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    protected override ModGroupBase[] ModGroups => new ModGroupBase[] { new EleHitBowsViridian(), new MoltenStrikeViridian() };
}

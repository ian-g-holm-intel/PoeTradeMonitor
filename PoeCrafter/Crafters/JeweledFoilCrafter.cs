using PoeHudWrapper;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class JeweledFoilCrafter : PhysicalWeaponCrafter
{
    public JeweledFoilCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    protected override double AttackSpeed => 1.6;
    protected override double MinDamage => 32;
    protected override double MaxDamage => 60;
}

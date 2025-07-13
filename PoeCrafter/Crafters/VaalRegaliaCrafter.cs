using PoeHudWrapper;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class VaalRegaliaCrafter : EnergyShieldArmorCrafter
{
    public VaalRegaliaCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm) { }

    protected override int BaseEnergyShield => 163;
    protected override int MinEnergyShield => 400;
}

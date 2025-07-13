using PoeHudWrapper;
using TradeBotLib;

namespace PoeCrafter.Crafters;

public class HubrisCircletCrafter : EnergyShieldArmorCrafter
{
    public HubrisCircletCrafter(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm) : base(phw, tc, rsm)
    {
    }

    protected override int BaseEnergyShield => 76;
    protected override int MinEnergyShield => 270;
}

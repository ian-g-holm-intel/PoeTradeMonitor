using System.Collections.Generic;

namespace PoeCrafter;

public class AffixLookup
{
    public Affix GetAffix(ModType type, List<double> values)
    {
        switch (type)
        {
            case ModType.PercentPhysical:
                return new PercentPhysicalAffix(values[0]);
            case ModType.LevelSocketedGems:
                return new LevelSocketedGemsAffix(values[0]);
            case ModType.LevelSocketedMeleeGems:
                return new LevelSocketedMeleeGemsAffix(values[0]);
            case ModType.LifeLeech:
                return new LifeLeechAffix(values[0]);
            case ModType.ManaLeech:
                return new ManaLeechAffix(values[0]);
            case ModType.ElementalDamageWithAttacks:
                return new ElementalDamageWithAttacksAffix(values[0]);
            case ModType.FlatAccuracy:
                return new FlatAccuracyAffix(values[0]);
            case ModType.AttackSpeed:
                return new AttackSpeedAffix(values[0]);
            case ModType.Dexterity:
                return new DexterityAffix(values[0]);
            case ModType.Strength:
                return new StrengthAffix(values[0]);
            case ModType.CritChance:
                return new CritChanceAffix(values[0]);
            case ModType.CritMultiplier:
                return new CritMultiplierAffix(values[0]);
            case ModType.LifeOnKill:
                return new LifeOnKillAffix(values[0]);
            case ModType.LifeOnHit:
                return new LifeOnHitAffix(values[0]);
            case ModType.ManaOnKill:
                return new ManaOnKillAffix(values[0]);
            case ModType.ColdResistance:
                return new ColdResistanceAffix(values[0]);
            case ModType.FireResistance:
                return new FireResistanceAffix(values[0]);
            case ModType.LightningResistance:
                return new LightningResistanceAffix(values[0]);
            case ModType.ChaosResistance:
                return new ChaosResistanceAffix(values[0]);
            case ModType.ReducedAttributeReq:
                return new ReducedAttributeReqAffix(values[0]);
            case ModType.StunDuration:
                return new StunDurationReqAffix(values[0]);
            case ModType.StunThreshold:
                return new StunThresholdReqAffix(values[0]);
            case ModType.DamageWithPoison:
                return new DamageWithPoisonAffix(values[0]);
            case ModType.ChanceToPoison:
                return new ChanceToPoisonAffix(values[0]);
            case ModType.PoisonDuration:
                return new PoisonDurationAffix(values[0]);
            case ModType.DamageWithBleeding:
                return new DamageWithBleedingAffix(values[0]);
            case ModType.ChanceToBleed:
                return new ChanceToBleedAffix(values[0]);
            case ModType.BleedDuration:
                return new BleedDurationAffix(values[0]);
            case ModType.FlatPhysical:
                return new FlatPhysicalAffix(values[0], values[1]);
            case ModType.HybridPhysical:
                return new HybridPhysicalAffix(values[0], values[1]);
            case ModType.FlatCold:
                return new FlatColdAffix(values[0], values[1]);
            case ModType.FlatFire:
                return new FlatFireAffix(values[0], values[1]);
            case ModType.FlatLightning:
                return new FlatLightningAffix(values[0], values[1]);
            case ModType.FlatChaos:
                return new FlatChaosAffix(values[0], values[1]);
            case ModType.LightRadiusAndAccuracy:
                return new LightRadiusAndAccuracyAffix(values[0], values[1]);
            default:
                return new UnknownAffix();
        }
    }
}

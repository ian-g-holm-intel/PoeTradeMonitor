namespace PoeCrafter;

public class FlatAccuracyAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.FlatAccuracy;

    public FlatAccuracyAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 251)
            return AffixTier.Tier1;
        if (Value >= 201)
            return AffixTier.Tier2;
        if (Value >= 166)
            return AffixTier.Tier3;
        if (Value >= 131)
            return AffixTier.Tier4;
        if (Value >= 101)
            return AffixTier.Tier5;
        if (Value >= 61)
            return AffixTier.Tier6;
        if (Value >= 16)
            return AffixTier.Tier7;
        if (Value >= 5)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class LightRadiusAndAccuracyAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.LightRadiusAndAccuracy;
    public override bool IsHybrid => true;

    public LightRadiusAndAccuracyAffix(double lightRadius, double accuracy) : base(lightRadius, lightRadius) { }

    protected override AffixTier ParseTier()
    {
        if (FirstValue == 15)
            return AffixTier.Tier1;
        if (FirstValue == 10)
            return AffixTier.Tier2;
        if (FirstValue == 5)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class AttackSpeedAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.AttackSpeed;

    public AttackSpeedAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 26)
            return AffixTier.Tier1;
        if (Value >= 23)
            return AffixTier.Tier2;
        if (Value >= 20)
            return AffixTier.Tier3;
        if (Value >= 17)
            return AffixTier.Tier4;
        if (Value >= 14)
            return AffixTier.Tier5;
        if (Value >= 11)
            return AffixTier.Tier6;
        if (Value >= 8)
            return AffixTier.Tier7;
        if (Value >= 5)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class DexterityAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.Dexterity;

    public DexterityAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 51)
            return AffixTier.Tier1;
        if (Value >= 43)
            return AffixTier.Tier2;
        if (Value >= 38)
            return AffixTier.Tier3;
        if (Value >= 33)
            return AffixTier.Tier4;
        if (Value >= 28)
            return AffixTier.Tier5;
        if (Value >= 23)
            return AffixTier.Tier6;
        if (Value >= 18)
            return AffixTier.Tier7;
        if (Value >= 13)
            return AffixTier.Tier8;
        if (Value >= 8)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class StrengthAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.Strength;

    public StrengthAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 51)
            return AffixTier.Tier1;
        if (Value >= 43)
            return AffixTier.Tier2;
        if (Value >= 38)
            return AffixTier.Tier3;
        if (Value >= 33)
            return AffixTier.Tier4;
        if (Value >= 28)
            return AffixTier.Tier5;
        if (Value >= 23)
            return AffixTier.Tier6;
        if (Value >= 18)
            return AffixTier.Tier7;
        if (Value >= 13)
            return AffixTier.Tier8;
        if (Value >= 8)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class CritChanceAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.CritChance;

    public CritChanceAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 35)
            return AffixTier.Tier1;
        if (Value >= 30)
            return AffixTier.Tier2;
        if (Value >= 25)
            return AffixTier.Tier3;
        if (Value >= 20)
            return AffixTier.Tier4;
        if (Value >= 15)
            return AffixTier.Tier5;
        if (Value >= 10)
            return AffixTier.Tier6;
        return AffixTier.Unknown;
    }
}

public class CritMultiplierAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.CritMultiplier;

    public CritMultiplierAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 35)
            return AffixTier.Tier1;
        if (Value >= 30)
            return AffixTier.Tier2;
        if (Value >= 25)
            return AffixTier.Tier3;
        if (Value >= 20)
            return AffixTier.Tier4;
        if (Value >= 15)
            return AffixTier.Tier5;
        if (Value >= 10)
            return AffixTier.Tier6;
        return AffixTier.Unknown;
    }
}

public class LifeOnKillAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.LifeOnKill;

    public LifeOnKillAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 11)
            return AffixTier.Tier1;
        if (Value >= 7)
            return AffixTier.Tier2;
        if (Value >= 3)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class LifeOnHitAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.LifeOnHit;

    public LifeOnHitAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value == 5)
            return AffixTier.Tier1;
        if (Value == 4)
            return AffixTier.Tier2;
        if (Value == 3)
            return AffixTier.Tier3;
        if (Value == 2)
            return AffixTier.Tier4;
        return AffixTier.Unknown;
    }
}

public class ManaOnKillAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ManaOnKill;

    public ManaOnKillAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 4)
            return AffixTier.Tier1;
        if (Value >= 2)
            return AffixTier.Tier2;
        if (Value == 1)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class ColdResistanceAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ColdResistance;

    public ColdResistanceAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 46)
            return AffixTier.Tier1;
        if (Value >= 42)
            return AffixTier.Tier2;
        if (Value >= 36)
            return AffixTier.Tier3;
        if (Value >= 30)
            return AffixTier.Tier4;
        if (Value >= 24)
            return AffixTier.Tier5;
        if (Value >= 18)
            return AffixTier.Tier6;
        if (Value >= 12)
            return AffixTier.Tier7;
        if (Value >= 6)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class FireResistanceAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.FireResistance;

    public FireResistanceAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 46)
            return AffixTier.Tier1;
        if (Value >= 42)
            return AffixTier.Tier2;
        if (Value >= 36)
            return AffixTier.Tier3;
        if (Value >= 30)
            return AffixTier.Tier4;
        if (Value >= 24)
            return AffixTier.Tier5;
        if (Value >= 18)
            return AffixTier.Tier6;
        if (Value >= 12)
            return AffixTier.Tier7;
        if (Value >= 6)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class LightningResistanceAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.LightningResistance;

    public LightningResistanceAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 46)
            return AffixTier.Tier1;
        if (Value >= 42)
            return AffixTier.Tier2;
        if (Value >= 36)
            return AffixTier.Tier3;
        if (Value >= 30)
            return AffixTier.Tier4;
        if (Value >= 24)
            return AffixTier.Tier5;
        if (Value >= 18)
            return AffixTier.Tier6;
        if (Value >= 12)
            return AffixTier.Tier7;
        if (Value >= 6)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class ChaosResistanceAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ChaosResistance;

    public ChaosResistanceAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 31)
            return AffixTier.Tier1;
        if (Value >= 26)
            return AffixTier.Tier2;
        if (Value >= 21)
            return AffixTier.Tier3;
        if (Value >= 16)
            return AffixTier.Tier4;
        if (Value >= 11)
            return AffixTier.Tier5;
        if (Value >= 5)
            return AffixTier.Tier6;
        return AffixTier.Unknown;
    }
}

public class ReducedAttributeReqAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ReducedAttributeReq;

    public ReducedAttributeReqAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value == 32)
            return AffixTier.Tier1;
        if (Value == 18)
            return AffixTier.Tier2;
        return AffixTier.Unknown;
    }
}

public class StunDurationReqAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.StunDuration;

    public StunDurationReqAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 31)
            return AffixTier.Tier1;
        if (Value >= 26)
            return AffixTier.Tier2;
        if (Value >= 21)
            return AffixTier.Tier3;
        if (Value >= 16)
            return AffixTier.Tier4;
        if (Value >= 11)
            return AffixTier.Tier5;
        return AffixTier.Unknown;
    }
}

public class StunThresholdReqAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.StunThreshold;

    public StunThresholdReqAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 14)
            return AffixTier.Tier1;
        if (Value >= 12)
            return AffixTier.Tier2;
        if (Value >= 10)
            return AffixTier.Tier3;
        if (Value >= 8)
            return AffixTier.Tier4;
        if (Value >= 5)
            return AffixTier.Tier5;
        return AffixTier.Unknown;
    }
}

public class DamageWithPoisonAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.DamageWithPoison;

    public DamageWithPoisonAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 31)
            return AffixTier.Tier1;
        if (Value >= 26)
            return AffixTier.Tier2;
        if (Value >= 21)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class ChanceToPoisonAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ChanceToPoison;

    public ChanceToPoisonAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value == 30)
            return AffixTier.Tier1;
        if (Value == 20)
            return AffixTier.Tier2;
        if (Value == 10)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class PoisonDurationAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.PoisonDuration;

    public PoisonDurationAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 13)
            return AffixTier.Tier1;
        if (Value >= 8)
            return AffixTier.Tier2;
        return AffixTier.Unknown;
    }
}

public class DamageWithBleedingAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.DamageWithBleeding;

    public DamageWithBleedingAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 31)
            return AffixTier.Tier1;
        if (Value >= 26)
            return AffixTier.Tier2;
        if (Value >= 21)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class ChanceToBleedAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.ChanceToBleed;

    public ChanceToBleedAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value == 20)
            return AffixTier.Tier1;
        if (Value == 15)
            return AffixTier.Tier2;
        if (Value == 10)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class BleedDurationAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Suffix;
    public override ModType ModType => ModType.BleedDuration;

    public BleedDurationAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 13)
            return AffixTier.Tier1;
        if (Value >= 8)
            return AffixTier.Tier2;
        return AffixTier.Unknown;
    }
}

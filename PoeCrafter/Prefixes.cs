namespace PoeCrafter;

public class FlatPhysicalAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.FlatPhysical;

    public FlatPhysicalAffix(double lowValue, double highValue) : base(lowValue, highValue) { }

    protected override AffixTier ParseTier()
    {
        if (FirstValue >= 25 || SecondValue >= 42)
            return AffixTier.Tier1;
        if (FirstValue >= 20 && SecondValue >= 41)
            return AffixTier.Unknown;
        if (FirstValue >= 17 && SecondValue >= 36)
            return AffixTier.Tier2;
        if (FirstValue >= 18 && SecondValue >= 31)
            return AffixTier.Tier3;
        if (FirstValue >= 14 && SecondValue >= 29)
            return AffixTier.Unknown;
        if (FirstValue >= 13 && SecondValue >= 26)
            return AffixTier.Tier4;
        if (SecondValue >= 20)
            return AffixTier.Tier5;
        if (SecondValue >= 16)
            return AffixTier.Tier6;
        if (SecondValue >= 13)
            return AffixTier.Tier7;
        if (SecondValue >= 8)
            return AffixTier.Tier8;
        if (FirstValue == 1)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class PercentPhysicalAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.PercentPhysical;

    public PercentPhysicalAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 170)
            return AffixTier.Tier1;
        if (Value >= 155)
            return AffixTier.Tier2;
        if (Value >= 135)
            return AffixTier.Tier3;
        if (Value >= 110)
            return AffixTier.Tier4;
        if (Value >= 85)
            return AffixTier.Tier5;
        if (Value >= 65)
            return AffixTier.Tier6;
        if (Value >= 50)
            return AffixTier.Tier7;
        if (Value >= 40)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class HybridPhysicalAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.HybridPhysical;
    public override bool IsHybrid => true;

    public HybridPhysicalAffix(double physDamage, double accuracy) : base(physDamage, accuracy) { }

    protected override AffixTier ParseTier()
    {
        if (FirstValue >= 75 && SecondValue >= 135)
            return AffixTier.Tier1;
        if (FirstValue >= 65 && SecondValue >= 100)
            return AffixTier.Tier2;
        if (FirstValue >= 55 && SecondValue >= 83)
            return AffixTier.Tier3;
        if (FirstValue >= 45 && SecondValue >= 65)
            return AffixTier.Tier4;
        if (FirstValue >= 35 && SecondValue >= 51)
            return AffixTier.Tier5;
        if (FirstValue >= 25 && SecondValue >= 31)
            return AffixTier.Tier6;
        if (FirstValue >= 20 && SecondValue >= 8)
            return AffixTier.Tier7;
        if (FirstValue >= 15 && SecondValue >= 3)
            return AffixTier.Tier8;
        return AffixTier.Unknown;
    }
}

public class LevelSocketedGemsAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.LevelSocketedGems;

    public LevelSocketedGemsAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        return AffixTier.Tier1;
    }
}

public class LevelSocketedMeleeGemsAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.LevelSocketedMeleeGems;

    public LevelSocketedMeleeGemsAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 2)
            return AffixTier.Tier1;
        return AffixTier.Tier2;
    }
}

public class LifeLeechAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.LifeLeech;

    public LifeLeechAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 1)
            return AffixTier.Tier1;
        if (Value >= 0.6)
            return AffixTier.Tier2;
        if (Value >= 0.2)
            return AffixTier.Tier3;
        return AffixTier.Unknown;
    }
}

public class ManaLeechAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.ManaLeech;

    public ManaLeechAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        return AffixTier.Tier1;
    }
}

public class ElementalDamageWithAttacksAffix : SingleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.ElementalDamageWithAttacks;

    public ElementalDamageWithAttacksAffix(double value) : base(value) { }

    protected override AffixTier ParseTier()
    {
        if (Value >= 38)
            return AffixTier.Tier1;
        if (Value >= 31)
            return AffixTier.Tier2;
        if (Value >= 21)
            return AffixTier.Tier3;
        if (Value >= 11)
            return AffixTier.Tier4;
        if (Value >= 5)
            return AffixTier.Tier5;
        return AffixTier.Unknown;
    }
}

public class FlatColdAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.FlatCold;

    public FlatColdAffix(double lowValue, double highValue) : base(lowValue, highValue) { }

    protected override AffixTier ParseTier()
    {
        if (SecondValue >= 74)
            return AffixTier.Tier1;
        if (SecondValue >= 63)
            return AffixTier.Tier2;
        if (SecondValue >= 52)
            return AffixTier.Tier3;
        if (SecondValue >= 43)
            return AffixTier.Tier4;
        if (SecondValue >= 34)
            return AffixTier.Tier5;
        if (SecondValue >= 27)
            return AffixTier.Tier6;
        if (SecondValue >= 19)
            return AffixTier.Tier7;
        if (SecondValue >= 13)
            return AffixTier.Tier8;
        if (SecondValue == 3)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class FlatFireAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.FlatFire;

    public FlatFireAffix(double lowValue, double highValue) : base(lowValue, highValue) { }

    protected override AffixTier ParseTier()
    {
        if (SecondValue >= 91)
            return AffixTier.Tier1;
        if (SecondValue >= 77)
            return AffixTier.Tier2;
        if (SecondValue >= 63)
            return AffixTier.Tier3;
        if (SecondValue >= 53)
            return AffixTier.Tier4;
        if (SecondValue >= 42)
            return AffixTier.Tier5;
        if (SecondValue >= 33)
            return AffixTier.Tier6;
        if (SecondValue >= 24)
            return AffixTier.Tier7;
        if (SecondValue >= 15)
            return AffixTier.Tier8;
        if (SecondValue >= 3)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class FlatLightningAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.FlatLightning;

    public FlatLightningAffix(double lowValue, double highValue) : base(lowValue, highValue) { }

    protected override AffixTier ParseTier()
    {
        if (SecondValue >= 158)
            return AffixTier.Tier1;
        if (SecondValue >= 133)
            return AffixTier.Tier2;
        if (SecondValue >= 110)
            return AffixTier.Tier3;
        if (SecondValue >= 91)
            return AffixTier.Tier4;
        if (SecondValue >= 72)
            return AffixTier.Tier5;
        if (SecondValue >= 58)
            return AffixTier.Tier6;
        if (SecondValue >= 41)
            return AffixTier.Tier7;
        if (SecondValue >= 27)
            return AffixTier.Tier8;
        if (SecondValue == 6)
            return AffixTier.Tier9;
        return AffixTier.Unknown;
    }
}

public class FlatChaosAffix : DoubleValueAffix
{
    public override AffixType Type => AffixType.Prefix;
    public override ModType ModType => ModType.FlatChaos;

    public FlatChaosAffix(double lowValue, double highValue) : base(lowValue, highValue) { }

    protected override AffixTier ParseTier()
    {
        return AffixTier.Tier1;
    }
}

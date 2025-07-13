using System;
using System.ComponentModel;

namespace PoeCrafter;

public abstract class Affix : IEquatable<Affix>
{
    public string Name
    {
        get
        {
            var fi = ModType.GetType().GetField(ModType.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length <= 0 ? ModType.ToString() : attributes[0].Description;
        }
    }

    public abstract AffixType Type { get; }
    public abstract ModType ModType { get; }
    public AffixTier Tier { get; set; }
    public virtual bool IsHybrid => false;
    protected abstract AffixTier ParseTier();

    public bool Equals(Affix other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && ModType == other.ModType && Tier == other.Tier;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Affix) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (int) Type;
            hashCode = (hashCode * 397) ^ (int) ModType;
            hashCode = (hashCode * 397) ^ (int) Tier;
            return hashCode;
        }
    }
}



public class UnknownAffix : Affix
{
    public override AffixType Type => AffixType.Unknown;
    public override ModType ModType => ModType.Unknown;
    protected override AffixTier ParseTier()
    {
        return AffixTier.Unknown;
    }
}

public abstract class SingleValueAffix : Affix
{
    public double Value { get; }

    protected SingleValueAffix(double value)
    {
        Value = value;
        Tier = ParseTier();
    }
}

public abstract class DoubleValueAffix : Affix
{
    public double FirstValue { get; }
    public double SecondValue { get; }

    protected DoubleValueAffix(double firstValue, double secondValue)
    {
        FirstValue = firstValue;
        SecondValue = secondValue;
        Tier = ParseTier();
    }
}

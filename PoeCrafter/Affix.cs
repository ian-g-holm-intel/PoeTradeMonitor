using System;
using System.ComponentModel;

namespace PoeCrafter;

/// <summary>
/// Abstract base class representing an affix (modifier) on Path of Exile items.
/// Affixes provide bonuses and properties to items such as weapons, armor, and accessories.
/// </summary>
public abstract class Affix : IEquatable<Affix>
{
    /// <summary>
    /// Gets the display name of the affix, using the Description attribute if available.
    /// </summary>
    public string Name
    {
        get
        {
            var fi = ModType.GetType().GetField(ModType.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length <= 0 ? ModType.ToString() : attributes[0].Description;
        }
    }

    /// <summary>
    /// Gets the type of affix (prefix, suffix, etc.).
    /// </summary>
    public abstract AffixType Type { get; }
    
    /// <summary>
    /// Gets the specific modifier type this affix represents.
    /// </summary>
    public abstract ModType ModType { get; }
    
    /// <summary>
    /// Gets or sets the tier of this affix, indicating its power level.
    /// </summary>
    public AffixTier Tier { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether this affix is a hybrid modifier.
    /// </summary>
    public virtual bool IsHybrid => false;
    
    /// <summary>
    /// Parses and determines the tier of this affix based on its values.
    /// </summary>
    /// <returns>The calculated affix tier.</returns>
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



/// <summary>
/// Represents an affix that could not be identified or parsed.
/// </summary>
public class UnknownAffix : Affix
{
    public override AffixType Type => AffixType.Unknown;
    public override ModType ModType => ModType.Unknown;
    protected override AffixTier ParseTier()
    {
        return AffixTier.Unknown;
    }
}

/// <summary>
/// Abstract base class for affixes that have a single numeric value.
/// </summary>
public abstract class SingleValueAffix : Affix
{
    /// <summary>
    /// Gets the numeric value of this affix.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleValueAffix"/> class.
    /// </summary>
    /// <param name="value">The numeric value of the affix.</param>
    protected SingleValueAffix(double value)
    {
        Value = value;
        Tier = ParseTier();
    }
}

/// <summary>
/// Abstract base class for affixes that have two numeric values.
/// </summary>
public abstract class DoubleValueAffix : Affix
{
    /// <summary>
    /// Gets the first numeric value of this affix.
    /// </summary>
    public double FirstValue { get; }
    
    /// <summary>
    /// Gets the second numeric value of this affix.
    /// </summary>
    public double SecondValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleValueAffix"/> class.
    /// </summary>
    /// <param name="firstValue">The first numeric value of the affix.</param>
    /// <param name="secondValue">The second numeric value of the affix.</param>
    protected DoubleValueAffix(double firstValue, double secondValue)
    {
        FirstValue = firstValue;
        SecondValue = secondValue;
        Tier = ParseTier();
    }
}

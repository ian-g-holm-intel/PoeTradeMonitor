using System;

namespace PoeCrafter;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class AffixPatternAttribute : Attribute
{
    public string AffixPattern { get; }

    public AffixPatternAttribute(string pattern)
    {
        AffixPattern = pattern;
    }
}

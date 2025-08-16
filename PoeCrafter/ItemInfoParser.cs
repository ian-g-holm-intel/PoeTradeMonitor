using System;
using System.Collections.Generic;

namespace PoeCrafter;

/// <summary>
/// Parses Path of Exile item information text to extract and identify affixes (modifiers).
/// </summary>
public class ItemInfoParser
{
    private readonly AffixParser affixParser = new AffixParser();
    /// <summary>
    /// Parses item information text and extracts all affixes from the modifiers section.
    /// </summary>
    /// <param name="itemInfo">The complete item information text from Path of Exile.</param>
    /// <returns>A list of parsed affixes found on the item.</returns>
    public List<Affix> Parse(string itemInfo)
    {
        string[] infoSections = itemInfo.Split(new[] {"--------"}, StringSplitOptions.RemoveEmptyEntries);
        var mods = infoSections[6];
        List<Affix> affixes = new List<Affix>();
        for (int i = 0; i < mods.Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Length; i++)
        {
            var affixList = affixParser.Parse(mods, i);
            foreach (var affix in affixList)
            {
                if (!affixes.Contains(affix))
                    affixes.Add(affix);
            }
        }
        return affixes;
    }
}

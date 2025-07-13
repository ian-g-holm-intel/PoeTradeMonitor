using System;
using System.Collections.Generic;

namespace PoeCrafter;

public class ItemInfoParser
{
    private readonly AffixParser affixParser = new AffixParser();
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

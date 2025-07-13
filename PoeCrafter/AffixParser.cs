using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PoeLib.JSON;

namespace PoeCrafter;

public class AffixParser
{
    private readonly AffixLookup affixLookup = new AffixLookup();
    public List<Affix> Parse(string modString, int index)
    {
        try
        {
            var modLines = modString.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var mods = ItemMods.GetMods(modLines);
            var mod = mods[index];

            var affixList = new List<Affix>();
            foreach (var modType in (ModType[]) Enum.GetValues(typeof(ModType)))
            {
                var fi = modType.GetType().GetField(modType.ToString());
                var attributes = (AffixPatternAttribute[]) fi.GetCustomAttributes(typeof(AffixPatternAttribute), false);

                if (attributes.Length == 1)
                {
                    if (new Regex(attributes[0].AffixPattern).IsMatch(mod.ModText))
                    {
                        affixList.Add(affixLookup.GetAffix(modType, mod.Values));
                    }
                }
                else
                {
                    if (attributes.Any(attr => new Regex(attr.AffixPattern).IsMatch(mod.ModText)))
                    {
                        if (attributes.All(attr => mods.Any(m => new Regex(attr.AffixPattern).IsMatch(m.ModText))))
                        {
                            List<double> modValues = new List<double>();
                            foreach (var attribute in attributes)
                            {
                                modValues.Add(mods.Single(m => new Regex(attribute.AffixPattern).IsMatch(m.ModText)).Values[0]);
                            }
                            affixList.Add(affixLookup.GetAffix(modType, modValues));
                        }
                    }
                }

            }
            return affixList;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse mod string", ex);
        }
    }
}

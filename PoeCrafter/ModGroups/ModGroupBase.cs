using System;
using System.Linq;

namespace PoeCrafter.ModGroups;

public abstract class ModGroupBase
{
    protected abstract string[] mods { get; }
    public bool ContainsMod(string modName)
    {
        return mods.Any(mod => mod.Equals(modName, StringComparison.InvariantCultureIgnoreCase));
    }

    public bool IsLife(string modName)
    {
        return modName.Equals("Vivid", StringComparison.InvariantCultureIgnoreCase);
    }

    public bool IsEnergyShield(string modName)
    {
        return modName.Equals("Shimmering", StringComparison.InvariantCultureIgnoreCase);
    }
}

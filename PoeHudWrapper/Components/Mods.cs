using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets.Native;
using GameOffsets;
using ExileCore.PoEMemory;
using ExileCore.Shared.Helpers;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Components;

public class Mods : Component
{
    public ModsComponentOffsets ModsStruct => M.Read<ModsComponentOffsets>(Address);
    public ModsComponentStatsOffsets ModStatsStruct => M.Read<ModsComponentStatsOffsets>(ModsStruct.ModsComponentStatsPtr);

    //use structure spider, search type: StringU with Max level:3, search exact name with VMT option disabled
    public string UniqueName => GetUniqueName(ModsStruct.UniqueName);

    // public bool Identified => Address != 0 && M.Read<byte>(Address + 0x88) == 1;
    public bool Identified => Address != 0 && ModsStruct.Identified;
    public ItemRarity ItemRarity => Address != 0 ? (ItemRarity)ModsStruct.ItemRarity : ItemRarity.Normal;

    //Usefull for cache items
    public long Hash => ModsStruct.implicitMods.GetHashCode() ^ ModsStruct.explicitMods.GetHashCode() ^ ModsStruct.GetHashCode();

    public List<ItemModWrapper> ItemMods => ImplicitMods.Concat(ExplicitMods).Concat(EnchantedMods).Concat(ScourgeMods).Concat(CrucibleMods).ToList();
    public List<ItemModWrapper> ImplicitMods => GetMods(ModsStruct.implicitMods.First, ModsStruct.implicitMods.Last);
    public List<ItemModWrapper> ExplicitMods => GetMods(ModsStruct.explicitMods.First, ModsStruct.explicitMods.Last);
    public List<ItemModWrapper> EnchantedMods => GetMods(ModsStruct.enchantMods.First, ModsStruct.enchantMods.Last);
    public List<ItemModWrapper> ScourgeMods => GetMods(ModsStruct.ScourgeMods.First, ModsStruct.ScourgeMods.Last);
    public List<ItemModWrapper> CrucibleMods => GetMods(ModsStruct.crucibleMods.First, ModsStruct.crucibleMods.Last);
    public List<ItemModWrapper> FracturedMods => CountFractured > 0 ? ExplicitMods.Take(CountFractured).ToList() : new List<ItemModWrapper>();
    public List<ItemModWrapper> SynthesisMods => ImplicitMods.Where(mod => mod?.ModRecord?.Key?.StartsWith("SynthesisImplicit") ?? false).ToList();

    public int ItemLevel => Address != 0 ? ModsStruct.ItemLevel : 1;
    public int RequiredLevel => Address != 0 ? ModsStruct.RequiredLevel : 1;
    public bool IsUsable => Address != 0 && ModsStruct.IsUsable == 1;
    public bool IsMirrored => Address != 0 && ModsStruct.IsMirrored == 1;
    public int CountFractured => ModsStruct.FracturedModsCount;
    //public bool Synthesised => M.Read<byte>(Address + 0x437) == 1; // Unsure if there is a new location for this, can still get synth mods from implicit on unid items
    public bool Synthesised => CountSynthesised > 0;
    public int CountSynthesised => SynthesisMods.Count;
    public bool HaveFractured => CountFractured > 0;
    public ItemStats ItemStats => new ItemStats(Owner);
    public List<string> HumanStats => GetStats(ModStatsStruct.ExplicitStatsArray);
    public List<string> HumanCraftedStats => GetStats(ModStatsStruct.CraftedStatsArray);
    public List<string> HumanImpStats => GetStats(ModStatsStruct.ImplicitStatsArray);
    public List<string> FracturedStats => GetStats(ModStatsStruct.FracturedStatsArray);
    public List<string> EnchantedStats => GetStats(ModStatsStruct.EnchantedStatsArray);
    public List<string> CrucibleStats => GetStats(ModStatsStruct.CrucibleStatsArray);
    public ushort IncubatorKills => ModsStruct.IncubatorKills;
    public string IncubatorName => Address != 0 && ModsStruct.IncubatorPtr != 0 ? M.ReadStringU(M.Read<long>(ModsStruct.IncubatorPtr, 0x20)) : null;

    private List<string> GetStats(StdVector array)
    {
        var strStruct = M.ReadStdVector<NativeUtf16Text>(array);
        return strStruct.Select(p => p.ToString(M)).ToList();
    }

    private List<ItemModWrapper> GetMods(long startOffset, long endOffset)
    {
        var list = new List<ItemModWrapper>();

        if (Address == 0)
            return list;

        var begin = startOffset;
        var end = endOffset;
        var count = (end - begin) / ItemModWrapper.STRUCT_SIZE;

        if (count > 12)
            return list;

        //System.Windows.Forms.MessageBox.Show(begin.ToString("x"));

        for (var i = begin; i < end; i += ItemModWrapper.STRUCT_SIZE)
        {
            list.Add(GetObject<ItemModWrapper>(i));
        }

        return list;
    }

    private string GetUniqueName(NativePtrArray source)
    {
        var words = new List<string>();
        if (Address == 0 || source.Size / 8 > 1000) return string.Empty;

        for (var first = source.First; first < source.Last; first += ModsComponentOffsets.NameRecordSize)
        {
            words.Add(M.ReadStringU(M.Read<long>(first, ModsComponentOffsets.NameOffset)).Trim());
        }
        return Cache.StringCache.Read($"{nameof(Mods)}{source.First}", () => string.Join(" ", words));
    }
}
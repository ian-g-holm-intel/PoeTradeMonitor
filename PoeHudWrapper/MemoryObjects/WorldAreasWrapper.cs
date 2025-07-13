using ExileCore.PoEMemory;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace PoeHudWrapper.MemoryObjects;

public class WorldAreasWrapper : UniversalFileWrapperWrapper<WorldAreaWrapper>
{
    private int _indexCounter;

    public WorldAreasWrapper(IMemory m, Func<long> address) : base(m, address)
    {
    }

    public Dictionary<int, WorldAreaWrapper> AreasIndexDictionary { get; } = new Dictionary<int, WorldAreaWrapper>();
    public Dictionary<int, WorldAreaWrapper> AreasWorldIdDictionary { get; } = new Dictionary<int, WorldAreaWrapper>();

    public WorldAreaWrapper GetAreaByAreaId(int index)
    {
        CheckCache();

        AreasIndexDictionary.TryGetValue(index, out var area);
        return area;
    }

    public WorldAreaWrapper GetAreaByAreaId(string id)
    {
        CheckCache();
        return AreasIndexDictionary.First(area => area.Value.Id == id).Value;
    }

    public WorldAreaWrapper GetAreaByWorldId(int id)
    {
        CheckCache();
        AreasWorldIdDictionary.TryGetValue(id, out var area);
        return area;
    }

    protected override void EntryAdded(long addr, WorldAreaWrapper entry)
    {
        entry.Index = _indexCounter++;
        AreasIndexDictionary.Add(entry.Index, entry);
        AreasWorldIdDictionary.Add(entry.WorldAreaId, entry);
    }
}
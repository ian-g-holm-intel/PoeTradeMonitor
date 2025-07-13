using ExileCore.PoEMemory;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.MemoryObjects;
using GameOffsets;

namespace PoeHudWrapper.MemoryObjects;

public class WorldAreaWrapper : RemoteMemoryObject
{
    private string _id;
    private string _name;
    private List<WorldAreaWrapper> _connections;
    private List<MonsterVariety> _bosses;
    public string Id => _id ??= M.ReadStringU(M.Read<long>(Address));

    [Obsolete("Use Id")]
    public string RawName => Id;

    public int Index { get; set; }
    public string Name => _name ??= M.ReadStringU(M.Read<long>(Address + 8));
    public int Act => M.Read<int>(Address + 0x10);
    public bool IsTown => M.Read<byte>(Address + 0x14) == 1;
    public bool IsHideout => Id.StartsWith("Hideout", StringComparison.OrdinalIgnoreCase);
    public bool HasWaypoint => M.Read<byte>(Address + 0x15) == 1;
    public int AreaLevel => M.Read<int>(Address + 0x26);
    public int WorldAreaId => M.Read<int>(Address + 0x2a);
    public bool IsUnique => M.Read<bool>(Address + 0x1EC);

    public WorldAreaWrapper Area => GameWrapper.TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x0));

    public List<WorldAreaWrapper> Connections =>
        _connections ??= M.Read<DatArrayStruct>(Address + 0x16).ReadSelfDatPtr(M).Select(GameWrapper.TheGame.Files.WorldAreas.GetByAddress).Where(x => x != null).ToList();

    public List<MonsterVariety> Bosses =>
        _bosses ??= M.Read<DatArrayStruct>(Address + 0x9E).ReadDatPtr(M).Select(GameWrapper.TheGame.Files.MonsterVarieties.GetByAddress).Where(x => x != null).ToList();

    public override string ToString()
    {
        return $"{Name}";
    }
}
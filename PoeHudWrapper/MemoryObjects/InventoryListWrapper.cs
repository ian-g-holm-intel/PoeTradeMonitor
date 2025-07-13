using ExileCore.PoEMemory;
using ExileCore.Shared.Enums;

namespace PoeHudWrapper.MemoryObjects;

public class InventoryListWrapper : RemoteMemoryObject
{
    public static int InventoryCount => 57;

    public InventoryWrapper this[InventoryIndex inv]
    {
        get
        {
            var num = (int)inv;

            if (num < 0 || num >= InventoryCount)
                return null;

            return ReadObjectAt<InventoryWrapper>(num * 8);
        }
    }

    public List<InventoryWrapper> DebugInventories => _debug();

    private List<InventoryWrapper> _debug()
    {
        var list = new List<InventoryWrapper>();

        foreach (var inx in Enum.GetValues<InventoryIndex>())
        {
            var num = (int)inx;

            if (num < 0 || num >= InventoryCount)
                return null;

            list.Add(ReadObjectAt<PlayerInventoryWrapper>(num * 8));
        }

        return list;
    }
}

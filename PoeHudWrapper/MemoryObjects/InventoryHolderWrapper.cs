using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace PoeHudWrapper.MemoryObjects;

public class InventoryHolderWrapper : RemoteMemoryObject
{
    public ServerInventoryWrapper Inventory => ReadObject<ServerInventoryWrapper>(Address);

    public const int StructSize = InventoryHolder.StructSize;
    public int Id => M.Read<int>(Address + 0x10);
    public InventoryNameE TypeId => (InventoryNameE)Id;

    public override string ToString()
    {
        return $"InventoryType: {Inventory.InventType}, InventorySlot: {Inventory.InventSlot}, Items.Count: {Inventory.Items.Count} ItemCount: {Inventory.ItemCount}";
    }
}
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements.InventoryElements;

public class NormalInventoryItemWrapper : ElementWrapper
{
    private NormalInventoryItemOffsets ItemOffsets => M.Read<NormalInventoryItemOffsets>(Address);

    public virtual int ItemWidth => ItemOffsets.Width;
    public virtual int ItemHeight => ItemOffsets.Height;

    public Entity Item => GetObject<Entity>(ItemOffsets.Item);

    public ToolTipType toolTipType => ToolTipType.InventoryItem;
}
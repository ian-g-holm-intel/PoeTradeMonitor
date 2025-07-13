using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using System.Drawing;

namespace PoeHudWrapper.MemoryObjects;

public class ServerStashTabWrapper : RemoteMemoryObject
{
    private static readonly int ColorOffset = Extensions.GetOffset<ServerStashTabOffsets>(x => x.Color);

    public ServerStashTabOffsets ServerStashTabOffsets => M.Read<ServerStashTabOffsets>(Address);
    public string NameOld => NativeStringReader.ReadString(Address + 0x8, M) + (RemoveOnly ? " (Remove-only)" : string.Empty);
    public string Name => ServerStashTabOffsets.Name.ToString(M);

    //public int InventoryId => M.Read<int>(Address + 0x20);
    public uint Color => ServerStashTabOffsets.Color;

    public Color Color2 => System.Drawing.Color.FromArgb(
        M.Read<byte>(Address + ColorOffset),
        M.Read<byte>(Address + ColorOffset + 1),
        M.Read<byte>(Address + ColorOffset + 2));

    public InventoryTabPermissions MemberFlags => (InventoryTabPermissions)ServerStashTabOffsets.MemberFlags;
    public InventoryTabPermissions OfficerFlags => (InventoryTabPermissions)ServerStashTabOffsets.OfficerFlags;
    public InventoryTabType TabType => (InventoryTabType)ServerStashTabOffsets.TabType;
    public ushort VisibleIndex => ServerStashTabOffsets.DisplayIndex;

    //public ushort LinkedParentId => M.ReadUShort(Address + 0x26);
    public InventoryTabFlags Flags => (InventoryTabFlags)ServerStashTabOffsets.Flags;
    public bool RemoveOnly => (Flags & InventoryTabFlags.RemoveOnly) == InventoryTabFlags.RemoveOnly;
    public bool IsHidden => (Flags & InventoryTabFlags.Hidden) == InventoryTabFlags.Hidden;

    public override string ToString()
    {
        return $"{Name}, DisplayIndex: {VisibleIndex}, {TabType}";
    }
}
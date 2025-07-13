using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;
using System.Numerics;

namespace PoeHudWrapper.MemoryObjects;

public class ServerInventoryWrapper : RemoteMemoryObject
{
    private const int HashReadLimit = 1000;
    private ServerInventoryOffsets Struct => M.Read<ServerInventoryOffsets>(Address);
    public InventoryTypeE InventType => (InventoryTypeE)Struct.InventType;
    public InventorySlotE InventSlot => (InventorySlotE)Struct.InventSlot;
    public int Columns => Struct.Columns;
    public int Rows => Struct.Rows;
    public bool IsRequested => Struct.IsRequested == 1;
    public long ItemCount => Struct.ItemCount;
    public int ServerRequestCounter => Struct.ServerRequestCounter;
    public IList<InventSlotItemWrapper> InventorySlotItems => ReadHashMap(Struct.InventorySlotItemsPtr, HashReadLimit).Values.ToList();
    public List<InventSlotItemWrapper> ItemsByPosition
    {
        get
        {
            {
                var addressVector = M.ReadStdVector<long>(Struct.InventoryItems);
                var items = addressVector.Distinct().ToDictionary(x => x, x => x == 0 ? null : GetObject<InventSlotItemWrapper>(x));
                return addressVector.Select(items.GetValueOrDefault).ToList();
            }
        }
    }
    public long Hash => Struct.Hash;
    public IList<Entity> Items => InventorySlotItems.Select(x => x.Item).ToList();

    public InventSlotItemWrapper this[int x, int y]
    {
        get
        {
            var index = x + y * Columns;
            if (index < 0 || index >= ItemsByPosition.Count)
            {
                return null;
            }
            return ItemsByPosition[index];
        }
    }

    public Dictionary<int, InventSlotItemWrapper> ReadHashMap(long pointer, int limitMax)
    {
        var result = new Dictionary<int, InventSlotItemWrapper>();

        var stack = new Stack<HashNodeWrapper>();
        var startNode = GetObject<HashNodeWrapper>(pointer);
        var item = startNode.Root;
        stack.Push(item);

        while (stack.Count != 0)
        {
            var node = stack.Pop();

            if (!node.IsNull)
                result[node.Key] = node.Value1;

            var prev = node.Previous;

            if (!prev.IsNull)
                stack.Push(prev);

            var next = node.Next;

            if (!next.IsNull)
                stack.Push(next);

            if (limitMax-- < 0)
            {
                DebugWindow.LogError("Too many items in inventory, breaking");
                break;
            }
        }

        return result;
    }

    public class HashNodeWrapper : RemoteMemoryObject
    {
        private NativeHashNode NativeHashNode => M.Read<NativeHashNode>(Address);
        public HashNodeWrapper Previous => GetObject<HashNodeWrapper>(NativeHashNode.Previous);
        public HashNodeWrapper Root => GetObject<HashNodeWrapper>(NativeHashNode.Root);
        public HashNodeWrapper Next => GetObject<HashNodeWrapper>(NativeHashNode.Next);

        //public readonly byte Unknown;
        public bool IsNull => NativeHashNode.IsNull != 0;

        //private readonly byte byte_0;
        //private readonly byte byte_1;
        public int Key => NativeHashNode.Key;

        //public readonly int Useless;
        public InventSlotItemWrapper Value1 => GetObject<InventSlotItemWrapper>(NativeHashNode.Value1);

        //public readonly long Value2;
    }

    public class InventSlotItemWrapper : RemoteMemoryObject
    {
        [Obsolete]
        public Vector2 InventoryPosition => Location.InventoryPosition;

        public Vector2 InventoryPositionNum => Location.InventoryPositionNum;
        public ItemMinMaxLocation Location => M.Read<ItemMinMaxLocation>(Address + 0x08);
        public Entity Item => ReadObject<Entity>(Address);
        public int PosX => M.Read<int>(Address + 0x8);
        public int PosY => M.Read<int>(Address + 0xc);
        public int SizeX => M.Read<int>(Address + 0x10) - PosX;
        public int SizeY => M.Read<int>(Address + 0x14) - PosY;
        private SharpDX.RectangleF ClientRect => GetClientRect();

        public System.Drawing.Point Center
        {
            get
            {
                var center = GetClientRect().Center;
                return new System.Drawing.Point(Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
            }
        }

        public SharpDX.RectangleF GetClientRect()
        {
            var playerInventElement = GameWrapper.TheGame.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var inventClientRect = playerInventElement.GetClientRect();
            var cellSize = inventClientRect.Width / 12;
            return Location.GetItemRect(inventClientRect.X, inventClientRect.Y, cellSize);
        }

        public override string ToString()
        {
            return $"InventSlotItem: {Location}, Item: {Item}";
        }

        public struct ItemMinMaxLocation
        {
            private int XMin { get; set; }
            private int YMin { get; set; }
            private int XMax { get; set; }
            private int YMax { get; set; }

            public SharpDX.RectangleF GetItemRect(float invStartX, float invStartY, float cellsize)
            {
                return new SharpDX.RectangleF(
                    invStartX + cellsize * XMin,
                    invStartY + cellsize * YMin,
                    (XMax - XMin) * cellsize,
                    (YMax - YMin) * cellsize);
            }

            [Obsolete]
            public Vector2 InventoryPosition => new(XMin, YMin);

            public Vector2 InventoryPositionNum => new(XMin, YMin);

            public override string ToString()
            {
                return $"({XMin}, {YMin}, {XMax}, {YMax})";
            }
        }
    }
}
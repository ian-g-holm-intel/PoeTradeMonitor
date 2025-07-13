using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using PoeHudWrapper.MemoryObjects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TradeBotLib;

public static class ExtensionMethods
{
    public static Point GetInventoryLocation(this ServerInventoryWrapper.InventSlotItemWrapper item)
    {
        if (item?.Item == null || !item.Item.IsValid) return new Point(-1, -1);
        return new Point(Convert.ToInt32(item.InventoryPositionNum.X), Convert.ToInt32(item.InventoryPositionNum.Y));
    }

    public static int GetNumberLinks(this Entity item)
    {
        if (!item.IsValid) return 0;
        var sockets = item.GetComponent<Sockets>();
        return sockets == null ? 0 : sockets.LargestLinkSize;
    }

    public static int GetItemLevel(this Entity item)
    {
        if (!item.IsValid) return 0;
        var mods = item.GetComponent<Mods>();
        return mods == null ? 0 : mods.ItemLevel;
    }

    public static int GetNumberSockets(this Entity item)
    {
        if (!item.IsValid) return 0;
        var sockets = item.GetComponent<Sockets>();
        return sockets == null ? 0 : sockets.NumberOfSockets;
    }

    public static bool IsCorrupted(this Entity item)
    {
        if (!item.IsValid) return false;
        var baseType = item.GetComponent<Base>();
        return baseType.isCorrupted;
    }

    public static List<string> GetMods(this Entity item)
    {
        if (!item.IsValid) return new List<string>();
        var mods = item.GetComponent<Mods>();
        return mods.HumanStats;
    }
}

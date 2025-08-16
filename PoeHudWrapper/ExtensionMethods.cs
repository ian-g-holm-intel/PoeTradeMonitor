using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper;

public static class ExtensionMethods
{
    public static Point GetInventoryLocation(this ServerInventory.InventSlotItem item)
    {
        if (item?.Item == null || !item.Item.IsValid) return new Point(-1, -1);
        return new Point(Convert.ToInt32(item.InventoryPosition.X), Convert.ToInt32(item.InventoryPosition.Y));
    }

    public static Point GetCenter(this ServerInventory.InventSlotItem item)
    {
        var center = item.GetClientRect().Center;
        return new Point(Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
    }

    public static int GetItemLevel(this Entity item)
    {
        if (!item.IsValid) return 0;
        var baseComponent = item.GetComponent<Base>();
        var skillGemComponent = item.GetComponent<SkillGem>();
        var modsComponent = item.GetComponent<Mods>();
        return modsComponent == null ? (skillGemComponent == null ? baseComponent.CurrencyItemLevel : skillGemComponent.Level) : modsComponent.ItemLevel;
    }

    public static int GetNumberSockets(this Entity item)
    {
        if (!item.IsValid) return 0;
        var sockets = item.GetComponent<Sockets>();
        return sockets == null ? 0 : sockets.NumberOfSockets;
    }

    public static int GetQuality(this Entity item)
    {
        if (!item.IsValid) return 0;
        var qualityComponent = item.GetComponent<Quality>();
        return qualityComponent == null ? 0 : qualityComponent.ItemQuality;
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

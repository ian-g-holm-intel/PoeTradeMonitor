using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class DivinationInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        var rect = Parent?.GetClientRect() ?? RectangleF.Empty;
        if (rect == RectangleF.Empty) return rect;

        // div stash tab scrollbar element scroll value calculator
        var addr = Parent?.Parent?.Parent?.Parent?[2].Address;
        if (addr == null) return rect;
        var sub = M.Read<int>(addr.Value + 0xA64) * (float)107.5;
        rect.Y -= sub;

        return rect;
    }
}

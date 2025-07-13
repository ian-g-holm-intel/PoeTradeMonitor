using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class EssenceInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

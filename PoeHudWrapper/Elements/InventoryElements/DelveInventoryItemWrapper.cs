using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class DelveInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

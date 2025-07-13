using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class BlightInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class DeliriumInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

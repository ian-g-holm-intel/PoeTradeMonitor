using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class GemInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

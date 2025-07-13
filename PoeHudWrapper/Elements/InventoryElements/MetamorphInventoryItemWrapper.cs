using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class MetamorphInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

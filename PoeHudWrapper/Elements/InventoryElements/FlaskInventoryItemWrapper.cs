using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class FlaskInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

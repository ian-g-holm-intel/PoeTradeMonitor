using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class FragmentInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

using SharpDX;

namespace PoeHudWrapper.Elements.InventoryElements;

public class CurrencyInventoryItemWrapper : NormalInventoryItemWrapper
{
    public override RectangleF GetClientRect()
    {
        return Parent?.GetClientRect() ?? RectangleF.Empty;
    }
}

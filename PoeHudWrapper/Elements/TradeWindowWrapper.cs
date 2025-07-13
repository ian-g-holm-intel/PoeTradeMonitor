using PoeHudWrapper.Elements.InventoryElements;
using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class TradeWindowWrapper : ElementWrapper
{
    public ElementWrapper SellDialog => GetChildFromIndices(3, 1, 0, 0);
    public ElementWrapper YourOfferElement => SellDialog?.GetChildAtIndex(0);
    public List<NormalInventoryItemWrapper> YourOffer => ExtractNormalInventoryItems(YourOfferElement?.Children);
    public ElementWrapper OtherOfferElement => SellDialog?.GetChildAtIndex(1);
    public List<NormalInventoryItemWrapper> OtherOffer => ExtractNormalInventoryItems(OtherOfferElement?.Children);
    public string SellerName => GetChildFromIndices(3, 0, 1, 0, 1)?.Text ?? string.Empty;
    public ElementWrapper AcceptButton => SellDialog?.GetChildAtIndex(6);
    public Point AcceptButtonLocation => AcceptButton?.Center ?? new Point(-1, -1);
    public bool AcceptButtonClickable => !AcceptButton?.IsSelected ?? false;
    public bool AcceptButtonClicked => AcceptButton?.GetChildAtIndex(0).Text == "cancel accept";
    public ElementWrapper CancelButton => SellDialog?.GetChildAtIndex(6);
    public Point CancelButtonLocation => CancelButton?.Center ?? new Point(-1, -1);

    private List<NormalInventoryItemWrapper> ExtractNormalInventoryItems(IList<ElementWrapper> children)
    {
        var resultList = new List<NormalInventoryItemWrapper>();

        if (children == null) 
            return resultList;

        for (var i = 1; i < children.Count; i++)
        {
            resultList.Add(children[i].AsObject<NormalInventoryItemWrapper>());
        }
        return resultList;
    }
}

using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class PoeChatElementWrapper : ElementWrapper
{
    public long TotalMessageCount => ChildCount;

    public EntityLabelWrapper this[int index]
    {
        get
        {
            if (index < TotalMessageCount)
                return GetChildAtIndex(index).AsObject<EntityLabelWrapper>();

            return null;
        }
    }

    //.GetChildrenAs doesn't have the same restriction on element count .Children does
    public List<ElementWrapper> MessageElements => GetChildrenAs<ElementWrapper>();
    public List<string> Messages => MessageElements.Select(x => x.Text).ToList();
}

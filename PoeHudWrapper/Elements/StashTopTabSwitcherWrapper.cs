using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class StashTopTabSwitcherWrapper : ElementWrapper
{
    public List<ElementWrapper> SwitchButtons => Children.Where(x => x.IsVisible && x.ChildCount > 0).ToList();
}

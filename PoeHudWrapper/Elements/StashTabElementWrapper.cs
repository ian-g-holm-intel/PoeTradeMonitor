using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class StashTabElementWrapper : ElementWrapper
{

    public ElementWrapper StashTab => Address != 0 ? ReadObjectAt<ElementWrapper>(0x1c0) : null;

    public string TabName => StashTab?.Text ?? string.Empty;
}

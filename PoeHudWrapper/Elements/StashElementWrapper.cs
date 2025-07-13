using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class StashElementWrapper : ElementWrapper
{
    private StashElementOffsets StashElementOffsets => M.Read<StashElementOffsets>(Address);

    public long TotalStashes => StashInventoryPanel?.ChildCount ?? 0;
    public ElementWrapper ExitButton => Address != 0 ? GetObject<ElementWrapper>(StashElementOffsets.ExitButtonPtr) : null;

    // Nice struct starts at 0xB80 till 0xBD0 and all are 8 byte long pointers.
    public StashTabContainerWrapper StashTabContainer => M.Read<long>(StashElementOffsets.StashTabContainerPtr1 + StashElementOffsets.StashTabContainerOffset2) switch
    {
        0 => GetChildFromIndices(2, 0, 0, 1)?.AsObject<StashTabContainerWrapper>(),
        var x => GetObject<StashTabContainerWrapper>(x)
    };

    public ElementWrapper StashTitlePanel => Address != 0 ? GetObject<ElementWrapper>(StashElementOffsets.StashTitlePanelPtr) : null;

    public ElementWrapper StashInventoryPanel => Address != 0 ? StashTabContainer?.StashInventoryPanel : null;

    public ElementWrapper ViewAllStashButton => Address != 0 ? StashTabContainer?.ViewAllStashesButton : null;

    public ElementWrapper ViewAllStashPanel => Address != 0 ? StashTabContainer?.ViewAllStashPanel : null;
    public ElementWrapper PinStashTabListButton => Address != 0 ? StashTabContainer?.PinStashTabListButton : null;
    public int IndexVisibleStash => StashTabContainer?.VisibleStashIndex ?? 0;
    public InventoryWrapper VisibleStash => IsVisible ? StashTabContainer?.VisibleStash : null;

    [Obsolete("Just use Inventories")]
    public IList<string> AllStashNames => StashTabContainer?.AllStashNames ?? [];

    [Obsolete("Just use Inventories")]
    public IList<InventoryWrapper> AllInventories => StashTabContainer?.AllInventories ?? [];

    public List<StashTabContainerInventoryWrapper> Inventories => StashTabContainer?.Inventories ?? [];

    public IList<ElementWrapper> TabListButtons => StashTabContainer?.TabListButtons;

    public IList<ElementWrapper> ViewAllStashPanelChildren => StashTabContainer?.ViewAllStashPanelChildren;

    public InventoryWrapper GetStashInventoryByIndex(int index) => StashTabContainer?.GetStashInventoryByIndex(index);

    public IList<ElementWrapper> GetTabListButtons() => TabListButtons;
}

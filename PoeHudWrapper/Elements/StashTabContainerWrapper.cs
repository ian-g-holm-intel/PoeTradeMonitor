using ExileCore.PoEMemory;
using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class VendorStashTabContainerWrapper : StashTabContainerWrapper
{
    public override List<StashTabContainerInventoryWrapper> Inventories => base.Inventories.Select(StashTabContainerInventoryWrapper (x) => x?.AsObject<VendorStashTabContainerInventoryWrapper>()).ToList();
}

public class StashTabContainerInventoryWrapper : StructuredRemoteMemoryObject<StashTabContainerInventoryOffsets>
{
    public virtual InventoryWrapper Inventory => GetObject<ElementWrapper>(Structure.InventoryPtr)[0]?.AsObject<InventoryWrapper>();
    public ElementWrapper TabButton => GetObject<ElementWrapper>(Structure.StashButtonPtr);

    public string TabName => TabButton?[0]?.Children
        ?.LastOrDefault(x => x.IsVisibleLocal && !string.IsNullOrEmpty(x.Text))?.Text ?? "";

    public override string ToString()
    {
        return Inventory is { } inventory ? $"InventoryWrapper: {(inventory.IsVisible ? "(selected)" : "")} {inventory}, '{TabName}'" : $"No inventory, '{TabName}'";
    }
}

public class VendorStashTabContainerInventoryWrapper : StashTabContainerInventoryWrapper
{
    public override VendorInventoryWrapper Inventory => base.Inventory.AsObject<VendorInventoryWrapper>();
}

public class StashTabContainerWrapper : ElementWrapper
{
    public StashTabContainerOffsets StashTabContainerOffsets => M.Read<StashTabContainerOffsets>(Address);
    public int VisibleStashIndex => StashTabContainerOffsets.VisibleStashIndex;
    public ElementWrapper StashInventoryPanel => Address != 0 ? this[1] : null;
    public ElementWrapper ViewAllStashPanel => Address != 0 ? this[4] : null;
    public long TotalStashes => Inventories?.Count ?? 0;

    public ElementWrapper ViewAllStashesButton => GetObject<ElementWrapper>(StashTabContainerOffsets.ViewAllStashesButtonPtr);
    public ElementWrapper PinStashTabListButton => GetObject<ElementWrapper>(StashTabContainerOffsets.PinStashTabListButtonPtr);
    public StashTopTabSwitcherWrapper TabSwitchBar => GetObject<StashTopTabSwitcherWrapper>(StashTabContainerOffsets.TabSwitchBarPtr);

    public InventoryWrapper VisibleStash => GetStashInventoryByIndex(VisibleStashIndex) switch
    {
        { IsVisible: true } visible => visible,
        //if user dragged the tabs around, we may have the wrong inventory, perform a linear search for the visible one
        var some => Inventories.FirstOrDefault(x => x?.Inventory?.IsVisible == true)?.Inventory ?? some
    };

    [Obsolete("Just use Inventories")]
    public IList<InventoryWrapper> AllInventories => Inventories.Select(x => x?.Inventory).ToList();

    [Obsolete("Just use Inventories")]
    public IList<string> AllStashNames => Inventories.Select(x => x?.TabName).ToList();

    public IList<ElementWrapper> ViewAllStashPanelChildren => ViewAllStashPanel.Children.LastOrDefault(x => x.ChildCount == TotalStashes)?.Children.Where(x => x.ChildCount > 0).ToList() ?? [];
    public IList<ElementWrapper> TabListButtons => ViewAllStashPanelChildren?.Take((int)TotalStashes).ToList() ?? [];

    public virtual List<StashTabContainerInventoryWrapper> Inventories => M.ReadRMOStdVector<StashTabContainerInventoryWrapper>(StashTabContainerOffsets.Stashes, StashTabContainerInventoryWrapper.StructureSize).ToList();

    [Obsolete("Just use Inventories")]
    public string GetStashName(int index) => Inventories.ElementAtOrDefault(index)?.TabName;

    public InventoryWrapper GetStashInventoryByIndex(int index) => Inventories.ElementAtOrDefault(index)?.Inventory;
}

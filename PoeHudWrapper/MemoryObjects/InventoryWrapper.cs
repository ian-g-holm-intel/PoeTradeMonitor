using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using GameOffsets;
using PoeHudWrapper.Elements;
using PoeHudWrapper.Elements.InventoryElements;

namespace PoeHudWrapper.MemoryObjects;

public class VendorInventoryWrapper : InventoryWrapper
{
    protected override InventoryType GetInvType() => InventoryType.VendorInventory;
    protected override ElementWrapper OffsetContainerElement => this;
}

public class PlayerInventoryWrapper : InventoryWrapper
{
    protected override InventoryType GetInvType() => InventoryType.PlayerInventory;
    protected override ElementWrapper OffsetContainerElement => this;
}

public class InventoryWrapper : ElementWrapper
{
    private InventoryType _cacheInventoryType;

    protected virtual ElementWrapper OffsetContainerElement => GetChildAtIndex(0);

    private InventoryOffsets InventoryStruct => NestedVisibleInventory?.InventoryStruct ?? M.Read<InventoryOffsets>(OffsetContainerElement?.Address ?? 0);
    public ServerInventoryWrapper ServerInventory=> GetServerInventory();
    public long ItemCount => InventoryStruct.ItemCount;
    public HoverItemState ItemHoverState => (HoverItemState)InventoryStruct.ItemHoverState;
    public long TotalBoxesInInventoryRow => InventoryStruct.InventorySize.X;
    public NormalInventoryItemWrapper HoverItem => InventoryStruct.HoverItem == 0 ? null : GetObject<NormalInventoryItemWrapper>(InventoryStruct.HoverItem);
    public new int X => InventoryStruct.RealPos.X;
    public new int Y => InventoryStruct.RealPos.Y;
    public int XFake => InventoryStruct.FakePos.X;
    public int YFake => InventoryStruct.FakePos.Y;
    public bool CursorHoverInventory => InventoryStruct.CursorInInventory == 1;
    public bool IsNestedInventory => ChildCount == 2 && this[1].ChildCount == 5;
    public InventoryType InvType => GetInvType();
    public ElementWrapper InventoryUIElement => IsNestedInventory ? NestedVisibleInventory?.InventoryUIElement : getInventoryElement();
    private InventoryWrapper NestedVisibleInventory => IsNestedInventory ? GetNestedVisibleInventory() : null;
    private StashTabContainerWrapper NestedStashContainer => this[1]?.AsObject<StashTabContainerWrapper>();
    public int? NestedVisibleInventoryIndex => IsNestedInventory ? NestedStashContainer?.VisibleStashIndex : null;
    public StashTopTabSwitcherWrapper NestedTabSwitchBar => IsNestedInventory ? NestedStashContainer?.TabSwitchBar : null;

    // Shows Item details of visible inventory/stashes
    public IList<NormalInventoryItemWrapper> VisibleInventoryItems
    {
        get
        {
            if (IsNestedInventory)
            {
                return NestedVisibleInventory?.VisibleInventoryItems ?? new List<NormalInventoryItemWrapper>();
            }

            var InvRoot = InventoryUIElement;

            if (InvRoot == null || InvRoot.Address == 0x00)
                return null;
            /*else if (!InvRoot.IsVisible)
                return null;*/

            var list = new List<NormalInventoryItemWrapper>();

            switch (InvType)
            {
                case InventoryType.PlayerInventory:
                case InventoryType.NormalStash:
                case InventoryType.QuadStash:
                case InventoryType.VendorInventory:
                    //first child is not an item, but the metadata container instead
                    list.AddRange(InvRoot.GetChildrenAs<NormalInventoryItemWrapper>().Skip(1));

                    break;

                //For 3.3 child count is 3, not 2 as earlier, so we using the second one
                //For 3.17 Currency tab was split into two sub tabs and require visible checks.
                case InventoryType.CurrencyStash:
                    ElementWrapper visibleSubTab = null;

                    if (Children[1].IsVisible)
                        visibleSubTab = Children[1];
                    else if (Children[2].IsVisible)
                        visibleSubTab = Children[2];

                    if (visibleSubTab != null)
                        foreach (var item in visibleSubTab.Children)
                            if (item.ChildCount > 1)
                                list.Add(item[1].AsObject<EssenceInventoryItemWrapper>());

                    foreach (var item in InvRoot.Children)
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<EssenceInventoryItemWrapper>());

                    break;
                case InventoryType.EssenceStash:
                    foreach (var item in InvRoot.Children)
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<EssenceInventoryItemWrapper>());
                    break;
                case InventoryType.FragmentStash:
                    ElementWrapper visibleSubTabFragment = null;

                    if (Children[0].IsVisible)
                        visibleSubTabFragment = Children[0];
                    else if (Children[1].IsVisible)
                        list.AddRange(Children[1][0].Children
                            .SelectMany(x => x[0].Children.Skip(1))
                            .Select(x => x.AsObject<FragmentInventoryItemWrapper>()));
                    else if (Children[2].IsVisible)
                        visibleSubTabFragment = Children[2][0];
                    else if (Children[3].IsVisible)
                        visibleSubTabFragment = Children[3][0];

                    if (visibleSubTabFragment != null)
                        list.AddRange(visibleSubTabFragment.Children
                            .SelectMany(x => x.Children.Skip(1))
                            .Select(x => x.AsObject<NormalInventoryItemWrapper>()));
                    break;
                case InventoryType.DivinationStash:
                    return InvRoot.GetChildAtIndex(0)?.GetChildAtIndex(1)?.Children
                        .Where(x => x is { IsVisibleLocal: true, ChildCount: > 1 } && x.GetChildAtIndex(1).ChildCount > 1)
                        .Select<ElementWrapper, NormalInventoryItemWrapper>(x => x.GetChildAtIndex(1)?.GetChildAtIndex(1)?.AsObject<DivinationInventoryItemWrapper>())
                        .Where(x => x != null)
                        .ToList() ?? list;
                case InventoryType.MapStash:
                    if (ChildCount <= 3) break;
                    foreach (var subInventory in this[3].Children.Where(x => x.IsVisible))
                    {
                        foreach (var item in subInventory.GetChildrenAs<NormalInventoryItemWrapper>().Skip(1))
                        {
                            list.Add(item);
                        }
                    }
                    // Items aren't sorted in memory, so we sort them by position
                    list.Sort((i1, i2) =>
                        (i1.PositionNum.X * 6 + i1.PositionNum.Y).CompareTo(i2.PositionNum.X * 6 + i2.PositionNum.Y));
                    break;
                case InventoryType.DelveStash:
                    foreach (var item in InvRoot.Children)
                    {
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<DelveInventoryItemWrapper>());
                    }

                    break;
                case InventoryType.BlightStash:
                    foreach (var item in InvRoot.Children.Skip(1).SkipLast(1))
                    {
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<BlightInventoryItemWrapper>());
                    }

                    break;
                case InventoryType.DeliriumStash:
                    foreach (var item in InvRoot.Children)
                    {
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<DeliriumInventoryItemWrapper>());
                    }

                    break;
                case InventoryType.UltimatumStash:
                    foreach (var item in InvRoot.Children)
                    {
                        if (item.ChildCount > 2)
                            list.AddRange(item.Children.Where(itemChild => itemChild.ChildCount == 2).Select(itemChild => itemChild[1].AsObject<NormalInventoryItemWrapper>()));
                        else if (item.ChildCount == 2) list.Add(item[1].AsObject<NormalInventoryItemWrapper>());
                    }

                    break;
                case InventoryType.UniqueStash:
                    foreach (var item in InvRoot.Children)
                    {
                        if (item.ChildCount > 1)
                            list.Add(item[1].AsObject<NormalInventoryItemWrapper>());
                    }

                    break;
                case InventoryType.GemStash:
                    return InvRoot.Children
                        .Where(subInventory => subInventory.IsVisibleLocal)
                        .SelectMany(x => x[1]?.Children ?? Enumerable.Empty<ElementWrapper>())
                        .Select(slot => slot[1]?.AsObject<GemInventoryItemWrapper>() as NormalInventoryItemWrapper)
                        .Where(x => x != null)
                        .ToList();
                case InventoryType.FlaskStash:
                    return InvRoot.Children
                        .Where(subInventory => subInventory.IsVisibleLocal)
                        .SelectMany(x => x[1]?.Children ?? Enumerable.Empty<ElementWrapper>())
                        .Select(slot => slot[1]?.AsObject<FlaskInventoryItemWrapper>() as NormalInventoryItemWrapper)
                        .Where(x => x != null)
                        .ToList();
            }

            return list;
        }
    }

    // Works even if inventory is currently not in view.
    // As long as game have fetched inventory data from Server.
    // Will return the item based on x,y format.
    // Give more controll to user what to do with
    // dublicate items (items taking more than 1 slot)
    // or slots where items doesn't exists (return null).
    public Entity this[int x, int y, int xLength]
    {
        get
        {
            var invAddr = M.Read<long>(Address + 0x410, 0x640, 0x38);
            y = y * xLength;
            var itmAddr = M.Read<long>(invAddr + (x + y) * 8);

            if (itmAddr <= 0)
                return null;

            return ReadObject<Entity>(itmAddr);
        }
    }

    protected virtual InventoryType GetInvType()
    {
        if (IsNestedInventory) return NestedVisibleInventory?.InvType ?? InventoryType.InvalidInventory;
        if (_cacheInventoryType != InventoryType.InvalidInventory) return _cacheInventoryType;

        if (Address == 0) return InventoryType.InvalidInventory;

        // For Poe MemoryLeak bug where ChildCount of PlayerInventory keep
        // Increasing on Area/Map Change. Ref:
        // http://www.ownedcore.com/forums/mmo/path-of-exile/poe-bots-programs/511580-poehud-overlay-updated-362.html#post3718876
        // Orriginal Value of ChildCount should be 0x18
        for (var j = 1; j < InventoryList.InventoryCount; j++)
        {
            if (GameWrapper.TheGame.IngameState.IngameUi.InventoryPanel[(InventoryIndex)j].Address == Address)
            {
                _cacheInventoryType = InventoryType.PlayerInventory;
                return _cacheInventoryType;
            }
        }


        switch (ChildCount)
        {
            case 111:
                _cacheInventoryType = InventoryType.EssenceStash;
                break;
            case 18:
                _cacheInventoryType = InventoryType.CurrencyStash;
                break;
            case 5:
                if (Children[4].ChildCount == 4)
                    _cacheInventoryType = InventoryType.FragmentStash;
                else
                    _cacheInventoryType = InventoryType.DivinationStash;
                break;
            case 7:
                _cacheInventoryType = InventoryType.MapStash;
                break;
            case 1:
                // Normal Stash and Quad Stash is same.
                if (TotalBoxesInInventoryRow == 24)
                    _cacheInventoryType = InventoryType.QuadStash;
                else
                    _cacheInventoryType = InventoryType.NormalStash;
                break;
            case 35:
                _cacheInventoryType = InventoryType.DelveStash;
                break;
            case 84:
                _cacheInventoryType = InventoryType.BlightStash;
                break;
            case 88:
                _cacheInventoryType = InventoryType.DeliriumStash;
                break;
            case 13:
                _cacheInventoryType = InventoryType.UltimatumStash;
                break;
            case 9:
                _cacheInventoryType = InventoryType.UniqueStash;
                break;
            case 4:
                _cacheInventoryType = this[0]?.ChildCount switch
                {
                    4 => InventoryType.GemStash,
                    5 => InventoryType.FlaskStash,
                    _ => InventoryType.InvalidInventory
                };
                break;
            default:
                _cacheInventoryType = InventoryType.InvalidInventory;
                break;
        }

        return _cacheInventoryType;
    }

    private ElementWrapper getInventoryElement()
    {
        switch (InvType)
        {
            case InventoryType.PlayerInventory:
            case InventoryType.CurrencyStash:
            case InventoryType.EssenceStash:
            case InventoryType.FragmentStash:
            case InventoryType.BlightStash:
            case InventoryType.DelveStash:
            case InventoryType.DeliriumStash:
            case InventoryType.UltimatumStash:
            case InventoryType.DivinationStash:
            case InventoryType.VendorInventory:
                return this;
            case InventoryType.NormalStash:
            case InventoryType.QuadStash:
                return GetChildAtIndex(0); //return AsObject<ElementWrapper>();
            case InventoryType.GemStash:
            case InventoryType.FlaskStash:
                return GetChildFromIndices(1, 0, 1); //return AsObject<ElementWrapper>();
            case InventoryType.MapStash:
                return AsObject<ElementWrapper>().Parent?.AsObject<MapStashTabElementWrapper>();
            case InventoryType.UniqueStash:
                return AsObject<ElementWrapper>().Parent;
            default:
                return null;
        }
    }

    private ServerInventoryWrapper GetServerInventory()
    {
        switch (InvType)
        {
            case InventoryType.PlayerInventory:
            case InventoryType.VendorInventory:
            case InventoryType.NormalStash:
            case InventoryType.QuadStash:
                return InventoryUIElement?.ReadObjectAt<ServerInventoryWrapper>(InventoryOffsets.DefaultServerInventoryOffset);
            case InventoryType.DivinationStash:
                return InventoryUIElement?.ReadObjectAt<ServerInventoryWrapper>(InventoryOffsets.DivinationServerInventoryOffset);
            case InventoryType.BlightStash:
                return InventoryUIElement?.ReadObjectAt<ElementWrapper>(InventoryOffsets.BlightServerInventoryOffset)
                    .ReadObjectAt<ServerInventoryWrapper>(InventoryOffsets.DefaultServerInventoryOffset);
            case InventoryType.CurrencyStash:
            case InventoryType.EssenceStash:
            case InventoryType.UltimatumStash:
                return InventoryUIElement?.ReadObjectAt<ElementWrapper>(InventoryOffsets.ComplexStashFirstLevelServerInventoryOffset)
                    .ReadObjectAt<ServerInventoryWrapper>(InventoryOffsets.DefaultServerInventoryOffset);
            default:
                return null;
        }
    }

    private InventoryWrapper GetNestedVisibleInventory()
    {
        return NestedStashContainer?.VisibleStash;
    }
}
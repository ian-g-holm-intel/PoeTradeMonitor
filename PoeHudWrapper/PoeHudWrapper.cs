using PoeLib;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using PoeHudWrapper.MemoryObjects;
using PoeHudWrapper.Elements;
using PoeHudWrapper.Elements.InventoryElements;
using System.Windows.Forms;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore;
using System.Linq;

namespace PoeHudWrapper;

public interface IPoeHudWrapper
{
    void Initialize();
    bool AcceptButtonClickable { get; }
    bool AcceptButtonClicked { get; }
    Point AcceptTradeButtonLocation { get; }
    string AreaName { get; }
    string[] CharactersInArea { get; }
    string ChatText { get; }
    bool ChatVisible { get; }
    Rectangle ClientBounds { get; }
    Entity CraftingSlotItem { get; }
    Point CraftingSlotLocation { get; }
    Point EssenceCraftingSlotLocation { get; }
    ModValueWrapper[] CraftingSlotMods { get; }
    ModValueWrapper[] EssenceCraftingSlotMods { get; }
    ItemRarity CraftingSlotRarity { get; }
    Point DestroyConfirmationKeepButtonLocation { get; }
    bool DestroyConfirmationVisible { get; }
    bool InGame { get; }
    bool InventoryPanelOpen { get; }
    bool IsLoading { get; }
    bool ItemOnCursor { get; }
    int Latency { get; }
    Point LeavePartyButtonLocation { get; }
    InviteWrapper[] PartyInvites { get; }
    string[] PartyMemberNames { get; }
    PartyMemberWrapper[] PartyMembers { get; }
    Point PartyTabLocation { get; }
    string[] PendingInvites { get; }
    List<CurrencyStack> PlayerInventoryCurrency { get; }
    List<ServerInventoryWrapper.InventSlotItemWrapper> PlayerInventoryItems { get; }
    NormalInventoryItemWrapper GetItemElementFromEntity(Entity entity);
    string[] GetTooltipLines();
    bool IsItemHovered { get; }
    bool SocialPanelOpen { get; }
    List<CurrencyStack> StashCurrencies { get; }
    List<CurrencyStack> StashEssences { get; }
    Point StashLocation { get; }
    bool StashOpen { get; }
    Currency[] TheirTradeCurrency { get; }
    List<NormalInventoryItemWrapper> TheirTradeItems { get; }
    InviteWrapper[] TradeInvites { get; }
    bool TradeWindowOpen { get; }
    string TradeWindowSellerName { get; }
    bool TradeWindowVisible { get; }
    bool PartyTabVisible { get; }
    string GetBaseType(Entity item);
    string GetClassName(Entity item);
    Point FreeItemSlot { get; }
    string GetItemName(Entity item);
    ModValueWrapper[] GetModValues(Entity item);
    Point GetPartyInviteAcceptLocation(string characterName);
    Point GetPartyInviteDeclineLocation(string characterName);
    string GetPartyMemberZone(string characterName);
    Point PartyMemberPortalButtonLocation(string characterName);
    IEnumerable<CurrencyStack> GetStashCurrencyOfType(CurrencyType currencyType);
    int GetStashIndex(string stashName);
    bool IsInventorySelected(Point point);
    bool IsItemBeast(Entity entity);
    bool IsItemCurrency(Entity entity);
    bool IsItemEssence(Entity entity);
    bool IsItemFossil(Entity entity);
    bool IsItemFragment(Entity entity);
    bool IsItemMap(Entity entity);
    bool IsItemResonator(Entity entity);
    bool IsItemUniqueRing(Entity entity);
    bool PlayerInParty(string name = "");
    bool PlayerInZone(string characterName);
    Task<bool> SwitchToTab(string stashName);
    Point GetLocationFromInventorySlot(Point point);
    bool MenuOpen { get; }
    bool IsAtCharacterSelect { get; }
    bool IsAtLogin { get; }
    bool TradeInviteSentVisible { get; }
    IGameWrapper TheGame { get; }
}

public class PoeHudWrapper : IPoeHudWrapper
{
    private readonly IGameWrapper gameWrapper;
    private readonly ILogger<PoeHudWrapper> logger;
    private bool initialized = false;

    public PoeHudWrapper(IGameWrapper gameWrapper, ILogger<PoeHudWrapper> logger)
    {
        this.gameWrapper = gameWrapper;
        this.logger = logger;
    }

    public void Initialize()
    {
        if (!initialized)
        {
            gameWrapper.Initialize();
            initialized = true;
        }
    }

    public int Latency => gameWrapper.IngameState.ServerData.Latency;
    public Rectangle ClientBounds => gameWrapper.ClientBounds;
    public bool InGame => gameWrapper.InGame;
    public bool IsLoading => gameWrapper.IsLoading;
    public string AreaName => gameWrapper.IngameState.Data.CurrentArea.Name;
    public bool StashOpen => StashElement.IsVisible;
    public bool TradeWindowOpen => TradeElement.IsVisible;
    public bool InventoryPanelOpen => InventoryElement.IsVisible;
    public bool SocialPanelOpen => SocialElement.IsVisible;
    public bool PartyTabVisible => SocialElement.PartyTabVisible;
    public bool ItemOnCursor => gameWrapper.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Cursor)?.ItemCount > 0;
    public bool DestroyConfirmationVisible => DestroyConfirmationElement.IsVisible;
    public bool ChatVisible => ChatPanel.ChatInputElement.IsVisible;
    public string ChatText => ChatPanel.InputText;
    public bool MenuOpen => gameWrapper.IsMenuOpen;
    public bool IsAtCharacterSelect => gameWrapper.IsSelectCharacterState;
    public bool IsAtLogin => gameWrapper.IsLoginState;
    public IGameWrapper TheGame => gameWrapper;

    #region Social
    public Point LeavePartyButtonLocation => SocialElement.LeavePartyButtonLocation;
    public string[] PendingInvites => SocialElement.PendingInvites;
    public Point GetPartyInviteAcceptLocation(string characterName) => SocialElement.GetPartyInviteAcceptLocation(characterName);
    public Point GetPartyInviteDeclineLocation(string characterName) => SocialElement.GetPartyInviteDeclineLocation(characterName);
    #endregion

    #region Party
    public string[] PartyMemberNames => PartyElement.PartyMemberNames;
    public PartyMemberWrapper[] PartyMembers => PartyElement.PartyMembers;
    public string GetPartyMemberZone(string characterName) => PartyElement.PartyMemberZone(characterName);
    public Point PartyMemberPortalButtonLocation(string characterName) => PartyElement.PartyMemberPortalButtonLocation(characterName);
    public bool PlayerInZone(string characterName) => CharactersInArea.Contains(characterName);

    public string[] CharactersInArea
    {
        get
        {
            var players = gameWrapper.IngameState.Data.EntityList.Entities.Where(entity => entity.Type == EntityType.Player);
            if (players == null || !players.Any())
                return new string[0];

            var playerNames = players.Select(p => p.GetComponent<Player>()?.PlayerName).Where(name => !string.IsNullOrEmpty(name)).ToArray();
            return playerNames;
        }
    }

    public bool PlayerInParty(string name = "")
    {
        if (!PartyMemberNames.Any())
            return false;
        else if (string.IsNullOrEmpty(name))
            return true;
        else
            return PartyMemberNames.Any(partyMember => partyMember == name);
    }
    #endregion

    #region TradeWindow
    public bool TradeWindowVisible => TradeElement.IsVisible;
    public string TradeWindowSellerName => TradeElement.SellerName;
    public Point AcceptTradeButtonLocation => TradeElement.AcceptButtonLocation;
    public bool AcceptButtonClickable => TradeElement.AcceptButtonClickable;
    public bool AcceptButtonClicked => TradeElement.AcceptButtonClicked;
    public bool TradeInviteSentVisible => PopUpWindowElement.IsVisible;
    #endregion

    #region PlayerInvites
    public InviteWrapper[] PartyInvites => InviteElement.GetPlayerInvites("sent you a party invite");
    public InviteWrapper[] TradeInvites => InviteElement.GetPlayerInvites("sent you a trade request");
    #endregion

    #region Locations
    public Point DestroyConfirmationKeepButtonLocation => DestroyConfirmationElement.KeepButtonLocation;
    public Point PartyTabLocation => SocialElement.PartyTabLocation;
    public Point CraftingSlotLocation => StashElement.VisibleStash?.GetChildAtIndex(3)?.Center ?? new Point(-1, -1);
    public Point EssenceCraftingSlotLocation => StashElement.VisibleStash?.GetChildAtIndex(108)?.Center ?? new Point(-1, -1);

    public Point StashLocation
    {
        get
        {
            var entities = gameWrapper.IngameState.Data.EntityList.Entities;
            var stashEntity = entities.SingleOrDefault(entity => entity.Type == EntityType.Stash);
            if (stashEntity == null)
                return new Point(-1, -1);

            return GetEntityLocation(stashEntity);
        }
    }
    #endregion

    #region Inventories
    public List<CurrencyStack> StashCurrencies
    {
        get
        {
            if (!StashOpen)
                return new List<CurrencyStack>();

            using var ctSource = new CancellationTokenSource(5000);
            while (!ctSource.IsCancellationRequested)
            {
                try
                {
                    return TryGetStashCurrency(false);
                }
                catch (MemoryReadException) { }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error getting stash currency");
                    break;
                }
            }
            return TryGetStashCurrency(true);
        }
    }

    public List<CurrencyStack> StashEssences
    {
        get
        {
            if (!StashOpen)
                return new List<CurrencyStack>();

            using var ctSource = new CancellationTokenSource(5000);
            while (!ctSource.IsCancellationRequested)
            {
                try
                {
                    return TryGetStashEssences(false);
                }
                catch (MemoryReadException) { }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error getting stash essences");
                    break;
                }
            }
            return TryGetStashCurrency(true);
        }
    }

    private IEnumerable<ElementWrapper> StashCurrencyElements
    {
        get
        {
            if (StashElement?.VisibleStash == null || StashElement?.VisibleStash.ChildCount == 0)
                return Enumerable.Empty<ElementWrapper>();

            return StashElement?.VisibleStash?.GetChildAtIndex(1).Children.Where(slot => slot.ChildCount == 2) // GeneralCurrency
                .Concat(StashElement?.VisibleStash?.Children.Skip(4).Where(slot => slot.ChildCount == 2)); // MiscCurrency
        }
    }

    private IEnumerable<ElementWrapper> StashEssenceElements
    {
        get
        {
            if (StashElement?.VisibleStash == null || StashElement?.VisibleStash.ChildCount == 0)
                return Enumerable.Empty<ElementWrapper>();

            return StashElement?.VisibleStash?.VisibleInventoryItems;
        }
    }

    private List<CurrencyStack> TryGetStashCurrency(bool force)
    {
        var currency = new List<CurrencyStack>();
        var currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        foreach (var element in StashCurrencyElements)
        {
            var item = element.GetChildAtIndex(1).AsObject<NormalInventoryItemWrapper>();
            if (!item.IsValid || item?.Item == null || !item.Item.IsValid)
            {
                if (!force)
                    throw new MemoryReadException();
                else
                    continue;
            }

            var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(item.Item.Path);
            if (baseItemType == null || baseItemType.ClassName != "StackableCurrency" || baseItemType.BaseName == "Prophecy" || baseItemType.BaseName.Contains("Splinter"))
                continue;

            var elementLocation = element.Center;
            var amount = item.Item.GetComponent<Stack>()?.Size ?? 0;
            var baseName = baseItemType.BaseName;
            var type = currencyTypes.SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
            if (type == CurrencyType.none)
                continue;

            currency.Add(new CurrencyStack
            {
                Amount = amount,
                Type = type,
                Location = element.Center
            });
        }
        return currency;
    }

    private List<CurrencyStack> TryGetStashEssences(bool force)
    {
        var currency = new List<CurrencyStack>();
        var currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        foreach (var element in StashEssenceElements)
        {
            var item = element.AsObject<NormalInventoryItemWrapper>();
            if (!item.IsValid || item?.Item == null || !item.Item.IsValid)
            {
                if (!force)
                    throw new MemoryReadException();
                else
                    continue;
            }

            var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(item.Item.Path);
            if (baseItemType == null || baseItemType.ClassName != "StackableCurrency" || baseItemType.BaseName == "Prophecy" || baseItemType.BaseName.Contains("Splinter"))
                continue;

            var elementLocation = element.Center;
            var amount = item.Item.GetComponent<Stack>()?.Size ?? 0;
            var baseName = baseItemType.BaseName;
            var type = currencyTypes.SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
            if (type == CurrencyType.none)
                continue;

            currency.Add(new CurrencyStack
            {
                Amount = amount,
                Type = type,
                Location = element.Center
            });
        }
        return currency;
    }

    public List<CurrencyStack> PlayerInventoryCurrency
    {
        get
        {
            var currency = new List<CurrencyStack>();
            using var ctSource = new CancellationTokenSource(5000);
            while (!ctSource.IsCancellationRequested)
            {
                if (currency.Count != 0)
                    currency = new List<CurrencyStack>();

                var inventory = InventoryElement[InventoryIndex.PlayerInventory].ServerInventory;
                if (inventory == null || !inventory.InventorySlotItems.Any())
                    return currency;

                try
                {
                    foreach (var inventorySlotItem in inventory.InventorySlotItems)
                    {
                        if (inventorySlotItem?.Item == null || !inventorySlotItem.Item.IsValid)
                        {
                            throw new MemoryReadException();
                        }

                        var item = inventorySlotItem.Item;
                        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(item.Path);
                        if (baseItemType == null || string.IsNullOrEmpty(baseItemType.BaseName) || baseItemType.ClassName != "StackableCurrency") continue;
                        var currencyType = ((CurrencyType[])Enum.GetValues(typeof(CurrencyType))).SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
                        if (currencyType != CurrencyType.none)
                        {
                            var currencyStack = new CurrencyStack();
                            currencyStack.Amount = item == null || !item.HasComponent<Stack>() ? 0 : item.GetComponent<Stack>().Size;
                            currencyStack.Type = currencyType;
                            currencyStack.Location = inventorySlotItem.Center;
                            currencyStack.Slot = new Point(Convert.ToInt32(inventorySlotItem.Location.InventoryPosition.X), Convert.ToInt32(inventorySlotItem.Location.InventoryPosition.Y));
                            currency.Add(currencyStack);
                        }
                    }
                    return currency;
                }
                catch (MemoryReadException) { }
                catch (Exception ex)
                {
                    logger.LogError("Unexpected error getting inventory currency", ex);
                    break;
                }

            }
            return currency;
        }
    }

    public List<NormalInventoryItemWrapper> TheirTradeItems => TradeElement.OtherOffer;

    public Currency[] TheirTradeCurrency
    {
        get
        {
            var currency = new List<Currency>();
            foreach (var item in TheirTradeItems)
            {
                var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(item.Item.Path);
                if (baseItemType.ClassName != "StackableCurrency") continue;
                var currencyType = ((CurrencyType[])Enum.GetValues(typeof(CurrencyType))).SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
                if (currencyType != CurrencyType.none)
                {
                    var existingCurrency = currency.SingleOrDefault(c => c.Type == currencyType);
                    if (existingCurrency != null)
                        existingCurrency.Amount += item.Item.GetComponent<Stack>().Size;
                    else
                        currency.Add(new Currency { Type = currencyType, Amount = item.Item.GetComponent<Stack>().Size });
                }
            }
            return currency.ToArray();
        }
    }

    public List<ServerInventoryWrapper.InventSlotItemWrapper> PlayerInventoryItems
    {
        get
        {
            var inventorySlotItems = gameWrapper.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.MainInventory)?.InventorySlotItems;
            if (inventorySlotItems == null)
                return new List<ServerInventoryWrapper.InventSlotItemWrapper>();

            return inventorySlotItems.OrderBy(i => i.InventoryPosition.X).ThenBy(j => j.InventoryPosition.Y).ToList();
        }
    }

    public bool IsInventorySelected(Point point)
    {
        var inventorySlotItems = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].ServerInventory.InventorySlotItems;
        var visibleInventoryItems = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var itemEntity = inventorySlotItems.SingleOrDefault(i => i.Location.InventoryPositionNum.X == point.X && i.Location.InventoryPositionNum.Y == point.Y)?.Item;
        var item = visibleInventoryItems?.SingleOrDefault(i => i?.Entity?.Address != null && itemEntity != null && i.Entity.Address == itemEntity.Address);
        return item != null && item.IsSelected;
    }

    public NormalInventoryItemWrapper GetItemElementFromEntity(Entity entity)
    {
        var visibleInventoryItems = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var item = visibleInventoryItems?.SingleOrDefault(i => i?.Entity?.Address != null && entity != null && i.Entity.Address == entity.Address);
        return item;
    }

    public string[] GetTooltipLines()
    {
        var tooltip = gameWrapper.IngameState.UIHover?.Tooltip;
        if (tooltip == null)
            return Array.Empty<string>();

        var tooltipElements = gameWrapper.IngameState.UIHover.Tooltip.GetChildAtIndex(0).FindChildRecursive(c => c.ChildCount > 2).Children;
        var tooltipLines = tooltipElements.Where(c => !string.IsNullOrEmpty(c.TextNoTags)).SelectMany(c => c.TextNoTags.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries)).ToArray();
        return tooltipLines == null ? Array.Empty<string>() : tooltipLines;
    }

    public bool IsItemHovered => gameWrapper.IngameState.UIHover.Tooltip != null;

    public IEnumerable<CurrencyStack> GetStashCurrencyOfType(CurrencyType currencyType) => StashCurrencies.Where(currency => currency.Type == currencyType);

    public Point FreeItemSlot
    {
        get
        {
            var inventoryItems = gameWrapper.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.MainInventory)?.InventorySlotItems;
            if (inventoryItems == null)
                return new Point(-1, -1);

            var filledCells = new ConcurrentDictionary<int, Dictionary<int, bool>>();
            foreach (var item in inventoryItems)
            {
                for (int x = item.PosX; x < item.PosX + item.SizeX; x++)
                {
                    for (int y = item.PosY; y < item.PosY + item.SizeY; y++)
                    {
                        filledCells.GetOrAdd(x, new Dictionary<int, bool>())[y] = true;
                    }
                }
            }

            return GetFreeCell(filledCells);
        }
    }

    private Point GetFreeCell(ConcurrentDictionary<int, Dictionary<int, bool>> filledCells)
    {
        for (var x = 0; x <= 11; x++)
        {
            if (!filledCells.ContainsKey(x))
                return new Point(x, 0);

            for (var y = 0; y <= 4; y++)
            {
                if (!filledCells[x].ContainsKey(y))
                    return new Point(x, y);
            }
        }

        return new Point(-1, -1);
    }

    public Point GetLocationFromInventorySlot(Point point)
    {
        try
        {
            var playerInventElement = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var inventClientRect = playerInventElement.GetClientRect();
            var cellSize = inventClientRect.Width / 12;
            return new Point(Convert.ToInt32(inventClientRect.X + cellSize * (point.X + 0.5)), Convert.ToInt32(inventClientRect.Y + cellSize * (point.Y + 0.5)));
        }
        catch
        {
            return new Point(-1, -1);
        }
    }
    #endregion

    #region Crafting Slot
    public Entity CraftingSlotItem
    {
        get
        {
            using var ctSource = new CancellationTokenSource(50000);
            while (!ctSource.IsCancellationRequested)
            {
                var inventorySlotItems = gameWrapper.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Currency)?.InventorySlotItems;
                var item = inventorySlotItems?.SingleOrDefault(s => s.PosX == 28);
                if (item == null)
                    break;

                if (item.Item == null || !item.Item.IsValid)
                    continue;

                return item.Item;
            }
            return null;
        }
    }

    public Entity EssenceCraftingSlotItem
    {
        get
        {
            using var ctSource = new CancellationTokenSource(5000);
            while (!ctSource.IsCancellationRequested)
            {
                var inventorySlotItems = gameWrapper.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Essence)?.InventorySlotItems;
                var item = inventorySlotItems?.SingleOrDefault(s => s.PosX == 108);
                if (item == null)
                    break;

                if (item.Item == null || !item.Item.IsValid)
                    continue;

                return item.Item;
            }
            return null;
        }
    }

    public ModValueWrapper[] CraftingSlotMods => GetModValues(CraftingSlotItem);

    public ModValueWrapper[] EssenceCraftingSlotMods => GetModValues(EssenceCraftingSlotItem);

    public ItemRarity CraftingSlotRarity
    {
        get
        {
            var item = CraftingSlotItem;
            if (item == null || !item.IsValid || !item.HasComponent<Components.Mods>())
                return ItemRarity.Normal;

            return item.GetComponent<Components.Mods>().ItemRarity;
        }
    }
    #endregion

    #region Items
    public string GetClassName(Entity item)
    {
        if (!item.IsValid) return "";
        var baseTypeInfo = gameWrapper.Files.BaseItemTypes.Translate(item.Path);
        return baseTypeInfo.ClassName;
    }

    public string GetBaseType(Entity item)
    {
        if (!item.IsValid) return "";
        var baseType = item.GetComponent<Base>()?.Name ?? string.Empty;

        var mods = item.GetComponent<Components.Mods>();
        if (mods != null && GetClassName(item) == "Map" && mods.ItemMods.Any(mod => mod.Name.Equals("MapElder")))
        {
            baseType = $"Elder {baseType}";
        }

        return baseType;
    }

    public ModValueWrapper[] GetModValues(Entity item)
    {
        if (item == null || !item.IsValid)
            return [];

        var modsComponent = item.GetComponent<Components.Mods>();
        var itemMods = modsComponent?.ItemMods;
        if (itemMods == null || itemMods.Count == 0 || itemMods.Any(mod => string.IsNullOrEmpty(mod.RawName)))
            return [];

        return itemMods.Select(m => new ModValueWrapper(m, gameWrapper.Files, modsComponent.ItemLevel, gameWrapper.Files.BaseItemTypes.Translate(item.Path))).ToArray();
    }

    public string GetItemName(Entity item)
    {
        if (!item.IsValid) return "";

        var baseType = GetBaseType(item);
        var mods = item.GetComponent<Components.Mods>();
        if (mods == null)
            return GetBaseType(item);

        var name = "";
        switch (mods.ItemRarity)
        {
            case ItemRarity.Normal:
                name = $"{GetBaseType(item)}";
                break;
            case ItemRarity.Magic:
                var itemMods = GetModValues(item);
                var prefix = itemMods.FirstOrDefault(mod => mod.AffixType == ModType.Prefix);
                var suffix = itemMods.FirstOrDefault(mod => mod.AffixType == ModType.Suffix);
                name = $"{(prefix != null ? prefix.AffixText + " " : "")}{GetBaseType(item)}{((suffix != null ? " " + suffix.AffixText : ""))}";
                break;
            default:
                name = mods.UniqueName;
                break;
        }

        return name;
    }

    public bool IsItemCurrency(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && !baseItemType.BaseName.Contains("Fossil") && !baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy") && !baseItemType.BaseName.Contains("Imprinted Bestiary Orb");
    }

    public bool IsItemUniqueRing(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        var itemMods = entity.GetComponent<ExileCore.PoEMemory.Components.Mods>();
        if (itemMods == null)
            return false;

        return itemMods.ItemRarity == ItemRarity.Unique && baseItemType.ClassName == "Ring";
    }

    public bool IsItemMap(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        var itemMods = entity.GetComponent<ExileCore.PoEMemory.Components.Mods>();
        if (itemMods == null)
            return false;

        return baseItemType.ClassName == "Map";
    }

    public bool IsItemBeast(Entity entity)
    {
        var baseType = entity.GetComponent<Base>();
        if (baseType == null)
            return false;

        return baseType.Name == "Imprinted Bestiary Orb";
    }

    public bool IsItemFossil(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && baseItemType.BaseName.Contains("Fossil") && !baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy");
    }

    public bool IsItemEssence(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && !baseItemType.BaseName.Contains("Fossil") && baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy");
    }

    public bool IsItemFragment(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "MapFragment" && !baseItemType.BaseName.Contains("Pure Breachstone");
    }

    public bool IsItemResonator(Entity entity)
    {
        var baseItemType = gameWrapper.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "DelveStackableSocketableCurrency" && baseItemType.BaseName.Contains("Resonator");
    }
    #endregion

    #region Switching between StashTabs
    public int GetStashIndex(string stashName)
    {
        var allStashes = gameWrapper.IngameState.IngameUi.StashElement.AllStashNames.ToArray();
        for (var i = 0; i < allStashes.Length; i++)
        {
            if (allStashes[i] == stashName)
                return i;
        }
        return -1;
    }

    private int GetIndexOfCurrentVisibleTab()
    {
        return gameWrapper.IngameState.IngameUi.StashElement.IndexVisibleStash;
    }

    private async Task SwitchToTabViaArrowKeys(int tabIndex)
    {
        var indexOfCurrentVisibleTab = GetIndexOfCurrentVisibleTab();
        var difference = tabIndex - indexOfCurrentVisibleTab;
        var tabIsToTheLeft = difference < 0;
        var retry = 0;

        while (GetIndexOfCurrentVisibleTab() != tabIndex && retry < 3)
        {
            for (var i = 0; i < Math.Abs(difference); i++)
            {
                Input.KeyDown(tabIsToTheLeft ? Keys.Left : Keys.Right);
                Input.KeyUp(tabIsToTheLeft ? Keys.Left : Keys.Right);
                await Task.Delay(50);
            }

            await Task.Delay(20);
            retry++;
        }
    }

    public async Task<bool> SwitchToTab(string stashName)
    {
        var tabIndex = GetStashIndex(stashName);
        if (tabIndex == -1) return false;
        var latency = gameWrapper.IngameState.ServerData.Latency;

        var visibleStashIndex = GetIndexOfCurrentVisibleTab();
        var travelDistance = Math.Abs(tabIndex - visibleStashIndex);
        if (travelDistance == 0)
        {
            return true;
        }

        await SwitchToTabViaArrowKeys(tabIndex);
        await Task.Delay(latency + 50);
        return true;
    }
    #endregion

    #region Elements
    private TradeWindowWrapper TradeElement => gameWrapper.IngameState.IngameUi.TradeWindow;

    private StashElementWrapper StashElement => gameWrapper.IngameState.IngameUi.StashElement;

    private SocialElementWrapper SocialElement => gameWrapper.IngameState.IngameUi.SocialPanel;

    private InventoryElementWrapper InventoryElement => gameWrapper.IngameState.IngameUi.InventoryPanel;

    private InvitesPanelWrapper InviteElement => gameWrapper.IngameState.IngameUi.InvitesPanel;

    private PartyElementWrapper PartyElement => gameWrapper.IngameState.IngameUi.PartyElement;

    private DestroyConfirmationElementWrapper DestroyConfirmationElement => gameWrapper.IngameState.IngameUi.DestroyConfirmationWindow;

    private ChatPanelWrapper ChatPanel => gameWrapper.IngameState.IngameUi.ChatPanel;

    private ElementWrapper PopUpWindowElement => gameWrapper.IngameState.IngameUi.PopUpWindow;
    #endregion

    #region Helpers
    private Point GetEntityLocation(Entity entity)
    {
        var vector = gameWrapper.IngameState.Camera.WorldToScreen(entity.PosNum);
        return new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
    }
    #endregion
}

using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using Microsoft.Extensions.Logging;
using PoeHudWrapper.MemoryObjects;
using PoeLib.Common;
using PoeTrade.Contracts.Extensions;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace PoeHudWrapper;

public interface IPoeHudWrapper
{
    bool AcceptButtonClickable { get; }
    bool AcceptButtonClicked { get; }
    Point AcceptTradeButtonLocation { get; }
    string AreaName { get; }
    string[] CharactersInArea { get; }
    string ChatText { get; }
    bool ChatVisible { get; }
    Rectangle ClientBounds { get; }
    Entity? CraftingSlotItem { get; }
    Point CraftingSlotLocation { get; }
    Point EssenceCraftingSlotLocation { get; }
    ModValue[] CraftingSlotMods { get; }
    ModValue[] EssenceCraftingSlotMods { get; }
    PoeTrade.Contracts.ItemRarity CraftingSlotRarity { get; }
    Point DestroyConfirmationKeepButtonLocation { get; }
    bool DestroyConfirmationVisible { get; }
    bool InGame { get; }
    bool InventoryPanelOpen { get; }
    bool IsLoading { get; }
    bool ItemOnCursor { get; }
    int Latency { get; }
    string[] PartyMemberNames { get; }
    List<PartyElementPlayerElement> PartyMembers { get; }
    Point PartyTabLocation { get; }
    string[] PendingInvites { get; }
    List<CurrencyStack> PlayerInventoryCurrency { get; }
    List<ServerInventory.InventSlotItem> PlayerInventoryItems { get; }
    NormalInventoryItem? GetItemElementFromEntity(Entity entity);
    string[] GetTooltipLines();
    bool IsItemHovered { get; }
    bool SocialPanelOpen { get; }
    List<CurrencyStack> StashCurrencies { get; }
    List<CurrencyStack> StashEssences { get; }
    Point StashLocation { get; }
    bool StashOpen { get; }
    PoeTrade.Contracts.CurrencyInfo[] TheirTradeCurrency { get; }
    IList<NormalInventoryItem> TheirTradeItems { get; }
    InvitesPanelItem[] PartyInvites { get; }
    InvitesPanelItem[] TradeInvites { get; }
    bool TradeWindowOpen { get; }
    string TradeWindowSellerName { get; }
    bool TradeWindowVisible { get; }
    bool PartyTabVisible { get; }
    string GetBaseType(Entity item);
    string GetClassName(Entity item);
    Point FreeItemSlot { get; }
    string GetItemName(Entity item);
    ModValue[] GetModValues(Entity item);
    Point GetPartyInviteAcceptLocation(string characterName);
    Point GetPartyInviteDeclineLocation(string characterName);
    string GetPartyMemberZone(string characterName);
    Point PartyMemberPortalButtonLocation(string characterName);
    IEnumerable<CurrencyStack> GetStashCurrencyOfType(PoeTrade.Contracts.TradeCurrencyType currencyType);
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
}

public class PoeHudWrapper : IPoeHudWrapper
{
    private readonly Core core;
    private readonly ILogger<PoeHudWrapper> logger;

    public PoeHudWrapper(Core core, ILogger<PoeHudWrapper> logger)
    {
        this.core = core;
        this.logger = logger;
    }

    public int Latency => core.GameController.IngameState.ServerData.Latency;
    public Rectangle ClientBounds => WinApi.GetClientRectangle(core.GameController.Memory.Process.MainWindowHandle);
    public bool InGame => core.GameController.Game.InGame;
    public bool IsLoading => core.GameController.Game.IsLoading;
    public string AreaName => core.GameController.IngameState.Data.CurrentArea.Name;
    public bool StashOpen => StashElement.IsVisible;
    public bool TradeWindowOpen => TradeElement.IsVisible;
    public bool InventoryPanelOpen => InventoryElement.IsVisible;
    public bool SocialPanelOpen => SocialElement.IsVisible;
    public bool PartyTabVisible => SocialElement.SocialPanels[SocialTabTypes.Party].IsVisible;
    public bool ItemOnCursor => core.GameController.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Cursor)?.ItemCount > 0;
    public bool DestroyConfirmationVisible => PopUpElement.IsVisible;
    public bool ChatVisible => ChatPanel.ChatInputElement.IsVisible;
    public string ChatText => ChatPanel.InputText;
    public bool MenuOpen => core.GameController.Game.IsEscapeState;
    public bool IsAtCharacterSelect => core.GameController.Game.IsSelectCharacterState;
    public bool IsAtLogin => core.GameController.Game.IsLoginState;

    #region Social
    public string[] PendingInvites => SocialElement.PartyTab.PendingInvites.Where(element => element.ChildCount == 1).Select(invite => invite.GetChildFromIndices(0, 1, 0, 0, 0).Text).ToArray();
    public Point GetPartyInviteAcceptLocation(string characterName) => SocialElement.PartyTab.PendingInvites.SingleOrDefault(i => i.Name.Equals(characterName))?.AcceptButton.Center ?? new Point(-1, -1);
    public Point GetPartyInviteDeclineLocation(string characterName) => SocialElement.PartyTab.PendingInvites.SingleOrDefault(i => i.Name.Equals(characterName))?.DeclineButton.Center ?? new Point(-1, -1);
    #endregion

    #region Party
    public string[] PartyMemberNames => PartyElement.Information.Keys.ToArray();
    public List<PartyElementPlayerElement> PartyMembers => PartyElement.PlayerElements;
    public string GetPartyMemberZone(string characterName) => PartyElement.PlayerElements.SingleOrDefault(e => e.PlayerName.Equals(characterName))?.ZoneName ?? string.Empty;
    public Point PartyMemberPortalButtonLocation(string characterName) => PartyElement.PlayerElements.SingleOrDefault(e => e.PlayerName.Equals(characterName))?.TeleportButton.Center ?? new Point(-1, -1);
    public bool PlayerInZone(string characterName) => CharactersInArea.Contains(characterName);

    public string[] CharactersInArea
    {
        get
        {
            var players = core.GameController.Entities.Where(entity => entity.Type == EntityType.Player);
            if (players == null || !players.Any())
                return new string[0];

            return players.Select(p => p.GetComponent<Player>().PlayerName).Where(name => !string.IsNullOrEmpty(name)).ToArray();
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
    public string TradeWindowSellerName => TradeElement.NameSeller;
    public Point AcceptTradeButtonLocation => TradeElement.AcceptButton?.Center ?? new Point(-1, -1);
    public bool AcceptButtonClickable => !TradeElement.AcceptButton?.IsSaturated ?? true;
    public bool AcceptButtonClicked => TradeElement.SellerAccepted;
    public bool TradeInviteSentVisible => PopUpElement.IsVisible;
    #endregion

    #region PlayerInvites
    public InvitesPanelItem[] PartyInvites => InviteElement.Invites.Where(i => i.Kind == InvitesPanelItemKind.Party).ToArray();
    public InvitesPanelItem[] TradeInvites => InviteElement.Invites.Where(i => i.Kind == InvitesPanelItemKind.Trade).ToArray();
    #endregion

    #region Locations
    public Point DestroyConfirmationKeepButtonLocation => PopUpElement.GetChildFromIndices(0, 0, 3, 1)?.Center ?? new Point(-1, -1);
    public Point PartyTabLocation => SocialElement.SocialTabs.ContainsKey(SocialTabTypes.Party) ? SocialElement.SocialTabs[SocialTabTypes.Party].Center : new Point(-1, -1);
    public Point CraftingSlotLocation => StashElement.VisibleStash?.GetChildAtIndex(3)?.Center ?? new Point(-1, -1);
    public Point EssenceCraftingSlotLocation => StashElement.VisibleStash?.GetChildAtIndex(108)?.Center ?? new Point(-1, -1);

    public Point StashLocation
    {
        get
        {
            var entities = core.GameController.Entities;
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

    private IEnumerable<Element> StashCurrencyElements
    {
        get
        {
            if (StashElement.VisibleStash == null || StashElement.VisibleStash.ChildCount == 0)
                return Enumerable.Empty<Element>();

            return StashElement.VisibleStash.GetChildAtIndex(1).Children.Where(slot => slot.ChildCount == 2) // GeneralCurrency
                    .Concat(StashElement.VisibleStash.Children.Skip(4).Where(slot => slot.ChildCount == 2)); // MiscCurrency
        }
    }

    private IEnumerable<Element> StashEssenceElements
    {
        get
        {
            if (StashElement.VisibleStash == null || StashElement.VisibleStash.ChildCount == 0)
                return Enumerable.Empty<Element>();

            return StashElement.VisibleStash.VisibleInventoryItems;
        }
    }

    private List<CurrencyStack> TryGetStashCurrency(bool force)
    {
        var currency = new List<CurrencyStack>();
        var currencyTypes = (PoeTrade.Contracts.TradeCurrencyType[])Enum.GetValues(typeof(PoeTrade.Contracts.TradeCurrencyType));
        foreach (var element in StashCurrencyElements)
        {
            var item = element.GetChildAtIndex(1).AsObject<NormalInventoryItem>();
            if (!item.IsValid || item?.Item == null || !item.Item.IsValid)
            {
                if (!force)
                    throw new MemoryReadException();
                else
                    continue;
            }

            var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(item.Item.Path);
            if (baseItemType == null || baseItemType.ClassName != "StackableCurrency" || baseItemType.BaseName == "Prophecy" || baseItemType.BaseName.Contains("Splinter"))
                continue;

            var elementLocation = element.Center;
            var amount = item.Item.GetComponent<Stack>()?.Size ?? 0;
            var baseName = baseItemType.BaseName;
            var type = currencyTypes.SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
            if (type == PoeTrade.Contracts.TradeCurrencyType.Unknown)
                continue;

            currency.Add(new CurrencyStack(type, element.Center) { Amount = amount });
        }
        return currency;
    }

    private List<CurrencyStack> TryGetStashEssences(bool force)
    {
        var currency = new List<CurrencyStack>();
        var currencyTypes = (PoeTrade.Contracts.TradeCurrencyType[])Enum.GetValues(typeof(PoeTrade.Contracts.TradeCurrencyType));
        foreach (var element in StashEssenceElements)
        {
            var item = element.AsObject<NormalInventoryItem>();
            if (!item.IsValid || item?.Item == null || !item.Item.IsValid)
            {
                if (!force)
                    throw new MemoryReadException();
                else
                    continue;
            }

            var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(item.Item.Path);
            if (baseItemType == null || baseItemType.ClassName != "StackableCurrency" || baseItemType.BaseName == "Prophecy" || baseItemType.BaseName.Contains("Splinter"))
                continue;

            var elementLocation = element.Center;
            var amount = item.Item.GetComponent<Stack>()?.Size ?? 0;
            var baseName = baseItemType.BaseName;
            var type = currencyTypes.SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
            if (type == PoeTrade.Contracts.TradeCurrencyType.Unknown)
                continue;

            currency.Add(new CurrencyStack(type, element.Center) {  Amount = amount });
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
                        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(item.Path);
                        if (baseItemType == null || string.IsNullOrEmpty(baseItemType.BaseName) || baseItemType.ClassName != "StackableCurrency") continue;
                        var currencyType = ((PoeTrade.Contracts.TradeCurrencyType[])Enum.GetValues(typeof(PoeTrade.Contracts.TradeCurrencyType))).SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
                        if (currencyType != PoeTrade.Contracts.TradeCurrencyType.Unknown)
                        {
                            var currencyStack = new CurrencyStack(currencyType, inventorySlotItem.GetCenter()) { Amount = item == null || !item.HasComponent<Stack>() ? 0 : item.GetComponent<Stack>().Size };
                            currencyStack.Slot = new Point(Convert.ToInt32(inventorySlotItem.Location.InventoryPositionNum.X), Convert.ToInt32(inventorySlotItem.Location.InventoryPositionNum.Y));
                            currency.Add(currencyStack);
                        }
                    }
                    return currency;
                }
                catch (MemoryReadException) { }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error getting inventory currency");
                    break;
                }

            }
            return currency;
        }
    }

    public IList<NormalInventoryItem> TheirTradeItems => TradeElement.OtherOffer;

    public PoeTrade.Contracts.CurrencyInfo[] TheirTradeCurrency
    {
        get
        {
            var currency = new List<PoeTrade.Contracts.CurrencyInfo>();
            foreach (var item in TheirTradeItems)
            {
                var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(item.Item.Path);
                if (baseItemType.ClassName != "StackableCurrency") continue;
                var currencyType = ((PoeTrade.Contracts.TradeCurrencyType[])Enum.GetValues(typeof(PoeTrade.Contracts.TradeCurrencyType))).SingleOrDefault(c => c.GetCurrencyDescription() == baseItemType.BaseName);
                if (currencyType != PoeTrade.Contracts.TradeCurrencyType.Unknown)
                {
                    var existingCurrency = currency.SingleOrDefault(c => c.Type == currencyType);
                    if (existingCurrency != null)
                        existingCurrency.Amount += item.Item.GetComponent<Stack>().Size;
                    else
                        currency.Add(new PoeTrade.Contracts.CurrencyInfo { Type = currencyType, Amount = item.Item.GetComponent<Stack>().Size });
                }
            }
            return currency.ToArray();
        }
    }

    public List<ServerInventory.InventSlotItem> PlayerInventoryItems
    {
        get
        {
            var inventorySlotItems = core.GameController.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.MainInventory)?.InventorySlotItems;
            if (inventorySlotItems == null)
                return new List<ServerInventory.InventSlotItem>();

            return inventorySlotItems.OrderBy(i => i.InventoryPositionNum.X).ThenBy(j => j.InventoryPositionNum.Y).ToList();
        }
    }

    public bool IsInventorySelected(Point point)
    {
        var inventorySlotItems = core.GameController.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].ServerInventory.InventorySlotItems;
        var visibleInventoryItems = core.GameController.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var itemEntity = inventorySlotItems.SingleOrDefault(i => i.Location.InventoryPositionNum.X == point.X && i.Location.InventoryPositionNum.Y == point.Y)?.Item;
        var item = visibleInventoryItems?.SingleOrDefault(i => i?.Entity?.Address != null && itemEntity != null && i.Entity.Address == itemEntity.Address);
        return item != null && !item.IsSaturated;
    }

    public NormalInventoryItem? GetItemElementFromEntity(Entity entity)
    {
        var visibleInventoryItems = core.GameController.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems ?? Array.Empty<NormalInventoryItem>();
        return visibleInventoryItems.SingleOrDefault(i => i.Entity?.Address != null && entity != null && i.Entity.Address == entity.Address);
    }

    public string[] GetTooltipLines()
    {
        var tooltip = core.GameController.IngameState.UIHover?.Tooltip;
        if (tooltip == null)
            return Array.Empty<string>();

        const int stringLength = 512;
        var tooltipElements = core.GameController.IngameState.UIHover?.Tooltip.GetChildAtIndex(0).FindChildRecursive(c => c.ChildCount > 2).Children ?? Array.Empty<Element>();
        var tooltipLines = tooltipElements.Where(c => !string.IsNullOrEmpty(c.GetTextWithNoTags(stringLength))).SelectMany(c => c.GetTextWithNoTags(stringLength).Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries)).ToArray();
        return tooltipLines == null ? Array.Empty<string>() : tooltipLines;
    }

    public bool IsItemHovered => core.GameController.IngameState.UIHover.Tooltip != null;

    public IEnumerable<CurrencyStack> GetStashCurrencyOfType(PoeTrade.Contracts.TradeCurrencyType currencyType) => StashCurrencies.Where(currency => currency.Type == currencyType);

    public Point FreeItemSlot
    {
        get
        {
            var inventoryItems = core.GameController.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.MainInventory)?.InventorySlotItems;
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
            var playerInventElement = core.GameController.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
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
    public Entity? CraftingSlotItem
    {
        get
        {
            using var ctSource = new CancellationTokenSource(50000);
            while (!ctSource.IsCancellationRequested)
            {
                var inventorySlotItems = core.GameController.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Currency)?.InventorySlotItems;
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

    public Entity? EssenceCraftingSlotItem
    {
        get
        {
            using var ctSource = new CancellationTokenSource(5000);
            while (!ctSource.IsCancellationRequested)
            {
                var inventorySlotItems = core.GameController.IngameState.ServerData.GetPlayerInventoryByType(InventoryTypeE.Essence)?.InventorySlotItems;
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

    public ModValue[] CraftingSlotMods => GetModValues(CraftingSlotItem);

    public ModValue[] EssenceCraftingSlotMods => GetModValues(EssenceCraftingSlotItem);

    public PoeTrade.Contracts.ItemRarity CraftingSlotRarity
    {
        get
        {
            var item = CraftingSlotItem;
            if (item == null || !item.IsValid || !item.HasComponent<Mods>())
                return PoeTrade.Contracts.ItemRarity.Normal;

            return (PoeTrade.Contracts.ItemRarity)item.GetComponent<Mods>().ItemRarity;
        }
    }
    #endregion

    #region Items
    public string GetClassName(Entity item)
    {
        if (!item.IsValid) return "";
        var baseTypeInfo = core.GameController.Game.Files.BaseItemTypes.Translate(item.Path);
        return baseTypeInfo.ClassName;
    }

    public string GetBaseType(Entity item)
    {
        if (!item.IsValid) return "";
        var baseType = item.GetComponent<Base>()?.Name ?? string.Empty;

        var mods = item.GetComponent<Mods>();
        if (mods != null && GetClassName(item) == "Map" && mods.ItemMods.Any(mod => mod.Name.Equals("MapElder")))
        {
            baseType = $"Elder {baseType}";
        }

        return baseType;
    }

    public ModValue[] GetModValues(Entity? item)
    {
        if (item == null || !item.IsValid)
            return [];

        var modsComponent = item.GetComponent<Mods>();
        if (modsComponent == null)
            return Array.Empty<ModValue>();

        var itemMods = modsComponent.ItemMods;
        if (itemMods == null || itemMods.Count == 0 || itemMods.Any(mod => string.IsNullOrEmpty(mod.RawName)))
            return [];

        return itemMods.Select(m => new ModValue(m, core.GameController.Files, modsComponent.ItemLevel, core.GameController.Files.BaseItemTypes.Translate(item.Path))).ToArray();
    }

    public string GetItemName(Entity item)
    {
        if (!item.IsValid) return "";

        var baseType = GetBaseType(item);
        var mods = item.GetComponent<Mods>();
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
                name = string.IsNullOrEmpty(mods.UniqueName) ? baseType : mods.UniqueName;
                break;
        }

        return name;
    }

    public bool IsItemCurrency(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && !baseItemType.BaseName.Contains("Fossil") && !baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy") && !baseItemType.BaseName.Contains("Imprinted Bestiary Orb");
    }

    public bool IsItemUniqueRing(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        var itemMods = entity.GetComponent<ExileCore.PoEMemory.Components.Mods>();
        if (itemMods == null)
            return false;

        return itemMods.ItemRarity == ItemRarity.Unique && baseItemType.ClassName == "Ring";
    }

    public bool IsItemMap(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
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
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && baseItemType.BaseName.Contains("Fossil") && !baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy");
    }

    public bool IsItemEssence(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "StackableCurrency" && !baseItemType.BaseName.Contains("Fossil") && baseItemType.BaseName.Contains("Essence") && !baseItemType.BaseName.Contains("Prophecy");
    }

    public bool IsItemFragment(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "MapFragment" && !baseItemType.BaseName.Contains("Pure Breachstone");
    }

    public bool IsItemResonator(Entity entity)
    {
        var baseItemType = core.GameController.Game.Files.BaseItemTypes.Translate(entity.Path);
        if (baseItemType == null || baseItemType.BaseName == null || baseItemType.ClassName == null)
            return false;

        return baseItemType.ClassName == "DelveStackableSocketableCurrency" && baseItemType.BaseName.Contains("Resonator");
    }
    #endregion

    #region Switching between StashTabs
    public int GetStashIndex(string stashName)
    {
        var allStashes = core.GameController.IngameState.IngameUi.StashElement.Inventories.Select(i => i.TabName).ToArray();
        for (var i = 0; i < allStashes.Length; i++)
        {
            if (allStashes[i] == stashName)
                return i;
        }
        return -1;
    }

    private int GetIndexOfCurrentVisibleTab()
    {
        return core.GameController.IngameState.IngameUi.StashElement.IndexVisibleStash;
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
        var latency = core.GameController.IngameState.ServerData.Latency;

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
    private TradeWindow TradeElement => core.GameController.IngameState.IngameUi.TradeWindow;

    private StashElement StashElement => core.GameController.IngameState.IngameUi.StashElement;

    private SocialElement SocialElement => core.GameController.IngameState.IngameUi.SocialPanel;

    private InventoryElement InventoryElement => core.GameController.IngameState.IngameUi.InventoryPanel;

    private InvitesPanel InviteElement => core.GameController.IngameState.IngameUi.InvitesPanel;

    private PartyElement PartyElement => core.GameController.IngameState.IngameUi.PartyElement;

    private Element PopUpElement => core.GameController.IngameState.IngameUi.PopUpWindow;

    private ChatPanel ChatPanel => core.GameController.IngameState.IngameUi.ChatPanel;
    #endregion

    #region Helpers
    private Point GetEntityLocation(Entity entity)
    {
        var vector = core.GameController.IngameState.Camera.WorldToScreen(entity.PosNum);
        return new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
    }
    #endregion
}

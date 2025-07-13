using ExileCore.Shared.Enums;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class InventoryElementWrapper : ElementWrapper
{
    private InventoryListWrapper _allInventories;
    private InventoryListWrapper AllInventories => _allInventories = _allInventories ?? GetObjectAt<InventoryListWrapper>(0x368);
    public InventoryWrapper this[InventoryIndex k] => AllInventories[k];

    public IList<ElementWrapper> GetItemsInInventory()
    {
        var playerInventory = GetElementSlot(InventoryIndex.PlayerInventory);
        var items = playerInventory.Children;
        items.RemoveAt(0);
        return items;
    }

    public ElementWrapper GetElementSlot(InventoryIndex inventoryIndex)
    {
        switch (inventoryIndex)
        {
            case InventoryIndex.None:
                throw new ArgumentOutOfRangeException(nameof(inventoryIndex));
            case InventoryIndex.Helm:
                return EquippedItems.GetChildAtIndex(10);
            case InventoryIndex.Amulet:
                return EquippedItems.GetChildAtIndex(11);
            case InventoryIndex.Chest:
                return EquippedItems.GetChildAtIndex(17);
            case InventoryIndex.LWeapon:
                return EquippedItems.GetChildAtIndex(14);
            case InventoryIndex.RWeapon:
                return EquippedItems.GetChildAtIndex(13);
            case InventoryIndex.LWeaponSwap:
                return EquippedItems.GetChildAtIndex(16);
            case InventoryIndex.RWeaponSwap:
                return EquippedItems.GetChildAtIndex(15);
            case InventoryIndex.LRing:
                return EquippedItems.GetChildAtIndex(18);
            case InventoryIndex.RRing:
                return EquippedItems.GetChildAtIndex(19);
            case InventoryIndex.Gloves:
                return EquippedItems.GetChildAtIndex(20);
            case InventoryIndex.Belt:
                return EquippedItems.GetChildAtIndex(21);
            case InventoryIndex.Boots:
                return EquippedItems.GetChildAtIndex(22);
            case InventoryIndex.PlayerInventory:
                return EquippedItems.GetChildAtIndex(24);
            case InventoryIndex.Flask:
                return EquippedItems.GetChildAtIndex(23);
            default:
                throw new ArgumentOutOfRangeException(nameof(inventoryIndex));
        }
    }
    private ElementWrapper EquippedItems => GetChildAtIndex(3);
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour {

    [System.Serializable]
    public class ItemSlot {
        public ItemStack itemStack;
        public override string ToString() {
            return itemStack.ToString();
        }
    }

    [SerializeField] int numSlots = 1;
    public ItemSlot[] itemSlots;


    private void Awake() {
        Init(numSlots);
    }

    void Init(int numSlots) {
        itemSlots = new ItemSlot[numSlots];
        for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i] = new ItemSlot();
            ItemSlot itemslot = itemSlots[i];
            itemslot.itemStack = new ItemStack();
            itemslot.itemStack.item = null;
            itemslot.itemStack.count = 0; ;
        }
    }
    IEnumerable<ItemSlot> GetItemStacksOfTypes(params ItemType[] matchingItemTypes) =>
        itemSlots.Where(sl => matchingItemTypes.Contains(sl.itemStack.item?.itemType));
    IEnumerable<ItemSlot> GetEmptyItemSlots() =>
        itemSlots.Where(sl => sl.itemStack.IsEmpty);

    private ItemSlot GetFirstEmptySlot() => itemSlots.FirstOrDefault(sl => sl.itemStack.IsEmpty);
    private ItemSlot GetFirstNotEmptySlot() => itemSlots.FirstOrDefault(sl => !sl.itemStack.IsEmpty);
    private ItemSlot GetFirstFilledSlot() => itemSlots.FirstOrDefault(sl => sl.itemStack.IsFull);

    private ItemSlot GetFirstNotEmptySlotOfType(params ItemType[] matchingItemTypes) => itemSlots.FirstOrDefault(sl =>
        matchingItemTypes.Contains(sl.itemStack.item?.itemType)
        && !sl.itemStack.IsEmpty);
    private ItemSlot GetFirstNotEmptyOrFullSlotOfType(params ItemType[] matchingItemTypes) => itemSlots.FirstOrDefault(sl =>
        matchingItemTypes.Contains(sl.itemStack.item?.itemType)
        && !sl.itemStack.IsEmpty && !sl.itemStack.IsFull);


    public bool HasAnyItems() => itemSlots.Any(sl => sl.itemStack.count > 0);
    public bool HasAnyItemsOfType(params ItemType[] matchingItemTypes) =>
        itemSlots.Any(sl => matchingItemTypes.Contains(sl.itemStack.item?.itemType) && sl.itemStack.count > 0);

    public bool HasSpaceFor(ItemType itemType) => HasSpaceFor(itemType, 1);
    // itemSlots.Any(sl => sl.itemStack.IsEmpty || (sl.itemStack.item.itemType == itemType && !sl.itemStack.IsFull));
    public bool HasSpaceFor(ItemType itemType, int count) {
        int maxSpace = GetAvailableSpaceFor(itemType);
        return maxSpace >= count;
    }

    public int GetAvailableSpaceFor(ItemType itemType) {
        // get all empty and not full matching slots
        IEnumerable<ItemSlot> emptySlots = GetEmptyItemSlots();
        IEnumerable<ItemSlot> nonFullMatchingSlots = GetItemStacksOfTypes(itemType);
        // Debug.Log("as " + itemSlots.ToStringFull());
        int maxSpace = emptySlots.Count() * itemType.itemMaxStack;
        // Debug.Log("es " + maxSpace);
        maxSpace += nonFullMatchingSlots.Sum(slot => slot.itemStack.RemainingSpace);
        // Debug.Log("ts " + maxSpace+" "+nonFullMatchingSlots.Count());
        return maxSpace;
    }

    public void AddItems(IEnumerable<ItemStack> itemStacks) {

    }
    public void AddItem(Item item, int count) {
        // probably a better way of doing this...
        for (int i = 0; i < count; i++) {
            AddItem(item);
        }
    }
    /// <summary>
    /// Adds one item
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item) {
        ItemSlot itemSlot = GetFirstNotEmptyOrFullSlotOfType(item.itemType);
        if (itemSlot == null) {
            itemSlot = GetFirstEmptySlot();
            itemSlot.itemStack.item = item;
            if (itemSlot == null) {
                Debug.LogWarning("Cant add item, inventory full!");
                return;
            }
        }
        itemSlot.itemStack.count++;
    }
    public Item TakeFirstItem() {
        ItemSlot itemSlot = GetFirstNotEmptySlot();
        if (itemSlot == null) {
            // empty
            return null;
        }
        itemSlot.itemStack.count--;
        if (itemSlot.itemStack.count <= 0) {
            itemSlot.itemStack.item = null;
        }
        return itemSlot.itemStack.item;
    }
    public void Sort() {
        // maximize maxed stacks
        IEnumerable<ItemType> allItemTypes = itemSlots.Where(sl => sl.itemStack.HasItem).Select(sl => sl.itemStack.item.itemType).Distinct();
        foreach (var itemType in allItemTypes) {
            // ignore full ones
            IEnumerable<ItemSlot> slots = GetItemStacksOfTypes(itemType).Where(sl => !sl.itemStack.IsFull);
            if (slots.Count() < 1) continue;
            // take from last and add to first, until only one(or zero) is not full
            while (slots.Count() > 1) {
                slots.Last().itemStack.count--;
                slots.First().itemStack.count++;
                if (slots.Last().itemStack.IsEmpty) {
                    slots.Last().itemStack.item = null;
                    slots = slots.SkipLast(1);
                }
                if (slots.First().itemStack.IsFull) {
                    // remove it
                    slots = slots.Skip(1);
                }
            }
        }

        // order
        System.Array.Sort(itemSlots, (a, b) => {
            if (!a.itemStack.HasItem && !b.itemStack.HasItem) return 0;
            if (a.itemStack.HasItem && !b.itemStack.HasItem) return -1;
            if (!a.itemStack.HasItem && b.itemStack.HasItem) return 1;
            // higher first
            return -a.itemStack.item.itemType.sortOrder + b.itemStack.item.itemType.sortOrder;
        });
    }
}
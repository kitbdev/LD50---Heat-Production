using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {
    public ItemType itemType;
    public Item(ItemType itemType) {
        this.itemType = itemType;
    }
}
[System.Serializable]
public class ItemStack {
    public Item item;
    public int count;
    public bool IsFull => item != null && count >= item.itemType.itemMaxStack;
    public bool IsEmpty => item == null || count == 0;
    public bool HasItem => item != null;
    public int RemainingSpace => item == null ? 0 : item.itemType.itemMaxStack - count;
    public override string ToString() {
        return IsEmpty ? "Empty Stack" : (count + " " + item.itemType.ToString() + "s stack");
    }
}
[System.Serializable]
public class ItemRecipe {
    public ItemStack[] requiredItems;
    // ? includes liquids?
    public ItemStack[] producedItems;
}

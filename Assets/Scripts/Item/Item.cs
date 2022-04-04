using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {
    public ItemType itemType;
    public Item(ItemType itemType) {
        this.itemType = itemType;
    }
    public override string ToString() {
        return "Item " + (itemType?.ToString() ?? "Error");
    }
}
[System.Serializable]
public struct ItemStack {
    public ItemType itemType;
    public int count;

    public bool IsFull => itemType != null && count >= itemType.itemMaxStack;
    public bool IsEmpty => itemType == null || count == 0;
    public bool HasItem => itemType != null;
    public int RemainingSpace => itemType == null ? 0 : itemType.itemMaxStack - count;

    public override string ToString() {
        return IsEmpty ? "Empty Stack" : ($"{count} {itemType.ToString()}{(count == 1 ? "" : "s")}");
    }

    public ItemStack Copy() {
        return new ItemStack() {
            count = count,
            itemType = itemType
        };
    }
}

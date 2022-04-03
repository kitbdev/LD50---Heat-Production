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
    public ItemType itemType;
    public int count;
    
    public bool IsFull => itemType != null && count >= itemType.itemMaxStack;
    public bool IsEmpty => itemType == null || count == 0;
    public bool HasItem => itemType != null;
    public int RemainingSpace => itemType == null ? 0 : itemType.itemMaxStack - count;
    
    public override string ToString() {
        return IsEmpty ? "Empty Stack" : (count + " " + itemType.ToString() + "s stack");
    }
    
    public ItemStack Copy() {
        return new ItemStack() {
            count = count,
            itemType = itemType
        };
    }
}

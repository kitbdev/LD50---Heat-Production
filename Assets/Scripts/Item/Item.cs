using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemType {
    public int itemMaxStack;
}
[System.Serializable]
public class Item {
    public ItemType itemType;
}
[System.Serializable]
public class ItemStack {
    public Item item;
    public int count;
}
[System.Serializable]
public class ItemRecipe {
    public ItemStack[] requiredItems;
    // ? includes liquids?
    public ItemStack[] producedItems;
}

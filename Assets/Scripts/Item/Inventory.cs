using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory {
    [System.Serializable]
    public class ItemSlot {
        public ItemStack itemStack;
    }
    public ItemSlot[] itemSlots;

    public void Init(int numSlots){
        itemSlots = new ItemSlot[numSlots];
    }
}
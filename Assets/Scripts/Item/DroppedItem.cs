using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class DroppedItem : MonoBehaviour {
    public Item item;
    // public ItemStack itemStack;
    [SerializeField] SpriteRenderer spriteR;

    public void UpdateSprite() {
        spriteR.sprite = item.itemType?.icon;
    }
}
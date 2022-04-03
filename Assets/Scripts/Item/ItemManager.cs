using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// for items and dropped items
/// </summary>
public class ItemManager : Singleton<ItemManager> {

    public ItemType[] allItemTypes;
    public ItemRecipe[] allItemRecipes;
    public GameObject droppedItemPrefab;

    public DroppedItem DropItem(Item item) {
        GameObject droppedGo = Instantiate(droppedItemPrefab, transform);
        droppedGo.name = item.itemType.name + " dropped";
        DroppedItem droppedItem = droppedGo.GetComponent<DroppedItem>();
        droppedItem.item = item;
        return droppedItem;
    }




    [ContextMenu("Find Types")]
    void FindTypes() {
        allItemTypes = AssetHelper.AutoFindAllAssets<ItemType>("Assets/Data/Items");
        // allItemRecipes = AssetHelper.AutoFindAllAssets<ItemRecipe>("Assets/Data/ItemRecipes");
    }

}
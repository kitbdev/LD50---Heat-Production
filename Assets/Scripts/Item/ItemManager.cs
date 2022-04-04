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
        if (item == null || item.itemType == null) {
            Debug.LogWarning("Cant drop null item!");
            return null;
        }
        GameObject droppedGo = Instantiate(droppedItemPrefab, transform);
        droppedGo.name = item.itemType.name + " dropped";
        droppedGo.transform.localPosition = Vector3.zero;
        DroppedItem droppedItem = droppedGo.GetComponent<DroppedItem>();
        droppedItem.item = item;
        droppedItem.UpdateSprite();
        return droppedItem;
    }




    [ContextMenu("Find Types")]
    void FindTypes() {
        allItemTypes = AssetHelper.AutoFindAllAssets<ItemType>("Assets/Data/Items");
        // allItemRecipes = AssetHelper.AutoFindAllAssets<ItemRecipe>("Assets/Data/ItemRecipes");
    }

}
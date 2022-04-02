using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// for items and dropped items
/// </summary>
public class ItemManager : Singleton<ItemManager> {

    public ItemType[] allItemTypes;
    public GameObject droppedItemPrefab;

    public DroppedItem DropItem(Item item) {
        GameObject droppedGo = Instantiate(droppedItemPrefab, transform);
        droppedGo.name = item.itemType.name + " dropped";
        DroppedItem droppedItem = droppedGo.GetComponent<DroppedItem>();
        droppedItem.item = item;
        return droppedItem;
    }





    [ContextMenu("Find Types")]
    void AutoFindAllNodeTypePrefabs() {
#if UNITY_EDITOR
        // Find all Gameobjects that have 'co' in their filename, that are labelled with 'architecture' and are placed in 'MyAwesomeProps' folder
        const string folder = "Assets/Data/Items";
        string[] guids2 = UnityEditor.AssetDatabase.FindAssets("", new[] { folder });
        List<ItemType> loadAssets = new List<ItemType>();
        foreach (string guid2 in guids2) {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid2);
            // Debug.Log("Loading " + path);
            loadAssets.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemType>(path));
        }
        allItemTypes = loadAssets.Where(p => p != null).ToArray();
#endif
    }
}
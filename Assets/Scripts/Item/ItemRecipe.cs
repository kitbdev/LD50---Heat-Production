using UnityEngine;

[CreateAssetMenu(fileName = "ItemRecipe", menuName = "HP/ItemRecipe", order = 0)]
public class ItemRecipe : ScriptableObject {
    // [System.Serializable]
    // public class ItemRecipe {

    public ItemStack[] requiredItems;
    // ? includes liquids?
    public ItemStack[] producedItems;
}

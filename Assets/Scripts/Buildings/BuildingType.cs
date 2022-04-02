using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "BuildingType", menuName = "HP/BuildingType", order = 0)]
public class BuildingType : ScriptableObject {
    // [System.Serializable]
    // public class BuildingType {
    // public GameObject ghostPrefab;
    public GameObject buildingPrefab;
    public Vector2Int[] localOccupiedSpaces;
    public BuildingRecipe buildingRecipe;
}
[System.Serializable]
public class BuildingRecipe {
    public ItemStack[] requiredItems;
    // public BuildingType buildingType;
}
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

    public Building buildingPrf => buildingPrefab.GetComponent<Building>();
}
// public class ProcessessorBuildingType : BuildingType {
    
// }
[System.Serializable]
public class BuildingRecipe {
    public ItemStack[] requiredItems;
    // required tier?
    // public BuildingType buildingType;
}
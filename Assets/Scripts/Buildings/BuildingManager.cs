using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager> {

    public GameObject[] buildingPrefabs;
    public IEnumerable<Building> buildingTypes => buildingPrefabs.Select(bp => bp.GetComponent<Building>());

    public Building GetBuildingTypeForBuilding(Building building) {
        // note be careful to seperate building types from buildings, building types are the prefabs
        return buildingTypes.FirstOrDefault(b => building.typeIndex == b.typeIndex);
    }
    public GameObject GetPrefabForBuildingType(Building buildingType) {
        return buildingType.gameObject;
    }

    [ContextMenu("Find Types")]
    void AutoFindAllNodeTypePrefabs() {
#if UNITY_EDITOR
        // Find all Gameobjects that have 'co' in their filename, that are labelled with 'architecture' and are placed in 'MyAwesomeProps' folder
        const string folder = "Assets/Prefabs/Buildings";
        string[] guids2 = UnityEditor.AssetDatabase.FindAssets("", new[] { folder });
        List<GameObject> loadAssets = new List<GameObject>();
        foreach (string guid2 in guids2) {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid2);
            // Debug.Log("Loading " + path);
            loadAssets.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }
        buildingPrefabs = loadAssets.Where(p => p != null).ToArray();
        for (int i = 0; i < buildingPrefabs.Length; i++) {
            GameObject buildingPrefab = buildingPrefabs[i];
            buildingPrefab.GetComponent<Building>().typeIndex = i;
        }
#endif
    }

}
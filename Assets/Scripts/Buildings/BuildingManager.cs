using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager> {

    // public GameObject[] buildingPrefabs;
    public BuildingType[] buildingTypes;

    [ContextMenu("Find Types")]
    void AutoFindAllNodeTypePrefabs() {
#if UNITY_EDITOR
        // Find all Gameobjects that have 'co' in their filename, that are labelled with 'architecture' and are placed in 'MyAwesomeProps' folder
        const string folder = "Assets/Data/Buildings";
        string[] guids2 = UnityEditor.AssetDatabase.FindAssets("", new[] { folder });
        List<BuildingType> loadAssets = new List<BuildingType>();
        foreach (string guid2 in guids2) {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid2);
            // Debug.Log("Loading " + path);
            loadAssets.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<BuildingType>(path));
        }
        buildingTypes = loadAssets.Where(p => p != null).ToArray();
#endif
    }

}
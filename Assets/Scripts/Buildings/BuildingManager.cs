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
    void FindPrefabs() {
        buildingPrefabs = AssetHelper.AutoFindAllAssets<GameObject>("Assets/Prefabs/Buildings");
        for (int i = 0; i < buildingPrefabs.Length; i++) {
            GameObject buildingPrefab = buildingPrefabs[i];
            buildingPrefab.GetComponent<Building>().typeIndex = i;
        }
    }
}
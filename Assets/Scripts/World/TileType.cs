using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "TileType", menuName = "HP/TileType", order = 0)]
public class TileType : ScriptableObject {

    public bool blocksBuildings = true;
    // like for miners
    public BuildingType[] allowedBuildingTypes;
    public Color color;
    public GameObject tilePrefab;

}
using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "TileType", menuName = "HP/TileType", order = 0)]
public class TileType : UnityEngine.Tilemaps.Tile {

    [Space]
    // public bool overrideBaseColor = false;
    public Material overrideMaterial;
    public bool blocksBuildings = true;
    public bool blocksPlayer = true;
    // like for miners
    public Building[] allowedBuildingTypes;
    // public Color color;
    public GameObject tilePrefab;// additional to existing prefab

//     private void OnDrawGizmos() {
// #if UNITY_EDITOR
//         UnityEditor.Handles.Label(Vector3.zero, name);
// #endif
//     }

}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour {

    public Vector2Int mapPos = Vector2Int.zero;

    [SerializeField]
    private TileType _groundTileType;
    public GameObject groundTile;
    public Building building;

    public bool HasBuilding => building != null;

    public TileType groundTileType { get => _groundTileType; protected set => _groundTileType = value; }

    private void Awake() {

    }
    public void SetGroundTile(TileType tileType) {
        _groundTileType = tileType;
        // todo fix go
    }
    public bool CanPlaceBuilding(Building building) {
        return !HasBuilding 
            && (!groundTileType.blocksBuildings || groundTileType.allowedBuildingTypes.Contains(building));
    }
    public void PlaceBuilding(Building building) {
        this.building = building;
        building.tile = this;
        building.OnPlaced();
    }
    public void RemoveBuilding() {
        if (!HasBuilding) return;
        building.OnRemoved();
        building.tile = null;
        if (Application.isPlaying) {
            Destroy(building.gameObject);
        } else {
            DestroyImmediate(building.gameObject);
        }
    }
}
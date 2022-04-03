using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour {

    [SerializeField, ReadOnly]
    private Vector2Int _mapPos = Vector2Int.zero;
    [SerializeField, ReadOnly]
    private TileType _groundTileType;
    [SerializeField, ReadOnly]
    private Building _building;

    // public GameObject topTile;

    public bool HasBuilding => building != null;

    public TileType groundTileType { get => _groundTileType; protected set => _groundTileType = value; }
    public Vector2Int mapPos { get => _mapPos; protected set => _mapPos = value; }
    public Building building { get => _building; protected set => _building = value; }

    private void Awake() {

    }
    [System.Serializable]
    public struct TileInitArgs {
        public TileType tileType;
        public bool hasTileCollision;
        public Vector2Int mapPos;
    }
    public void Init(TileInitArgs args) {
        mapPos = args.mapPos;
        GetComponent<Collider>().enabled = args.hasTileCollision;
        SetGroundTile(args.tileType);
    }
    public void SetGroundTile(TileType tileType) {
        _groundTileType = tileType;
        // todo fix go
    }
    public bool CanPlaceBuilding(Building buildingType) {
        if (HasBuilding) return false;
        // tile type accepts buildings
        if (groundTileType.blocksBuildings && !groundTileType.allowedBuildingTypes.Contains(buildingType)) return false;
        // building accepts tile
        if ((buildingType.mustBePlacedOnTileTypes.Length > 0 && !buildingType.mustBePlacedOnTileTypes.Contains(groundTileType))) return false;
        // player is not standing here
        if (WorldManager.Instance.WorldPosToTilePos(GameManager.Instance.player.transform.position) == mapPos) return false;

        // can place
        return true;
    }
    public void PlaceBuilding(Building building) {
        this.building = building;
        building.tile = this;
        building.OnPlaced();
    }
    public void RemoveBuilding() {
        // Debug.Log("Rem " + name + " " + HasBuilding);
        if (!HasBuilding) return;
        building.OnRemoved();
        building.tile = null;
        building = null;
    }
    public void DeleteBuilding() {
        if (!HasBuilding) return;
        RemoveBuilding();
        if (Application.isPlaying) {
            Destroy(building.gameObject);
        } else {
            DestroyImmediate(building.gameObject);
        }
    }
}
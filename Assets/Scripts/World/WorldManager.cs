using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : Singleton<WorldManager> {

    [System.Serializable]
    public struct PlaceBuildingData {
        public Building buildingType;
        public Vector2Int coords;
    }

    public RectInt bounds;
    [SerializeField] bool hideTiles = true;
    [SerializeField] float tileSize = 2;
    [SerializeReference] GameObject tilePrefab;
    [SerializeField] Tilemap sourceMap;
    // [SerializeField] int extraTilesBorder;
    // [SerializeField] TileType extraTilesType;
    [SerializeField]
    PlaceBuildingData[] placeBuildingDataOnStarts = new PlaceBuildingData[0];

    [SerializeField][HideInInspector] Tile[] tiles;

    protected override void Awake() {
        CreateTiles();
    }
    private void Start() {
        foreach (var placeBuilding in placeBuildingDataOnStarts) {
            Tile tile = GetTileAt(placeBuilding.coords);
            if (tile != null && placeBuilding.buildingType != null) {
                var bgo = Instantiate(BuildingManager.Instance.GetPrefabForBuildingType(placeBuilding.buildingType), transform);
                bgo.transform.position = tile.transform.position;
                Building building = bgo.GetComponent<Building>();
                tile.PlaceBuilding(building);
            } else {
                Debug.LogWarning($"Cant place {placeBuilding.buildingType} at {placeBuilding.coords}");
            }
        }
    }

    [ContextMenu("CenterBounds")]
    void CenterBounds() {
        bounds.x = -bounds.width / 2;
        bounds.y = -bounds.height / 2;
    }
    [ContextMenu("Clear tiles")]
    void ClearTiles() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) {
                Destroy(transform.GetChild(i).gameObject);
            } else {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
    [ContextMenu("ReCreate tiles")]
    void CreateTiles() {
        // fill bounds with new tiles
        ClearTiles();
        List<Tile> tilelist = new List<Tile>();
        for (int y = 0; y < bounds.height; y++) {
            for (int x = 0; x < bounds.width; x++) {
                Vector2Int tilePos = new Vector2Int(x, y) + bounds.min;
                Vector3Int sourcePos = new Vector3Int(tilePos.x, tilePos.y);
                if (!sourceMap.HasTile(sourcePos)) {
                    Debug.LogWarning("no tile for " + sourcePos);
                    continue;
                }
                var tileType = sourceMap.GetTile<TileType>(sourcePos);
                if (tileType == null) {
                    Debug.LogWarning("invalid tile for " + sourcePos);
                    break;
                }
                Tile tile = MakeTile(tilePos, tileType);
                tilelist.Add(tile);
            }
        }
        tiles = tilelist.ToArray();
    }

    private Tile MakeTile(Vector2Int tilePos, TileType tileType) {
        GameObject ntile = Instantiate(tilePrefab, transform);
        ntile.hideFlags = hideTiles ? HideFlags.HideAndDontSave : HideFlags.DontSave;
        if (tileType.tilePrefab != null) {
            GameObject tileTop = Instantiate(tileType.tilePrefab, ntile.transform);
            tileTop.transform.localPosition = Vector3.zero;
            tileTop.hideFlags = hideTiles ? HideFlags.HideAndDontSave : HideFlags.DontSave;
        }
        if (tileType.overrideMaterial != null) {
            ntile.GetComponentInChildren<Renderer>().sharedMaterial = tileType.overrideMaterial;
        }

        ntile.name = "Tile " + tilePos;
        ntile.transform.position = TilePosToWorldPos(tilePos);
        Tile tile = ntile.GetComponent<Tile>();
        tile.Init(new Tile.TileInitArgs() {
            mapPos = tilePos,
            hasTileCollision = tileType.blocksPlayer,
            tileType = tileType,
        });
        return tile;
    }
    public void UpdateTileType(Vector2Int tilePos, TileType tileType) {
        Tile tile = GetTileAt(tilePos);
        if (tile == null) {
            Debug.LogWarning("Invalid tilepos " + tilePos);
            return;
        }
        // todo dont just delete and remake
        // or at least save some data
        var b = tile.building;
        if (b != null) {
            tile.RemoveBuilding();
        }
        Destroy(tile.gameObject);
        if (tileType == null) {
            return;
        }
        Tile ntile = MakeTile(tilePos, tileType);
        tiles[GetIndx(tilePos)] = ntile;
        if (b != null) {
            ntile.PlaceBuilding(b);
            b.tile = ntile;
        }
    }

    public Vector2Int WorldPosToTilePos(Vector3 worldPos) {
        Vector3 pos = transform.InverseTransformPoint(worldPos);
        pos /= tileSize;
        Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        return tilePos;
    }
    public Vector3 TilePosToWorldPos(Vector2Int tilePos) {
        Vector3 pos = new Vector3(tilePos.x, 0, tilePos.y);
        pos *= tileSize;
        return transform.TransformPoint(pos);
    }
    public Tile GetTileAt(Vector3 worldPos) {
        Vector2Int tilePos = WorldPosToTilePos(worldPos);
        if (tilePos == null) return null;
        return GetTileAt(tilePos);
    }
    public Tile GetTileAt(Vector2Int tilePos) {
        if (tilePos.x < bounds.xMin || tilePos.x >= bounds.xMax ||
            tilePos.y < bounds.yMin || tilePos.y >= bounds.yMax) {
            // invalid position
            return null;
        }
        tilePos -= bounds.min;
        int indx = GetIndx(tilePos);
        // Debug.Log($"i{indx} {tilePos} {tiles.Length} {bounds}");
        return tiles[indx];
    }

    private int GetIndx(Vector2Int tilePos) {
        return tilePos.x + (bounds.width) * tilePos.y;
    }

    private void OnDrawGizmos() {
        Vector3 offset = Vector3.up * 1;
        Vector3 min = TilePosToWorldPos(bounds.min);
        Vector3 max = TilePosToWorldPos(bounds.max);
        Vector3 upleft = TilePosToWorldPos(bounds.min + bounds.height * Vector2Int.up);
        Vector3 bottomRight = TilePosToWorldPos(bounds.min + bounds.width * Vector2Int.right);
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.DrawLine(min + offset, upleft + offset, 2);
        UnityEditor.Handles.DrawLine(min + offset, bottomRight + offset, 2);
        UnityEditor.Handles.DrawLine(max + offset, upleft + offset, 2);
        UnityEditor.Handles.DrawLine(max + offset, bottomRight + offset, 2);
#endif
    }
}
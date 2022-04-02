using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager> {

    public RectInt bounds;
    [SerializeField] float tileSize = 2;
    [SerializeReference] GameObject tilePrefab;
    [SerializeField] int extraTilesBorder;
    [SerializeField] TileType extraTilesType;

    [SerializeField][HideInInspector] Tile[] tiles;

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
                GameObject ntile = Instantiate(tilePrefab, transform);
                Vector2Int tilePos = new Vector2Int(x, y) + bounds.min;
                ntile.name = "Tile " + tilePos;
                ntile.transform.position = TilePosToWorldPos(tilePos);
                Tile tile = ntile.GetComponent<Tile>();
                tile.mapPos = tilePos;
                tilelist.Add(tile);
            }
        }
        tiles = tilelist.ToArray();
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
        int indx = tilePos.x + (bounds.width) * tilePos.y;
        // Debug.Log($"i{indx} {tilePos} {tiles.Length} {bounds}");
        return tiles[indx];
    }

}
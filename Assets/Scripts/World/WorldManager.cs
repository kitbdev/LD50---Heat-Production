using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager> {

    public RectInt bounds;
    [SerializeField] float tileSize = 2;
    [SerializeReference] GameObject tilePrefab;

    Tile[] tiles;

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
                ntile.transform.position = TilePosToWorldPos(new Vector2Int(x, y));
                Tile tile = ntile.GetComponent<Tile>();
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
        return GetTileAt(tilePos);
    }
    public Tile GetTileAt(Vector2Int tilePos) {
        if (tilePos.x < bounds.xMin || tilePos.x > bounds.xMax ||
            tilePos.y < bounds.yMin || tilePos.y > bounds.yMax) {
            // invalid position
            return null;
        }
        int indx = tilePos.x + bounds.width * tilePos.y;
        return tiles[indx];
    }

}

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SourceTile", menuName = "HP/SourceTile", order = 0)]
public class SourceTile : UnityEngine.Tilemaps.Tile {

    // todo merge together or something
    public TileType tileType;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);
        // tileData.color = tileType.color;
    }
}
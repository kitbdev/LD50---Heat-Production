using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Vector2Int mapPos = Vector2Int.zero;

    public TileTypeSO groundTileType;
    public GameObject groundTile;
    public Building building;

    public bool HasBuilding => building != null;

    private void Awake() {

    }
    public void SetGroundTile() {

    }
    // public void AddBuilding(){

    // }
}
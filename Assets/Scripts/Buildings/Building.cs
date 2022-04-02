using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Building : MonoBehaviour {

    public BuildingType buildingType;

    [SerializeField] bool isPlaced = false;
    // public Direction dir;
    // public Quaternion dir;
    [SerializeField] TMPro.TMP_Text labelName;

    Tile tile;

    public IEnumerable<Vector2Int> occupiedSpaces => tile == null ? buildingType.localOccupiedSpaces :
        buildingType.localOccupiedSpaces.Select(los => LocalRelPosToTilePos(los));

    public Vector2Int LocalRelPosToTilePos(Vector2Int localPos) {
        Vector3 rotated = transform.localRotation * new Vector3(localPos.x, 0, localPos.y);
        Vector2Int rotedpos = new Vector2Int((int)rotated.x, (int)rotated.z);
        return tile.mapPos + rotedpos;
    }

    protected virtual void Awake() {
        labelName.text = name;
    }
    public virtual void OnPlaced(){
        // todo stop looking like a ghost?
        isPlaced = true;
        // todo anim
    }
    public virtual void OnRemoved(){
        isPlaced = false;
    }

}
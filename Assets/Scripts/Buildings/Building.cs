using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    public Vector2Int[] localOccupiedSpaces;
    // public Direction dir;
    // public Quaternion dir;
    [SerializeField] TMPro.TMP_Text labelName;

    Tile tile;

    public IEnumerable<Vector2Int> occupiedSpaces => tile == null ? localOccupiedSpaces :
        localOccupiedSpaces.Select(los => {
            Vector3 rotated = transform.localRotation * new Vector3(los.x, 0, los.y);
            Vector2Int rotedpos = new Vector2Int((int)rotated.x, (int)rotated.z);
            return tile.mapPos + rotedpos;
        });

    protected virtual void Awake() {
        labelName.text = name;
    }
}
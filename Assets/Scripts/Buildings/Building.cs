using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Building : MonoBehaviour {

    [System.Serializable]
    public class BuildingRecipe {
        public ItemStack[] requiredItems;
        // required tier?
        // public BuildingType buildingType;
    }

    // public BuildingType buildingType;
    [ReadOnly] public int typeIndex;

    public Vector2Int[] localOccupiedSpaces;
    public BuildingRecipe buildingRecipe;

    // local
    [Space]
    [SerializeField] TMPro.TMP_Text labelName;

    // [Header("Runtime")]
    [SerializeField] bool isPlaced = false;
    [ReadOnly] public Tile tile;

    public IEnumerable<Vector2Int> occupiedSpaces => tile == null ? localOccupiedSpaces :
        localOccupiedSpaces.Append(Vector2Int.zero).Distinct().Select(los => LocalRelPosToTilePos(los));

    public Vector2Int LocalRelPosToTilePos(Vector2Int localPos) {
        Vector3 rotated = transform.localRotation * new Vector3(localPos.x, 0, localPos.y);
        Vector2Int rotedpos = new Vector2Int((int)rotated.x, (int)rotated.z);
        return tile.mapPos + rotedpos;
    }

    private void OnValidate() {
        SetLabelName();
    }
    protected virtual void Awake() {
        // NewMethod();
    }

    [ContextMenu("SetName")]
    private void SetLabelName() {
        labelName.text = name;
    }

    public virtual void OnPlaced() {
        // todo stop looking like a ghost?
        isPlaced = true;
        // todo anim
    }
    public virtual void OnRemoved() {
        isPlaced = false;
    }

}
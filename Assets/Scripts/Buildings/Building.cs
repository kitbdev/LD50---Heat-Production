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
    public QuickOutline quickOutline;

    // [Header("Runtime")]
    [SerializeField] bool isPlaced = false;
    [ReadOnly] public Tile tile;

    public IEnumerable<Vector2Int> occupiedSpaces => tile == null ? localOccupiedSpaces :
        localOccupiedSpaces.Append(Vector2Int.zero).Distinct().Select(los => LocalRelPosToTilePos(los));

    public bool IsPlaced { get => isPlaced; protected set => isPlaced = value; }

    public Vector2Int LocalRelPosToTilePos(Vector2Int localPos) {
        Vector3 rotated = transform.localRotation * new Vector3(localPos.x, 0, localPos.y);
        Vector2Int rotedpos = new Vector2Int((int)rotated.x, (int)rotated.z);
        return tile.mapPos + rotedpos;
    }


    [ContextMenu("SetName")]
    protected void SetLabelName() {
        labelName.text = name;
    }

    private void OnValidate() {
        SetLabelName();
    }
    protected virtual void Awake() {
        // NewMethod();
    }
    // private void Start() {
    //     OnPlaced();
    // }
    protected virtual void OnEnable() {
        if (IsPlaced) {
            OnPlaced();
        }
    }
    protected virtual void OnDisable() {
        // for hot reloading 
        if (IsPlaced) {
            OnRemoved();
            IsPlaced = true;
        }
    }

    public virtual void OnPlaced() {
        // todo stop looking like a ghost?
        IsPlaced = true;
        // todo anim
    }
    public virtual void OnRemoved() {
        IsPlaced = false;
    }

}
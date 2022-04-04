using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Building : MonoBehaviour {

    [System.Serializable]
    public class BuildingRecipe {
        public ItemStack[] requiredItems;
        // required tier?
        // public BuildingType buildingType;
    }

    // type data
    [Header("Building")]
    // public BuildingType buildingType;
    [ReadOnly] public int typeIndex;
    public int sortOrder = 0;

    public Vector2Int[] localOccupiedSpaces;
    public BuildingRecipe buildingRecipe;
    public TileType[] mustBePlacedOnTileTypes = new TileType[0];
    public Sprite icon;

    // local
    [Space]
    [SerializeField] TMPro.TMP_Text labelName;
    public QuickOutline quickOutline;
    protected Animator animator;
    [SerializeField] GameObject previewOrSelectOnlyGo;

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
        if (!Application.isPlaying) {
            // for prefabs
            SetLabelName();
        }
    }
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        // NewMethod();
        if (previewOrSelectOnlyGo != null) previewOrSelectOnlyGo.SetActive(true);
    }
    // private void Start() {//debug
    //     OnPlaced();
    // }
    protected virtual void OnEnable() {
        if (IsPlaced) {
            OnPlaced();
        }
        OnUnHover();
    }
    protected virtual void OnDisable() {
        // for hot reloading 
        if (IsPlaced) {
            OnRemoved();
            IsPlaced = true;
        }
    }

    public virtual void OnPlaced() {
        IsPlaced = true;
        //? stop looking like a ghost?
        if (previewOrSelectOnlyGo != null) previewOrSelectOnlyGo.SetActive(false);
    }
    public virtual void OnRemoved() {
        IsPlaced = false;
        if (previewOrSelectOnlyGo != null) previewOrSelectOnlyGo.SetActive(true);
    }
    public virtual void OnNeighborUpdated() {
        
    }

    public void OnHover() {
        labelName.transform.parent.gameObject.SetActive(true);
        if (previewOrSelectOnlyGo != null) previewOrSelectOnlyGo.SetActive(true);
    }
    public void OnUnHover() {
        labelName.transform.parent.gameObject.SetActive(false);
        if (previewOrSelectOnlyGo != null && isPlaced) previewOrSelectOnlyGo.SetActive(false);
    }


    // for UI

    public virtual bool HasBuildingScreen => true;
    public virtual Inventory GetFirstInv() {
        return null;
    }
    public virtual Inventory GetSecondInv() {
        return null;
    }
    public virtual string GetDescription() {
        return "";
    }
    public virtual bool CanTakeFromFirst => false;
    public virtual bool CanTakeFromSecond => false;
    public virtual bool CanPutInFirst => false;
    public virtual bool CanPutInSecond => false;
    public virtual UnityEvent<float> sliderUpdateAction => null;

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo inserter chain
// public class InserterBuilding : Building, IAccecptsItem, IHoldsItem {
public class InserterBuilding : Building {

    [Header("Inserter")]
    public float speed = 1;
    public Vector2Int fromBuildingLPos;
    public Vector2Int toBuildingLPos;

    public Transform grabber;
    [SerializeField, ReadOnly] DroppedItem heldItem;

    Timer processTimer;


    private IHoldsItem GetHoldsItem(Vector2Int localPos) {
        Building building = WorldManager.Instance.GetTileAt(LocalRelPosToTilePos(localPos))?.building;
        if (building is IHoldsItem holdsItem) {
            return holdsItem;
        }
        return null;
    }
    private IAccecptsItem GetAcceptsItem(Vector2Int localPos) {
        Building building = WorldManager.Instance.GetTileAt(LocalRelPosToTilePos(localPos))?.building;
        if (building is IAccecptsItem accItem) {
            return accItem;
        }
        return null;
    }

    public IHoldsItem GetFromBuilding() {
        return GetHoldsItem(fromBuildingLPos);
    }
    public IAccecptsItem GetToBuilding() {
        return GetAcceptsItem(toBuildingLPos);
    }


    protected override void Awake() {
        base.Awake();
        processTimer = GetComponent<Timer>();
    }

    public override void OnPlaced() {
        base.OnPlaced();
        // todo use anim instead
        processTimer.onTimerComplete.AddListener(StartMovingItem);
        processTimer.StartTimer();
    }
    public override void OnRemoved() {
        base.OnRemoved();
        processTimer.onTimerComplete.RemoveListener(StartMovingItem);
        processTimer.StopTimer();
    }
    void StartMovingItem() {
        // Debug.Log("try grab " + fromBuildingLPos + " " + LocalRelPosToTilePos(fromBuildingLPos));
        IHoldsItem frombuilding = GetFromBuilding();
        if (frombuilding != null) {
            if (frombuilding.FromInventory.HasAnyItems()) {
                // todo optional filter
                Item item = frombuilding.FromInventory.TakeFirstItem();
                heldItem = ItemManager.Instance.DropItem(item);
                heldItem.transform.parent = grabber;
                heldItem.transform.position = Vector3.zero;
                heldItem.transform.rotation = Quaternion.identity;
                // Debug.Log("Grabbed");
            }
        }
        FinishMovingItem();
    }
    void FinishMovingItem() {
        if (heldItem == null) return;
        IAccecptsItem tobuilding = GetToBuilding();
        if (tobuilding != null) {
            if (tobuilding.ToInventory.HasSpaceFor(heldItem.item.itemType)) {
                // Debug.Log("Placed");
                tobuilding.ToInventory.AddItem(heldItem.item.itemType);
                Destroy(heldItem.gameObject);
                heldItem = null;
                animator.SetTrigger("Insert");
            }
        }
    }


    public override bool HasBuildingScreen => false;
}
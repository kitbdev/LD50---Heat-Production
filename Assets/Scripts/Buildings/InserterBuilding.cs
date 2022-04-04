using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class InserterBuilding : Building, IAccecptsItem, IHoldsItem {
public class InserterBuilding : Building, IHoldsItem {

    [Header("Inserter")]
    public float grabDur = 1;
    public float placeDur = 1;
    public Vector2Int fromBuildingLPos;
    public Vector2Int toBuildingLPos;

    public Transform grabber;
    [SerializeField, ReadOnly] DroppedItem heldItem;

    Timer processTimer;
    [SerializeField] Inventory heldInventory;
    [SerializeField, ReadOnly] Inventory fromInv = null;

    public Inventory FromInventory => heldInventory;


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
        processTimer.onTimerComplete.AddListener(TimerEvent);
        heldInventory.OnInventoryUpdateEvent.AddListener(HeldInvUpdate);
        processTimer.StartTimer();
        UpdateFromInv();
    }
    public override void OnRemoved() {
        base.OnRemoved();
        processTimer.onTimerComplete.RemoveListener(TimerEvent);
        heldInventory.OnInventoryUpdateEvent.RemoveListener(HeldInvUpdate);
        processTimer.StopTimer();
    }
    public override void OnNeighborUpdated() {
        base.OnNeighborUpdated();
        UpdateFromInv();
        if (heldItem != null) {
            processTimer.ResumeTimer();
            // FinishMovingItem();
        }
    }
    void UpdateFromInv() {
        if (fromInv != null) {
            fromInv.OnInventoryUpdateEvent.RemoveListener(FromInvUpdate);
        }
        fromInv = GetFromBuilding()?.FromInventory;
        if (fromInv != null) {
            fromInv.OnInventoryUpdateEvent.AddListener(FromInvUpdate);
        }
    }
    void FromInvUpdate() {
        if (heldItem == null) {
            // processTimer.ResumeTimer();
            StartMovingItem();
        }
    }
    void HeldInvUpdate() {
        // remove dropped item if empty
        if (!heldInventory.HasAnyItems()) {
            // out item was taken
            if (heldItem != null) {
                Destroy(heldItem.gameObject);
                heldItem = null;
            }
            animator.SetBool("Grabbed", false);

            processTimer.duration = grabDur;
            processTimer.ResumeTimer();
        }
    }
    void TimerEvent() {
        if (heldItem != null) {
            FinishMovingItem();
        } else {
            StartMovingItem();
        }
    }
    void StartMovingItem() {
        if (heldItem != null) return;
        // Debug.Log("try grab " + fromBuildingLPos + " " + LocalRelPosToTilePos(fromBuildingLPos));
        if (fromInv != null && fromInv.HasAnyItems()) {
            // todo optional filter
            // Debug.Log($"taking '{fromInv}' from {fromInv.name}");
            Item item = fromInv.TakeFirstItem();
            // Debug.Log($"taking '{item}' from {fromInv.name}");
            heldItem = ItemManager.Instance.DropItem(item);
            heldItem.transform.parent = grabber;
            heldItem.transform.position = Vector3.zero;
            heldItem.transform.rotation = Quaternion.identity;
            // Debug.Log("Grabbed");
            processTimer.duration = placeDur;
            processTimer.StartTimer();
            // processTimer.ResumeTimer();
            animator.SetBool("Grabbed", true);
        }
        // FinishMovingItem();
    }
    void FinishMovingItem() {
        if (heldItem == null) return;
        IAccecptsItem tobuilding = GetToBuilding();
        if (tobuilding != null) {
            if (tobuilding.ToInventory.HasSpaceFor(heldItem.item.itemType)) {
                // Debug.Log("Placed");
                tobuilding.ToInventory.AddItem(heldItem.item.itemType);
                if (heldInventory.HasItemAtLeast(heldItem.item.itemType, 1)) {
                    heldInventory.TakeFirstItem();
                }
                Destroy(heldItem.gameObject);
                heldItem = null;
                processTimer.duration = grabDur;
                processTimer.StartTimer();
                animator.SetBool("Grabbed", false);
                // animator.SetTrigger("Insert");
                return;
            }
        }
        // no building to put in, hold in inventory
        heldInventory.AddItem(heldItem.item.itemType);
    }


    public override bool HasBuildingScreen => false;
}
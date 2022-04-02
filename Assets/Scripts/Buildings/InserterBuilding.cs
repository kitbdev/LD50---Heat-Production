using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InserterBuilding : Building {

    [Header("Inserter")]
    public float speed = 1;
    public Vector2Int fromBuildingLPos;
    public Vector2Int toBuildingLPos;

    public Transform grabber;

    public IHoldsItem fromBuilding {
        get {
            Building building = WorldManager.Instance.GetTileAt(LocalRelPosToTilePos(fromBuildingLPos))?.building;
            if (building is IHoldsItem holdsItem) {
                return holdsItem;
            }
            return null;
        }
    }
    public IHoldsItem toBuilding {
        get {
            Building building = WorldManager.Instance.GetTileAt(LocalRelPosToTilePos(toBuildingLPos))?.building;
            if (building is IHoldsItem holdsItem) {
                return holdsItem;
            }
            return null;
        }
    }

    DroppedItem heldItem;

    Timer processTimer;
    protected override void Awake() {
        processTimer = GetComponent<Timer>();
    }

    public override void OnPlaced() {
        base.OnPlaced();
        processTimer.onTimerComplete.AddListener(StartMovingItem);
        processTimer.StartTimer();
    }
    public override void OnRemoved() {
        base.OnRemoved();
        processTimer.onTimerComplete.RemoveListener(StartMovingItem);
        processTimer.StopTimer();
    }
    void StartMovingItem() {
        if (fromBuilding != null) {
            if (fromBuilding.Inventory.HasAnyItems()) {
                // todo optional filter
                Item item = fromBuilding.Inventory.TakeFirstItem();
                heldItem = ItemManager.Instance.DropItem(item);
                heldItem.transform.parent = grabber;
                heldItem.transform.position = Vector3.zero;
                heldItem.transform.rotation = Quaternion.identity;
            }
        }
    }
    void FinishMovingItem() {
        if (toBuilding != null) {
            if (toBuilding.Inventory.HasSpaceFor(heldItem.item.itemType)) {
                toBuilding.Inventory.AddItem(heldItem.item);
                Destroy(heldItem.gameObject);
                heldItem = null;
            }
        }
    }
}
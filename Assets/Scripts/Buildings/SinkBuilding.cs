using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SinkBuilding : Building, IAccecptsItem {

    [Header("Sink")]
    public ItemType[] eatableItemTypes;
    public int eatRate;

    Timer processTimer;
    [SerializeField] Inventory inputInventory;

    public UnityEvent onEatItemEvent;

    public Inventory ToInventory => inputInventory;

    protected override void Awake() {
        base.Awake();
        processTimer = GetComponent<Timer>();
    }
    public override void OnPlaced() {
        base.OnPlaced();
        processTimer.onTimerComplete.AddListener(EatItem);
        processTimer.onTimerUpdate.AddListener(UpdateProgressBar);
        inputInventory.OnInventoryUpdateEvent.AddListener(InvUpdate);
        processTimer.StartTimer();
    }
    public override void OnRemoved() {
        processTimer.onTimerComplete.RemoveListener(EatItem);
        processTimer.onTimerUpdate.RemoveListener(UpdateProgressBar);
        inputInventory.OnInventoryUpdateEvent.RemoveListener(InvUpdate);
        processTimer.StopTimer();
        base.OnRemoved();
    }
    void InvUpdate() {
        if (!processTimer.IsRunning) {
            if (inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
                EatItem();
                processTimer.ResumeTimer();
            }
        }
    }
    void EatItem() {
        Debug.Log("eating!");
        if (inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
            // only one slot, so its fine
            inputInventory.TakeFirstItem();
            onEatItemEvent?.Invoke();
        } else {
            // not enough items
            processTimer.StopTimer();
        }
        if (!inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
            // not enough items next time
            processTimer.StopTimer();
        }
    }

    public override bool HasBuildingScreen => true;
    public override Inventory GetFirstInv() {
        return inputInventory;
    }
    public override bool CanTakeFromFirst => true;
    public override bool CanPutInFirst => true;
    
    UnityEvent<float> sliderEvent = new UnityEvent<float>();
    public void UpdateProgressBar(float v) => sliderEvent?.Invoke(v);
    public override UnityEvent<float> sliderUpdateAction => sliderEvent;
}
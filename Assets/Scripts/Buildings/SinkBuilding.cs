using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SinkBuilding : Building, IAccecptsItem {

    [Header("Sink")]
    [SerializeField] ItemType[] eatableItemTypes;
    [SerializeField] int eatRate;
    [SerializeField] bool isHeater = true;
    [SerializeField] float heatRate = 1f;
    [SerializeField] bool needsItems = true;


    [SerializeField] GameObject activeGo;

    Timer processTimer;
    [SerializeField] Inventory inputInventory;

    public UnityEvent onEatItemEvent;

    public Inventory ToInventory => inputInventory;

    [SerializeField, ReadOnly] bool isActive = false;


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
        SetActive(true);
    }
    public override void OnRemoved() {
        processTimer.onTimerComplete.RemoveListener(EatItem);
        processTimer.onTimerUpdate.RemoveListener(UpdateProgressBar);
        inputInventory.OnInventoryUpdateEvent.RemoveListener(InvUpdate);
        processTimer.StopTimer();
        base.OnRemoved();
    }
    bool processing = false;
    void InvUpdate() {
        if (!processTimer.IsRunning) {
            if (!needsItems || inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
                // EatItem();
                SetActive(true);
            }
        }
    }
    void EatItem() {
        // Debug.Log("eating!");
        if (inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
            if (!processing) {
                processing = true;
                // only one slot, so its fine
                Item item = inputInventory.TakeFirstItem();
                onEatItemEvent?.Invoke();
                heatRate = item.itemType.heatRate;
                // if (isHeater) {
                //     HeatManager.Instance.AddHeat(heatRate);
                // }
                processing = false;
            }
        } else if (needsItems) {
            // not enough items
            SetActive(false);
        }
        if (needsItems && !inputInventory.HasAnyItemsOfType(eatableItemTypes)) {
            // not enough items next time
            SetActive(false);
        }
    }
    private void Update() {
        // Debug.Log("upd" + isActive + " " + isHeater);
        if (isActive) {
            if (isHeater) {
                HeatManager.Instance.AddHeat(heatRate * Time.deltaTime);
            }
        }
    }
    void SetActive(bool active) {
        isActive = active;
        if (activeGo != null) activeGo.SetActive(isActive);
        if (isActive) {
            processTimer.ResumeTimer();
        } else {
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
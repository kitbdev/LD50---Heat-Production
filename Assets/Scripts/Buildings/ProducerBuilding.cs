using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProducerBuilding : Building, IHoldsItem {

    // [Header("Producer")]
    // public int productionRate;
    public ItemType productionItem => tile?.groundTileType.produces;

    Timer processTimer;
    Inventory inventory;

    public Inventory FromInventory => inventory;

    protected override void Awake() {
        base.Awake();
        processTimer = GetComponent<Timer>();
        inventory = GetComponent<Inventory>();
    }
    public override void OnPlaced() {
        base.OnPlaced();
        processTimer.onTimerComplete.AddListener(ProduceItem);
        processTimer.onTimerUpdate.AddListener(UpdateProgressBar);
        inventory.OnInventoryUpdateEvent.AddListener(InvUpdate);
        audioSource.loop = true;
        processTimer.StartTimer();
        audioSource.Play();
        animator.SetBool("Active", true);
    }
    public override void OnRemoved() {
        processTimer.onTimerComplete.RemoveListener(ProduceItem);
        processTimer.onTimerUpdate.RemoveListener(UpdateProgressBar);
        inventory.OnInventoryUpdateEvent.RemoveListener(InvUpdate);
        processTimer.StopTimer();
        animator.SetBool("Active", false);
        base.OnRemoved();
    }
    void InvUpdate() {
        // Debug.Log("updating "+name+" inv "+inventory);
        if (!processTimer.IsRunning) {
            if (inventory.HasSpaceFor(productionItem)) {
                // ProduceItem();
                if (!processTimer.IsRunning) {
                    audioSource.Play();
                    // audioSource.UnPause();
                }
                processTimer.ResumeTimer();
            }
        }
    }
    void ProduceItem() {
        // Debug.Log("Producing!");
        if (inventory.HasSpaceFor(productionItem)) {
            inventory.AddItem(productionItem);
            animator.SetBool("Active", true);
        } else {
            // is full
            animator.SetBool("Active", false);
            processTimer.PauseTimer();
            audioSource.Pause();
        }
    }

    public override bool HasBuildingScreen => true;
    public override Inventory GetFirstInv() {
        return inventory;
    }
    public override bool CanTakeFromFirst => true;
    public override bool CanPutInFirst => false;

    UnityEvent<float> sliderEvent = new UnityEvent<float>();
    public void UpdateProgressBar(float v) => sliderEvent?.Invoke(v);
    public override UnityEvent<float> sliderUpdateAction => sliderEvent;
}
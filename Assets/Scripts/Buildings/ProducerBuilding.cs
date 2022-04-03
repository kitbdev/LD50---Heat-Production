using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerBuilding : Building, IHoldsItem {

    [Header("Producer")]
    public ItemType productionItem;
    public int productionRate;

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
        processTimer.StartTimer();
        animator.SetBool("Active", true);
    }
    public override void OnRemoved() {
        processTimer.onTimerComplete.RemoveListener(ProduceItem);
        processTimer.StopTimer();
        animator.SetBool("Active", false);
        base.OnRemoved();
    }
    void ProduceItem() {
        // Debug.Log("Producing!");
        if (inventory.HasSpaceFor(productionItem)) {
            inventory.AddItem(new Item(productionItem));
            animator.SetBool("Active", true);
        } else {
            // is full
            animator.SetBool("Active", false);
        }
    }
}
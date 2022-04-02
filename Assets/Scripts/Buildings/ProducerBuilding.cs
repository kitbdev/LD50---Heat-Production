using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerBuilding : Building, IHoldsItem {

    public ItemType productionItem;
    public int productionRate;

    Timer processTimer;
    Inventory inventory;

    public Inventory Inventory => inventory;

    protected override void Awake() {
        processTimer = GetComponent<Timer>();
        inventory = GetComponent<Inventory>();
    }
    public override void OnPlaced() {
        base.OnPlaced();
        processTimer.onTimerComplete.AddListener(ProduceItem);
    }
    public override void OnRemoved() {
        processTimer.onTimerComplete.RemoveListener(ProduceItem);
        base.OnRemoved();
    }
    void ProduceItem() {
        inventory.AddItem(new Item(productionItem));
    }
}
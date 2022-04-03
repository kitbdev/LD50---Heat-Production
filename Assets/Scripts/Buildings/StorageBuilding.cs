using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : Building, IHoldsItem, IAccecptsItem {
    
    [Header("Storage")]
    [SerializeField] Inventory inventory;

    public Inventory FromInventory => inventory;
    public Inventory ToInventory => inventory;



    public override Inventory GetFirstInv() {
        return inventory;
    }
    public override bool BiDirectionalFirstInv => true;
}
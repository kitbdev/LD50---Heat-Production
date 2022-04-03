using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : Building, IHoldsItem, IAccecptsItem {
    
    [Header("Storage")]
    [SerializeField] Inventory inventory;

    public Inventory FromInventory => inventory;
    public Inventory ToInventory => inventory;



    public override bool HasBuildingScreen => true;
    public override Inventory GetFirstInv() {
        return inventory;
    }
    public override bool CanTakeFromFirst => true;
    public override bool CanPutInFirst => true;
    public override bool CanTakeFromSecond => false;
    public override bool CanPutInSecond => false;
}
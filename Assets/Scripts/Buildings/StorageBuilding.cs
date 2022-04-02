using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : Building, IHoldsItem {
    
    [Header("Storage")]
    [SerializeField] Inventory inventory;

    public Inventory Inventory => inventory;
}
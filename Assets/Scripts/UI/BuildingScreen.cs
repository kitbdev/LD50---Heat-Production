using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScreen : MonoBehaviour {

    public Building building;
    [SerializeField] TMPro.TMP_Text nameText;
    [SerializeField] TMPro.TMP_Text descText;
    [SerializeField] ShowInventoryUI showInv;
    [SerializeField] ShowInventoryUI secondShowInv;// for outputs or such
    [SerializeField] ShowInventoryUI playerInv;
    [SerializeField] GameObject defInvGo;

    MenuScreen menuScreen;

    private void Awake() {
        menuScreen = GetComponent<MenuScreen>();
    }

    public void SetShown(bool show) {
        menuScreen.SetShown(show);
        if (defInvGo != null) {
            // hide default inventory
            defInvGo.SetActive(!show);
        }
    }

    public void UpdateUI() {
        Building buildingType = BuildingManager.Instance.GetBuildingTypeForBuilding(building);
        nameText.text = buildingType.name;
        descText.text = building.GetDescription();
        Inventory inventory = building.GetFirstInv();
        if (inventory != null) {
            showInv.SetInv(inventory);
        } else {
            showInv.gameObject.SetActive(false);
        }
        Inventory inventory1 = building.GetSecondInv();
        if (inventory1 != null) {
            secondShowInv.SetInv(inventory1);
        } else {
            secondShowInv.gameObject.SetActive(false);
        }
        playerInv.secondInventory = building.CanPutInFirst ? showInv.Inventory : (building.CanPutInSecond ? secondShowInv.Inventory : null);
        showInv.secondInventory = building.CanTakeFromFirst ? GameManager.Instance.playerInventory : null;
        secondShowInv.secondInventory = building.CanTakeFromSecond ? GameManager.Instance.playerInventory : null;
        // if (building.BiDirectionalFirstInv) {
        //     showInv.secondInventory = GameManager.Instance.playerInventory;
        // }
    }
}
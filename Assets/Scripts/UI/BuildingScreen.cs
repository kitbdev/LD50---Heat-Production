using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingScreen : MonoBehaviour {

    [SerializeField, ReadOnly] Building _building;
    [SerializeField] TMPro.TMP_Text nameText;
    [SerializeField] TMPro.TMP_Text descText;
    [SerializeField] ShowInventoryUI showInv;
    [SerializeField] ShowInventoryUI secondShowInv;// for outputs or such
    [SerializeField] ShowInventoryUI playerInv;
    [SerializeField] GameObject defInvGo;
    [SerializeField] Slider progressSlider;

    MenuScreen menuScreen;

    public Building building {
        get => _building;
        set {
            if (_building?.sliderUpdateAction != null) {
                building.sliderUpdateAction.RemoveListener(UpdateSlider);
            }
            _building = value;
        }
    }

    private void Awake() {
        menuScreen = GetComponent<MenuScreen>();
    }

    public bool IsShown() => menuScreen.isShown;
    public void SetShown(bool show) {
        menuScreen.SetShown(show);
        if (defInvGo != null) {
            // hide default inventory
            defInvGo.SetActive(!show);
        }
    }

    [SerializeField, ReadOnly]Building btype;
    public void UpdateUI() {
        Building buildingType = BuildingManager.Instance.GetBuildingTypeForBuilding(building);
        btype = buildingType;
        nameText.text = buildingType.name;
        // Debug.Log("opening " + buildingType.name);
        descText.text = building.GetDescription();
        Inventory inventory = building.GetFirstInv();
        if (inventory != null) {
            showInv.SetInv(inventory);
            showInv.gameObject.SetActive(true);
        } else {
            showInv.gameObject.SetActive(false);
        }
        Inventory inventory1 = building.GetSecondInv();
        if (inventory1 != null) {
            secondShowInv.SetInv(inventory1);
            secondShowInv.gameObject.SetActive(true);
        } else {
            secondShowInv.gameObject.SetActive(false);
        }
        playerInv.secondInventory = building.CanPutInFirst ? showInv.Inventory : (building.CanPutInSecond ? secondShowInv.Inventory : null);
        showInv.secondInventory = building.CanTakeFromFirst ? GameManager.Instance.playerInventory : null;
        secondShowInv.secondInventory = building.CanTakeFromSecond ? GameManager.Instance.playerInventory : null;

        if (building.sliderUpdateAction != null) {
            progressSlider.gameObject.SetActive(true);
            building.sliderUpdateAction.AddListener(UpdateSlider);
        } else {
            progressSlider.gameObject.SetActive(false);
        }
    }

    private void UpdateSlider(float v) {
        progressSlider.value = v;
    }
}
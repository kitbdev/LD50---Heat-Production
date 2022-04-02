using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingToggleUI : MonoBehaviour {

    [System.Serializable]
    public struct BToggleInitData {
        public ToggleGroup toggleGroup;
        public BuildingType buildingType;
        public BuildInterface buildInterface;
    }

    [SerializeField] TMPro.TMP_Text label;
    [SerializeField] Transform icon;
    [SerializeField] Toggle toggle;

    [SerializeField, ReadOnly] BToggleInitData data;

    private void Reset() {
        toggle = GetComponent<Toggle>();
    }
    private void Awake() {
        toggle ??= GetComponent<Toggle>();
    }
    public void Init(BToggleInitData initData) {
        data = initData;
        toggle.group = initData.toggleGroup;
        label.text = initData.buildingType.name;
        // ?
    }
    public void OnToggled(bool value) {
        if (value) {
            data.buildInterface.StartPlacingBuilding(data.buildingType);
        } else {
            data.buildInterface.StartPlacingBuilding(null);
        }
    }

}
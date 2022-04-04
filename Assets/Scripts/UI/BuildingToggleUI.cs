using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingToggleUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [System.Serializable]
    public struct BToggleInitData {
        public ToggleGroup toggleGroup;
        public Building buildingType;
        public BuildInterface buildInterface;
    }

    [SerializeField] TMPro.TMP_Text label;
    [SerializeField] Transform icon;
    [SerializeField] Toggle toggle;
    [SerializeField] Image image;

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
        image.sprite = initData.buildingType.icon;
    }
    public void OnToggled(bool value) {
        if (value) {
            data.buildInterface.StartPlacingBuilding(data.buildingType);
        } else {
            data.buildInterface.StartPlacingBuilding(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (data.buildInterface != null) {
            data.buildInterface.OnBuildingImageHover(data.buildingType, eventData);
        }
    }
    public void OnPointerExit(PointerEventData eventData) {
        if (data.buildInterface != null) {
            data.buildInterface.OnBuildingImageUnHover(data.buildingType, eventData);
        }
    }
}
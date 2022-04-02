using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingToggleUI : MonoBehaviour {

    [SerializeField] TMPro.TMP_Text label;
    [SerializeField] Transform icon;
    [SerializeField] Toggle toggle;
    
    BToggleInitData toggleInitData;

    private void Reset() {
        toggle = GetComponent<Toggle>();
    }
    private void Awake() {
        toggle ??= GetComponent<Toggle>();
    }
    public struct BToggleInitData {
        public ToggleGroup toggleGroup;
        public GameObject buildingType;
        public BuildInterface buildInterface;
    }
    public void Init(BToggleInitData initData) {
        toggleInitData = initData;
        toggle.group = initData.toggleGroup;
        // todo 
    }
    public void OnToggled(bool value){
        // toggleInitData.buildInterface.
    }

}
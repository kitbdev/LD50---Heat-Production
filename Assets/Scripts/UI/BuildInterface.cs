using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildInterface : MonoBehaviour {

    [SerializeField] GameObject buildingUI;
    [SerializeField] bool isBuilding = false;
    [SerializeField, ReadOnly] int curRotation;
    
    Controls controls;

    private void OnEnable() {
        controls = new Controls();
        controls.Player.BuildMode.Enable();
        controls.Player.BuildMode.performed += c => {
            if (isBuilding) {
                FinishBuilding();
            } else {
                StartBuilding();
            }
        };
        controls.Player.PlaceBuilding.performed += c => {};
        controls.Player.RemoveBuilding.performed += c => {};
        controls.Player.RotateBuilding.performed += c => {
            curRotation += c.ReadValue<float>() > 0 ? 1 : -1;
            if (curRotation > 3) curRotation = 0;
            if (curRotation < 0) curRotation = 3;
        };
        controls.Player.KeepPlacingHold.performed += c => {};
        controls.Player.KeepPlacingToggle.performed += c => {};
    }
    private void OnDisable() {
        controls.Player.BuildMode.Disable();
    }
    void StartBuilding() {
        isBuilding = true;
        // show ui
        // todo
    }
    void FinishBuilding() {
        isBuilding = false;
    }
    void PlaceBuilding() {

    }
}
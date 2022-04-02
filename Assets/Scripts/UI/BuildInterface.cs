using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildInterface : MonoBehaviour {

    [Header("UI")]
    [SerializeField] MenuScreen buildScreen;
    [SerializeField] Transform buildingButtonHolder;
    [SerializeField] GameObject buildingToggleBtnPrefab;
    [SerializeField] UnityEngine.UI.ToggleGroup buildingToggleGroup;


    [Header("Placing")]
    [SerializeField] int previewLayer;
    [SerializeField] Transform cursorT;
    [SerializeField] int ghostSmoothing = 10;
    // [Space]
    [SerializeField, ReadOnly] bool isBuilding = false;
    [SerializeField, ReadOnly] bool isPlacing;
    [SerializeField, ReadOnly] int curRotation;
    [SerializeField, ReadOnly] Vector3 cursorPos = Vector3.zero;

    GameObject placingGhost;
    GameObject placingPrefab;

    [SerializeField] Camera cam;
    Controls controls;
    Plane gamePlane = new Plane(Vector3.up, Vector3.zero);

    private void Awake() {
        cam ??= Camera.main;
        SetUpUI();
    }
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
        controls.Player.PlaceBuilding.performed += c => {
            if (isPlacing) {
                FinishPlacing();
            }
        };
        controls.Player.RemoveBuilding.performed += c => { };
        controls.Player.RotateBuilding.performed += c => {
            curRotation += c.ReadValue<float>() > 0 ? 1 : -1;
            if (curRotation > 3) curRotation = 0;
            if (curRotation < 0) curRotation = 3;
        };
        controls.Player.KeepPlacingHold.performed += c => { };
        controls.Player.KeepPlacingToggle.performed += c => { };
        if (PauseManager.Instance) PauseManager.Instance.pauseEvent.AddListener(OnPauseEvent);
    }
    private void OnDisable() {
        controls.Player.BuildMode.Disable();
        if (PauseManager.Instance) PauseManager.Instance.pauseEvent.RemoveListener(OnPauseEvent);
    }

    void OnPauseEvent() {
        AbortInteraction();
    }

    private void AbortInteraction() {
        if (isPlacing) CancelPlacing();
        // todo cancel eyedropper toggle?
    }

    private void Update() {
        if (isBuilding) {

            // get cursor pos
            // if using mouse
            // todo contoller
            Vector2 mousepos = Mouse.current.position.ReadValue();
            Ray mouseRay = cam.ScreenPointToRay(mousepos);
            if (gamePlane.Raycast(mouseRay, out var dist)) {
                cursorPos = mouseRay.GetPoint(dist);
            }
            // otherwise use last 

            if (isPlacing) {
                UpdatePlacing();
            }
        }
    }
    void ClearUIBtns() {
        for (int i = buildingButtonHolder.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) {
                Destroy(buildingButtonHolder.GetChild(i).gameObject);
            } else {
                DestroyImmediate(buildingButtonHolder.GetChild(i).gameObject);
            }
        }
    }
    void SetUpUI() {
        ClearUIBtns();
        for (int i = 0; i < BuildingManager.Instance.buildingPrefabs.Length; i++) {
            // BuildingManager.Instance.buildingPrefabs[i];
            GameObject buildingBtnGo = Instantiate(buildingToggleBtnPrefab, buildingButtonHolder);
            BuildingToggleUI buildingToggleUI = buildingBtnGo.GetComponent<BuildingToggleUI>();
            buildingToggleUI.Init(new BuildingToggleUI.BToggleInitData() {
                buildInterface = this,
                toggleGroup = buildingToggleGroup,
                // todo
            });
        }
        buildingToggleGroup.SetAllTogglesOff();
    }

    void StartBuilding() {
        isBuilding = true;
        // show ui
        buildScreen?.Show();

    }
    void FinishBuilding() {
        isBuilding = false;
        buildScreen?.Hide();
        if (isPlacing) {
            CancelPlacing();
        }
    }

    public void StartPlacingBuilding(GameObject buildingPrefab) {
        if (!isBuilding) return;
        this.placingPrefab = buildingPrefab;
        if (buildingPrefab != null) {
            StartPlacing();
        }
    }
    void StartPlacing() {
        isPlacing = true;
        placingGhost = Instantiate(placingPrefab, transform);
        // todo disable physics and logic
        // todo make transp
        placingGhost.transform.position = cursorPos;
        placingGhost.layer = previewLayer;
    }
    void FinishPlacing() {
        isPlacing = false;
        Vector3 targetPos = cursorPos;
        // snap to world grid
        Tile tile = WorldManager.Instance.GetTileAt(targetPos);
        if (tile != null) {
            if (tile.HasBuilding) {
                // invalid
                // todo set color validity
            } else {
                targetPos = tile.transform.position;
                Quaternion rotation = GetCurrentRotation();
                placingGhost.transform.position = targetPos;
                placingGhost.transform.rotation = rotation;
                // tile.building = 
                // todo
            }
        }
        placingGhost = null;
    }
    void CancelPlacing() {
        isPlacing = false;
        Destroy(placingGhost.gameObject);
        placingGhost = null;
    }
    void UpdatePlacing() {
        Vector3 targetPos = cursorPos;
        // snap to world grid
        Tile tile = WorldManager.Instance.GetTileAt(targetPos);
        if (tile != null) {
            if (tile.HasBuilding) {
                // invalid
                // todo set color validity
            } else {
                targetPos = tile.transform.position;
            }
        }
        Quaternion rotation = GetCurrentRotation();
        if (ghostSmoothing > 0) {
            targetPos = Vector3.Lerp(placingGhost.transform.position, targetPos, ghostSmoothing * Time.deltaTime);
        }
        placingGhost.transform.position = targetPos;
        placingGhost.transform.rotation = rotation;
    }

    private Quaternion GetCurrentRotation() {
        return Quaternion.Euler(0, curRotation * 90f, 0);
    }
}
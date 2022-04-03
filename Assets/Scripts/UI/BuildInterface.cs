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

    [SerializeField] Color selectedColor;
    [SerializeField] Color validGhostColor;
    [SerializeField] Color invalidGhostColor;

    [Header("Placing")]
    [SerializeField] bool requireResources = true;
    [Layer][SerializeField] int previewLayer;
    [SerializeField] Transform cursorT;
    [SerializeField] TMPro.TMP_Text cursorText;
    [SerializeField] int ghostSmoothingPos = 20;
    [SerializeField] int ghostSmoothingRot = 40;
    // [Space]
    [SerializeField, ReadOnly] bool isPlacing;
    [SerializeField, ReadOnly] GameObject placingGhost;
    [SerializeField, ReadOnly] Building placingGhostBuilding;
    [SerializeField, ReadOnly] Building placingType;
    [SerializeField, ReadOnly] int curRotation;
    [SerializeField, ReadOnly] bool keepPlacingHold = false;
    [SerializeField, ReadOnly] bool keepPlacingToggle = false;

    [Space]
    [SerializeField, ReadOnly] bool isBuilding = false;
    [SerializeField, ReadOnly] Vector3 cursorPos = Vector3.zero;
    [SerializeField, ReadOnly] Tile cursorOverTile;
    int defLayer;


    Vector3 cursorDragCheckPos = Vector3.zero;
    bool dragHeld = false;
    float dragCursorThreshold = 0.1f;

    // for deletion
    [SerializeField, ReadOnly] Building selectedBuilding;

    [SerializeField] Camera cam;
    Controls controls;
    Plane gamePlane = new Plane(Vector3.up, Vector3.zero);

    private void Awake() {
        cam ??= Camera.main;
        SetUpUI();
        if (cursorText) {
            cursorText.transform.parent.gameObject.SetActive(false);
        }
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
        controls.Player.PlaceBuilding.Enable();
        controls.Player.PlaceBuilding.performed += c => {
            if (isBuilding) {
                if (isPlacing) {
                    FinishPlacing();
                } else {
                    // select building
                    if (cursorOverTile != null && cursorOverTile.HasBuilding) {
                        DeselectBuilding();
                        selectedBuilding = cursorOverTile.building;
                        selectedBuilding.quickOutline.enabled = true;
                        selectedBuilding.quickOutline.OutlineColor = selectedColor;
                        // todo show more info
                        cursorDragCheckPos = cursorPos;
                        dragHeld = true;
                    } else {
                        // deselect
                        DeselectBuilding();
                    }
                }
            }
        };
        controls.Player.PlaceBuilding.canceled += c => {
            dragHeld = false;
            // drag release finish placing
            if (isBuilding && isPlacing) {
                FinishPlacing();
            }
        };
        controls.Player.RemoveBuilding.Enable();
        controls.Player.RemoveBuilding.performed += c => { RemoveBuilding(); };
        controls.Player.RotateBuilding.Enable();
        controls.Player.RotateBuilding.performed += c => {
            curRotation += c.ReadValue<float>() > 0 ? 1 : -1;
            if (curRotation > 3) curRotation = 0;
            if (curRotation < 0) curRotation = 3;
        };
        controls.Player.KeepPlacingHold.Enable();
        controls.Player.KeepPlacingHold.performed += c => { keepPlacingHold = true; };
        controls.Player.KeepPlacingHold.canceled += c => { keepPlacingHold = false; };
        controls.Player.KeepPlacingToggle.Enable();
        controls.Player.KeepPlacingToggle.performed += c => { keepPlacingToggle = !keepPlacingToggle; };
        if (PauseManager.Instance) PauseManager.Instance.pauseEvent.AddListener(OnPauseEvent);
    }

    private void DeselectBuilding() {
        if (selectedBuilding != null) {
            selectedBuilding.quickOutline.enabled = false;
            selectedBuilding = null;
        }
    }

    private void OnDisable() {
        controls.Player.BuildMode.Disable();
        controls.Player.PlaceBuilding.Disable();
        controls.Player.RemoveBuilding.Disable();
        controls.Player.RotateBuilding.Disable();
        controls.Player.KeepPlacingHold.Disable();
        controls.Player.KeepPlacingToggle.Disable();
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

        // get cursor pos
        // if using mouse
        // todo contoller
        Vector2 mousepos = Mouse.current.position.ReadValue();
        Ray mouseRay = cam.ScreenPointToRay(mousepos);
        if (gamePlane.Raycast(mouseRay, out var dist)) {
            cursorPos = mouseRay.GetPoint(dist);
            // otherwise use last
        }

        cursorOverTile = WorldManager.Instance.GetTileAt(cursorPos);
        // todo also when not building?
        // get building info
        if (cursorT != null) {
            if (cursorOverTile != null) {
                cursorT.gameObject.SetActive(true);
                cursorT.position = cursorOverTile.transform.position;
            } else {
                cursorT.gameObject.SetActive(false);
            }
        }

        if (isBuilding) {
            // show tile cursor is over
            if (cursorText != null) {
                if (cursorOverTile != null) {
                    cursorText.text = cursorOverTile.groundTileType.name;
                    cursorText.transform.parent.gameObject.SetActive(true);
                    // todo show other info ?
                } else {
                    cursorText.text = "";
                    cursorText.transform.parent.gameObject.SetActive(false);
                }
            }

            if (isPlacing) {
                UpdatePlacing();
            } else {
                if (dragHeld) {
                    if (selectedBuilding != null) {
                        if (Vector3.Distance(cursorPos, cursorDragCheckPos) > dragCursorThreshold) {
                            // start dragging
                            // convert building to ghost and place it
                            StartPlacingFromSelection();
                        }
                    }
                }
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
        foreach (Building buildingType in BuildingManager.Instance.buildingTypes) {
            GameObject buildingBtnGo = Instantiate(buildingToggleBtnPrefab, buildingButtonHolder);
            BuildingToggleUI buildingToggleUI = buildingBtnGo.GetComponent<BuildingToggleUI>();
            buildingToggleUI.Init(new BuildingToggleUI.BToggleInitData() {
                buildInterface = this,
                toggleGroup = buildingToggleGroup,
                buildingType = buildingType,
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

    void RemoveBuilding() {
        if (!isBuilding) return;
        if (isPlacing) {
            CancelPlacing();
            return;
        }
        if (selectedBuilding != null) {
            selectedBuilding.tile.RemoveBuilding();
        }
    }
    void EyeDropperSample() {
        if (!isBuilding) return;
        if (cursorOverTile != null && cursorOverTile.HasBuilding) {
            placingType = BuildingManager.Instance.GetBuildingTypeForBuilding(cursorOverTile.building);
            curRotation = GetRotInt(cursorOverTile.building.transform.rotation);
            StartPlacing();
        }
    }

    public void StartPlacingBuilding(Building buildingType) {
        if (!isBuilding) return;
        // Debug.Log("StartPlacingBuilding " + (buildingType?.name ?? "none"));
        if (isPlacing) {
            CancelPlacing();
        }
        if (buildingType != null) {
            this.placingType = buildingType;
            StartPlacing();
        } else {
            // this.placingType = null;
        }
    }
    void StartPlacingFromSelection() {
        isPlacing = true;
        curRotation = GetRotInt(selectedBuilding.transform.rotation);
        placingGhost = selectedBuilding.gameObject;
        placingGhostBuilding = selectedBuilding;
        placingGhost.layer = previewLayer;
        DeselectBuilding();
        placingGhostBuilding.tile.building = null;
        placingGhostBuilding.OnRemoved();
        placingGhostBuilding.quickOutline.enabled = true;
        // dont need resources here
    }
    void StartPlacing() {
        if (requireResources) {
            bool canBuild = GameManager.Instance.playerInventory.HasItems(placingType.buildingRecipe.requiredItems);
            if (!canBuild) return;
            GameManager.Instance.playerInventory.TakeItems(placingType.buildingRecipe.requiredItems);
        }
        DeselectBuilding();
        isPlacing = true;
        placingGhost = Instantiate(BuildingManager.Instance.GetPrefabForBuildingType(placingType), transform);
        placingGhost.transform.position = cursorPos;
        defLayer = placingGhost.layer;
        placingGhost.layer = previewLayer;
        placingGhostBuilding = placingGhost.GetComponent<Building>();
        placingGhostBuilding.quickOutline.enabled = true;
        //? disable physics and logic
        // should be disabled by default
    }
    void FinishPlacing() {
        // Debug.Log("Finish placing");
        Vector3 targetPos = cursorPos;
        // snap to world grid
        Tile tile = WorldManager.Instance.GetTileAt(targetPos);
        if (tile != null && tile.CanPlaceBuilding(placingType)) {
            targetPos = tile.transform.position;
            Quaternion rotation = GetCurrentRotation();
            placingGhost.transform.position = targetPos;
            placingGhost.transform.rotation = rotation;
            placingGhost.layer = defLayer;
            tile.PlaceBuilding(placingGhostBuilding);
            placingGhostBuilding.quickOutline.enabled = false;
            placingGhost = null;
        } else {
            // invalid
            // ? or just ignore click?
            CancelPlacing();
            return;// dont try to keep placing
        }
        isPlacing = false;
        if (keepPlacingHold || keepPlacingToggle) {
            StartPlacing();
        } else {
            placingType = null;
            placingGhostBuilding = null;
            buildingToggleGroup.SetAllTogglesOff();
        }
    }
    void CancelPlacing() {
        bool wasntplacing = !isPlacing;
        if (wasntplacing) return;
        isPlacing = false;
        buildingToggleGroup.SetAllTogglesOff();// ! this would call self if no isplacing check before
        if (requireResources) {
            // refund
            if (placingType == null) {
                Debug.LogWarning("Cancellng placing with a null placing type!");
                placingType = BuildingManager.Instance.GetBuildingTypeForBuilding(placingGhostBuilding);
            }
            if (GameManager.Instance.playerInventory.HasSpaceFor(placingType.buildingRecipe.requiredItems)) {
                GameManager.Instance.playerInventory.AddItems(placingType.buildingRecipe.requiredItems);
            }
        }
        // todo add what we can and drop the rest?
        // ItemManager.Instance.DropItem()
        // Debug.Log("Cancel placing");
        Destroy(placingGhost.gameObject);
        placingGhost = null;
        placingGhostBuilding = null;
    }
    void UpdatePlacing() {
        Vector3 targetPos = cursorPos;
        // snap to world grid
        if (cursorOverTile != null) {
            if (cursorOverTile.CanPlaceBuilding(placingType)) {
                targetPos = cursorOverTile.transform.position;
                placingGhostBuilding.quickOutline.OutlineColor = validGhostColor;
            } else {
                // invalid
                placingGhostBuilding.quickOutline.OutlineColor = invalidGhostColor;
            }
        }
        Quaternion rotation = GetCurrentRotation();
        if (ghostSmoothingPos > 0) {
            targetPos = Vector3.Lerp(placingGhost.transform.position, targetPos, ghostSmoothingPos * Time.deltaTime);
        }
        placingGhost.transform.position = targetPos;
        if (ghostSmoothingRot > 0) {
            rotation = Quaternion.Lerp(placingGhost.transform.rotation, rotation, ghostSmoothingRot * Time.deltaTime);
        }
        placingGhost.transform.rotation = rotation;
    }

    private Quaternion GetCurrentRotation() {
        return Quaternion.Euler(0, curRotation * 90f, 0);
    }
    private int GetRotInt(Quaternion rot) {
        int turn = Mathf.RoundToInt(rot.eulerAngles.y / 90f);
        while (turn > 3) turn -= 4;
        while (turn < 0) turn += 4;
        return turn;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildInterface : MonoBehaviour {

    [Header("UI")]
    [SerializeField] MenuScreen buildScreen;
    [SerializeField] Transform buildingButtonHolder;
    [SerializeField] GameObject buildingToggleBtnPrefab;
    [SerializeField] UnityEngine.UI.ToggleGroup buildingToggleGroup;
    [SerializeField] BuildingScreen buildingScreen;


    [SerializeField] TMPro.TMP_Text buildingInfoText;
    [SerializeField] GameObject buildingInfoPanel;


    [SerializeField] float tileClickDelay = 0.1f;
    float tileClickedLastTime = 0;

    [SerializeField] Color selectedColor;
    [SerializeField] Color validGhostColor;
    [SerializeField] Color invalidGhostColor;

    [Header("Placing")]
    [SerializeField] bool requireResources = true;
    [Layer][SerializeField] int previewLayer;
    [SerializeField] int ghostSmoothingPos = 20;
    [SerializeField] int ghostSmoothingRot = 40;
    [SerializeField] Transform cursorT;
    [SerializeField] Material cursorMat;
    [SerializeField] TMPro.TMP_Text cursorText;


    [SerializeField] AudioClip placeClip;
    [SerializeField] AudioClip destroyClip;
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


    bool isPlacingRealBulding = false;


    Vector3 cursorDragCheckPos = Vector3.zero;
    bool dragHeld = false;
    float dragCursorThreshold = 0.1f;

    [SerializeField, ReadOnly] Building hoverBuilding = null;
    // for deletion
    [SerializeField, ReadOnly] Building selectedBuilding;

    [SerializeField] Camera cam;
    Controls controls;
    Plane gamePlane = new Plane(Vector3.up, Vector3.zero);
    private AudioSource audioSource;

    private void Awake() {
        cam ??= Camera.main;
        audioSource = GetComponent<AudioSource>();
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
                if (buildingScreen.IsShown()) {
                    HideBuildingScreen();
                } else {
                    StartBuilding();
                }
            }
        };
        controls.Player.PlaceBuilding.Enable();
        controls.Player.PlaceBuilding.performed += c => {
            if (Time.deltaTime == 0) return;
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
            } else {
                // show building screen, if available
                if (cursorOverTile != null) {
                    if (cursorOverTile.HasBuilding) {
                        if (cursorOverTile.building.HasBuildingScreen) {
                            // show it
                            buildingScreen.SetShown(true);
                            buildingScreen.building = cursorOverTile.building;
                            buildingScreen.UpdateUI();
                        }
                    } else {
                        // Debug.Log("tileclick0");
                        if (Time.time >= tileClickedLastTime + tileClickDelay) {
                            // Debug.Log("tileclick1");
                            cursorOverTile.groundTileType.OnClick(cursorOverTile);
                            tileClickedLastTime = Time.time;
                        }
                    }
                }
            }
        };
        controls.Player.PlaceBuilding.canceled += c => {
            // if (Time.deltaTime == 0) return;
            dragHeld = false;
            // drag release finish placing
            if (isBuilding && isPlacing && !(keepPlacingHold || keepPlacingToggle)) {
                FinishPlacing();
            }
        };
        controls.Player.RemoveBuilding.Enable();
        controls.Player.RemoveBuilding.performed += c => {
            if (Time.deltaTime == 0) return;
            DeleteBuilding();
        };
        controls.Player.RotateBuilding.Enable();
        controls.Player.RotateBuilding.performed += c => {
            if (Time.deltaTime == 0) return;
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
        //? select cancel
    }

    public void HideBuildingScreen() {
        buildingScreen.SetShown(false);
    }

    private void Update() {
        if (Time.deltaTime == 0) {
            cursorT?.gameObject.SetActive(false);
            cursorText?.transform.parent.gameObject.SetActive(false);
            return;
        }
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
        // hover
        if (cursorOverTile != null && cursorOverTile.HasBuilding) {
            SetHoverBuilding(cursorOverTile.building);
        } else {
            SetHoverBuilding(null);
        }

        // cursor
        if (cursorT != null) {
            if (cursorOverTile != null) {
                cursorT.gameObject.SetActive(true);
                cursorT.position = cursorOverTile.transform.position;
            } else {
                cursorT.gameObject.SetActive(false);
            }
        }

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

        if (isBuilding) {
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

    private void SetHoverBuilding(Building selbuilding) {
        if (hoverBuilding != null) {
            hoverBuilding.OnUnHover();
        }
        hoverBuilding = selbuilding;
        if (hoverBuilding != null) {
            // Debug.Log("Hove");
            hoverBuilding.OnHover();
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
        buildingInfoPanel.SetActive(false);
    }

    void StartBuilding() {
        isBuilding = true;
        // show ui
        HideBuildingScreen();
        if (buildingScreen != null) {
            buildScreen.Show();
        }
    }
    void FinishBuilding() {
        isBuilding = false;
        DeselectBuilding();
        if (isPlacing) {
            CancelPlacing();
        }
        if (cursorMat != null) cursorMat.color = selectedColor;
        buildScreen?.Hide();
    }
    void DeleteBuilding() {
        if (!isBuilding) return;
        if (isPlacing) {
            CancelPlacing();
            return;
        }
        if (selectedBuilding != null) {
            audioSource.PlayOneShot(destroyClip);
            selectedBuilding.tile.DeleteBuilding();
            TryRefund(selectedBuilding);
        } else if (hoverBuilding != null) {
            audioSource.PlayOneShot(destroyClip);
            hoverBuilding.tile.DeleteBuilding();
            TryRefund(hoverBuilding);
        }
    }

    private void TryRefund(Building building) {
        if (requireResources) {
            // refund
            var remType = BuildingManager.Instance.GetBuildingTypeForBuilding(building);
            // Debug.Log("Refunding for " + building + " " + remType.name + " " + remType.buildingRecipe.requiredItems.Length);
            if (GameManager.Instance.playerInventory.HasSpaceFor(remType.buildingRecipe.requiredItems)) {
                GameManager.Instance.playerInventory.AddItems(remType.buildingRecipe.requiredItems);
            } else {
                Debug.LogWarning("Refund inventory full! items lost");
            }
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
        // Debug.Log("Removing " + placingGhostBuilding.tile, placingGhostBuilding.tile);
        placingGhostBuilding.tile.RemoveBuilding();
        placingGhostBuilding.quickOutline.enabled = true;
        isPlacingRealBulding = true;
        // dont need resources here
    }
    void StartPlacing() {
        // Debug.Log("Start placing");
        if (requireResources) {
            bool canBuild = GameManager.Instance.playerInventory.HasItems(placingType.buildingRecipe.requiredItems);
            if (!canBuild) {
                buildingToggleGroup.SetAllTogglesOff();
                return;
            }
            GameManager.Instance.playerInventory.TakeItems(placingType.buildingRecipe.requiredItems);
        }
        isPlacingRealBulding = false;
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
            audioSource.PlayOneShot(placeClip);
        } else {
            // Debug.Log("Cancelling placing1 ");
            // invalid
            // ? or just ignore click?
            CancelPlacing();
            return;// dont try to keep placing
        }
        isPlacing = false;
        if (keepPlacingHold || keepPlacingToggle) {
            // Debug.Log("keep placing");
            StartPlacing();
        } else {
            // placingType = null;
            placingGhostBuilding = null;
            buildingToggleGroup.SetAllTogglesOff();
            if (cursorMat != null) cursorMat.color = selectedColor;
        }
    }
    void CancelPlacing() {
        bool wasntplacing = !isPlacing;
        if (wasntplacing) return;
        // Debug.Log("Cancelling placing2 ");
        isPlacing = false;
        buildingToggleGroup.SetAllTogglesOff();// ! this would call self if no isplacing check before
        if (isPlacingRealBulding) {
            audioSource.PlayOneShot(destroyClip);
        }
        if (requireResources) {
            if (placingType == null) {
                Debug.LogWarning("Cancellng placing with a null placing type!");
                placingType = BuildingManager.Instance.GetBuildingTypeForBuilding(placingGhostBuilding);
            }
            TryRefund(placingGhostBuilding);
            // refund
            // if (GameManager.Instance.playerInventory.HasSpaceFor(placingType.buildingRecipe.requiredItems)) {
            //     GameManager.Instance.playerInventory.AddItems(placingType.buildingRecipe.requiredItems);
            // }
        }
        // todo add what we can and drop the rest?
        // ItemManager.Instance.DropItem()
        // Debug.Log("Cancel placing");
        Destroy(placingGhost.gameObject);
        placingGhost = null;
        placingGhostBuilding = null;
        if (cursorMat != null) cursorMat.color = selectedColor;
    }
    void UpdatePlacing() {
        Vector3 targetPos = cursorPos;
        // snap to world grid
        if (cursorOverTile != null) {
            targetPos = cursorOverTile.transform.position;
            if (cursorOverTile.CanPlaceBuilding(placingType)) {
                if (cursorMat != null) cursorMat.color = validGhostColor;
                placingGhostBuilding.quickOutline.OutlineColor = validGhostColor;
            } else {
                // invalid
                if (cursorMat != null) cursorMat.color = invalidGhostColor;
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


    [SerializeField, ReadOnly] Building hoverImageBuildingType;

    public void OnBuildingImageHover(Building buildingType, PointerEventData eventData) {
        if (hoverImageBuildingType != null) {
            // 
            Debug.LogWarning("alreadyhovering");
        }
        buildingInfoPanel.SetActive(true);
        // show building requirements
        buildingInfoText.text =
            $"{buildingType.buildingRecipe}";
        hoverImageBuildingType = buildingType;
    }
    public void OnBuildingImageUnHover(Building buildingType, PointerEventData eventData) {
        if (hoverImageBuildingType == buildingType) {
            hoverImageBuildingType = null;
            // unhover
            buildingInfoPanel.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(10)]// after inventory
public class ShowInventoryUI : MonoBehaviour, IPointerMoveHandler {

    [SerializeField] GameObject itemSlotPrefab;
    [SerializeField] Transform itemSlotHolder;
    [SerializeField] TMPro.TMP_Text itemHoverText;
    [Space]
    [SerializeField] Inventory inventory;
    public Inventory secondInventory;
    [SerializeField] InputActionReference shiftMoreButton;

    List<ItemSlotUI> itemSlotUIs = new List<ItemSlotUI>();

    public Inventory Inventory => inventory;

    private void Awake() {
        ClearItemSlots();
        itemHoverText.gameObject.SetActive(false);
    }
    private void OnEnable() {
        // Debug.Log(name + " enable " + inventory, gameObject);
        if (inventory != null) {
            inventory.OnInventoryUpdateEvent.AddListener(UpdateInv);
            CreateItemSlots();
            UpdateInv();
        }
        if (shiftMoreButton != null) shiftMoreButton.action.Enable();
        itemHoverText.gameObject.SetActive(false);
    }
    private void OnDisable() {
        if (inventory != null) {
            inventory.OnInventoryUpdateEvent.RemoveListener(UpdateInv);
        }
        if (shiftMoreButton != null) shiftMoreButton.action.Disable();
    }
    public void SetInv(Inventory inventory) {
        if (inventory != null) {
            inventory.OnInventoryUpdateEvent.RemoveListener(UpdateInv);
        }
        this.inventory = inventory;
        if (inventory != null) {
            inventory.OnInventoryUpdateEvent.AddListener(UpdateInv);
            UpdateInv();
        }
    }

    public void UpdateInv() {
        if (inventory == null) return;
        // Debug.Log($"updating Show Inv {name} {inventory.name} {inventory.numSlots}", gameObject);
        if (inventory.numSlots != itemSlotHolder.childCount) {
            CreateItemSlots();
        }
        UpdateSlots();
    }
    void ClearItemSlots() {
        for (int i = itemSlotHolder.childCount - 1; i >= 0; i--) {
            Destroy(itemSlotHolder.GetChild(i).gameObject);
        }
        itemSlotUIs.Clear();
    }
    void CreateItemSlots() {
        ClearItemSlots();
        for (int i = 0; i < inventory.numSlots; i++) {
            GameObject itemSlotGO = Instantiate(itemSlotPrefab, itemSlotHolder);
            ItemSlotUI itemSlotUI = itemSlotGO.GetComponent<ItemSlotUI>();
            itemSlotUI.Init(this);
            itemSlotUI.UpdateItem(inventory.itemSlots[i].itemStack);
            itemSlotUIs.Add(itemSlotUI);
        }
    }
    void UpdateSlots() {
        for (int i = 0; i < inventory.numSlots; i++) {
            ItemSlotUI itemSlotUI = itemSlotUIs[i];
            if (i >= inventory.itemSlots.Length) {
                Debug.LogWarning("not enough slots! " + i + " " + inventory.itemSlots.Length);
                break;
            }
            itemSlotUI.UpdateItem(inventory.itemSlots[i].itemStack);
        }
    }
    public void Sort() {
        inventory.Sort();
    }
    public void OnItemClicked(int index, PointerEventData eventData) {
        if (inventory == null || secondInventory == null) return;
        // switch that item to another inventory

        if (index < 0 || index >= inventory.itemSlots.Length) {
            Debug.LogWarning("invalid click index " + index + " inv size " + inventory.itemSlots.Length);
            return;
        }

        Inventory.ItemSlot itemSlot = inventory.itemSlots[index];
        if (!itemSlot.itemStack.HasItem) return;

        // if shift is down, transfer whole stack - inverted
        bool shift = shiftMoreButton.action.IsPressed();
        int transferAmount;// = !shift ? itemSlot.itemStack.count : 1;
        if (eventData.button == PointerEventData.InputButton.Right) {
            transferAmount = itemSlot.itemStack.count == 1 ? 1 : itemSlot.itemStack.count / 2;
        } else if (shift) {
            transferAmount = 1;
        } else {
            transferAmount = itemSlot.itemStack.count;
        }
        ItemStack tStack = itemSlot.itemStack.Copy();
        tStack.count = transferAmount;
        inventory.TransferItemsIfCan(secondInventory, tStack);
        inventory.Sort();
        secondInventory.Sort();
        eventData.Use();
    }
    public void OnHovered(int index) {
        ItemStack itemStack = itemSlotUIs[index].itemStack;
        itemHoverText.gameObject.SetActive(true);
        if (itemStack.HasItem) {
            itemHoverText.text = $"{itemStack.itemType.name} ({itemStack.count})";
        } else {
            itemHoverText.text = $"Empty";
        }
    }
    public void OnUnHovered(int index) {
        itemHoverText.gameObject.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData) {
        if (inventory != null) {
            itemHoverText.transform.position = eventData.position;
        }
    }
}
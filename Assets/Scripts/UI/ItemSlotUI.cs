using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//IPointerMoveHandler
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] TMPro.TMP_Text countText;
    [SerializeField] Image itemImage;
    [SerializeField] GameObject selectedGo;
    [SerializeField] TMPro.TMP_Text nameText;

    ShowInventoryUI showInventoryUI;
    [ReadOnly] public ItemStack itemStack;
    // [SerializeField, ReadOnly] bool isHovered = false;

    private void Awake() {
        SetSelect(false);
    }
    public void Init(ShowInventoryUI showInventoryUI) {
        this.showInventoryUI = showInventoryUI;
    }

    public void UpdateItem(ItemStack itemStack) {
        this.itemStack = itemStack;
        countText.text = "" + itemStack.count;
        nameText.text = "" + itemStack.itemType?.name ?? "None";
        if (itemStack.itemType != null) {
            itemImage.sprite = itemStack.itemType.icon;
            itemImage.gameObject.SetActive(true);
        } else {
            itemImage.gameObject.SetActive(false);
        }
    }
    public void SetSelect(bool selected) {
        selectedGo.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData eventData) {
        showInventoryUI?.OnItemClicked(transform.GetSiblingIndex(), eventData);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // isHovered = true;
        // showInventoryUI?.OnHovered(transform.GetSiblingIndex());
        // selectedGo.SetActive(isHovered);
        nameText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // isHovered = false;
        // showInventoryUI?.OnUnHovered(transform.GetSiblingIndex());
        // selectedGo.SetActive(isHovered);
        nameText.gameObject.SetActive(false);
    }
}
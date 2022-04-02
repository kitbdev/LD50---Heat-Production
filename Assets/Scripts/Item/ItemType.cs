using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "ItemType", menuName = "HP/ItemType", order = 0)]
public class ItemType : ScriptableObject {
    // [System.Serializable]
    // public class ItemType {

    public int itemMaxStack;
    // higher first
    public int sortOrder;
}

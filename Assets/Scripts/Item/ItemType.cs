using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "ItemType", menuName = "HP/ItemType", order = 0)]
public class ItemType : ScriptableObject {
    // [System.Serializable]
    // public class ItemType {

    [Min(1)]
    public int itemMaxStack;
    // higher first
    [Tooltip("Higher first")]
    public int sortOrder;
    public Sprite icon;
    public float heatRate = 1;

    public override string ToString() {
        return name;
    }
}

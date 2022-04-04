using UnityEngine;

[ExtendedSO]
[CreateAssetMenu(fileName = "TileType", menuName = "HP/TileType", order = 0)]
public class TileType : UnityEngine.Tilemaps.Tile {

    [Space]
    // public string displayName;
    // public bool overrideBaseColor = false;
    public Material overrideMaterial;
    public bool blocksBuildings = true;
    public bool blocksPlayer = true;
    // like for miners
    public Building[] allowedBuildingTypes;
    // public Color color;
    public GameObject tilePrefab;// additional to existing prefab

    public ItemType produces;
    // public float productionTime;
    public TileType changeToTypeOnClick;
    public AudioClip produceGetClip;
    public float produceClipVol = 1f;

    public virtual void OnClick(Tile tile) {
        // nothing

        if (produces != null) {
            ProductionClick.Instance.ProduceItem(this, tile);
        }
    }
    // public override bool Equals(object other) {
    //     return name.Equals(other);
    // }
    // public override int GetHashCode() {
    //     return name.GetHashCode();
    // }
    // public static bool operator ==(TileType a, TileType other) => a?.name == other?.name;
    // public static bool operator !=(TileType a, TileType other) => a.name != other.name;
}

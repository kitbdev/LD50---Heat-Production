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
    public TileType changeToTypeOnClick;
    public AudioClip produceGetClip;

    public virtual void OnClick(Tile tile) {
        // nothing

        // make item
        if (produces != null) {
            if (produceGetClip != null) {
                AudioManager.Instance.PlaySfx(produceGetClip, tile.transform.position);
            }
            Inventory playerInventory = GameManager.Instance.playerInventory;
            if (playerInventory.HasSpaceFor(produces)) {
                playerInventory.AddItem(produces);
            }
        }
        if (changeToTypeOnClick != null) {
            // todo anim?
            tile.ChangeGroundTile(changeToTypeOnClick);
        }
    }
}

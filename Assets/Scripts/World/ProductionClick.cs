using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionClick : Singleton<ProductionClick> {

    [SerializeField] Timer timer;
    [SerializeField] Slider slider;

    [SerializeField, ReadOnly] TileType tileType;
    [SerializeField, ReadOnly] Tile tile;
    [SerializeField, ReadOnly] AudioSource audioSource;

    protected override void Awake() {
        audioSource = GetComponent<AudioSource>();
        slider.gameObject.SetActive(false);
    }
    private void OnEnable() {
        timer.onTimerComplete.AddListener(OnTimer);
        timer.onTimerUpdate.AddListener(OnTimerUpdate);
    }
    private void OnDisable() {
        timer.onTimerComplete.RemoveListener(OnTimer);
        timer.onTimerUpdate.RemoveListener(OnTimerUpdate);
    }

    public void ProduceItem(TileType tileType, Tile tile) {
        if (this.tileType != null) return;
        this.tileType = tileType;
        this.tile = tile;
        timer.ResumeTimer();
        slider.gameObject.SetActive(true);
    }
    public void OnTimerUpdate(float p) {
        slider.value = p;
    }
    public void OnTimer() {
        // make item
        if (tileType.produces != null) {
            if (tileType.produceGetClip != null) {
                // audioSource.PlayOneShot(tileType.produceGetClip, )
                AudioManager.Instance.PlaySfx(tileType.produceGetClip, tile.transform.position, tileType.produceClipVol);
            }
            Inventory playerInventory = GameManager.Instance.playerInventory;
            if (playerInventory.HasSpaceFor(tileType.produces)) {
                playerInventory.AddItem(tileType.produces);
            }
        }
        if (tileType.changeToTypeOnClick != null) {
            // todo anim?
            tile.ChangeGroundTile(tileType.changeToTypeOnClick);
        }
        timer.StopTimer();
        slider.gameObject.SetActive(false);
        this.tileType = null;
    }
}
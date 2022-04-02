using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager> {

    public Rect bounds;

    void ClearTiles() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) {
                Destroy(transform.GetChild(i).gameObject);
            } else {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    [ContextMenu("Create tiles")]
    void CreateTiles() {
        // fill bounds with grass tiles
        ClearTiles();
        for (int y = 0; y < bounds.height; y++) {
            for (int x = 0; x < bounds.width; x++) {
                // Instantiate()
            }
        }
    }

}
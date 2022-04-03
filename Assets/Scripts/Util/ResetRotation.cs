using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRotation : MonoBehaviour {
    public bool always = true;

    private void LateUpdate() {
        if (always) {
            ResetRot();
        }
    }

    public void ResetRot() {
        transform.localRotation = Quaternion.identity;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour {

    [SerializeField] GameObject[] optionalEffects;

    public void EnableExtraEffects(bool enable) {
        // snow and stuff
        foreach (var opef in optionalEffects) {
            opef.SetActive(enable);
        }
    }
}
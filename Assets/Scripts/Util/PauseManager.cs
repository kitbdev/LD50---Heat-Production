using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PauseManager : Singleton<PauseManager> {

    [ReadOnly] public bool isPaused = false;
    [SerializeField] bool pauseOnStart = false;
    public bool enablePauseButton = true;
    public bool allowUnpausing = true;
    public bool autoPauseWhenFocusLost = true;
    public bool autoUnpauseWhenFocusGainedAfterAutoPause = false;
    [SerializeField, ReadOnly] bool didAutoPause = false;

    [SerializeField] InputActionReference togglePauseButton;
    IEnumerator pauseLerpCo;

    [Header("Events")]
    public UnityEvent pauseEvent;
    public UnityEvent unpauseEvent;

    protected void OnEnable() {
        if (togglePauseButton) {
            togglePauseButton.action.Enable();
            togglePauseButton.action.performed += c => { if (enablePauseButton) TogglePause(); };
        }
    }
    private void OnDisable() {
        if (togglePauseButton) {
            togglePauseButton.action.Disable();
        }
    }
    private void Start() {
        if (pauseOnStart) {
            Pause();
        }
    }

    [ContextMenu("Toggle Pause")]
    public void TogglePause() {
        SetPaused(!isPaused);
    }
    public void Pause() {
        SetPaused(true);
    }
    public void UnPause() {
        SetPaused(false);
    }
    public void SetPaused(bool pause = true) {
        isPaused = pause;
        float targetScale = isPaused ? 0 : 1;
        Time.timeScale = targetScale;
        if (isPaused) {
            pauseEvent.Invoke();
        } else {
            unpauseEvent.Invoke();
        }
    }
    private void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            if (didAutoPause && autoUnpauseWhenFocusGainedAfterAutoPause) {
                UnPause();
                didAutoPause = false;
            }
        } else {
            if (autoPauseWhenFocusLost) {
                Pause();
                didAutoPause = true;
            }
        }
    }
    void OnApplicationPause(bool pauseStatus) {
        OnApplicationFocus(!pauseStatus);
    }
}
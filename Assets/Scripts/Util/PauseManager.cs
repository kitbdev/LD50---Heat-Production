using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PauseManager : Singleton<PauseManager> {

    [ReadOnly] public bool isPaused = false;
    [Min(0)]
    [SerializeField] float timeLerpDur = 0;
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
            togglePauseButton.action.Dispose();
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
        if (timeLerpDur > 0) {
            StopCoroutine(pauseLerpCo);
            pauseLerpCo = SetTimeScaleCo(targetScale);
            StartCoroutine(pauseLerpCo);
        } else {
            Time.timeScale = targetScale;
        }
        if (isPaused) {
            pauseEvent.Invoke();
        } else {
            unpauseEvent.Invoke();
        }
    }
    IEnumerator SetTimeScaleCo(float target) {
        float initial = Time.timeScale;
        float progress = 0;
        float interp = initial;
        float scaleSpeed = 1f / timeLerpDur;
        while (progress < 1) {
            yield return null;
            progress += Time.unscaledDeltaTime * scaleSpeed;
            interp = Mathf.Lerp(initial, target, progress);
            Time.timeScale = interp;
        }
        Time.timeScale = target;
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
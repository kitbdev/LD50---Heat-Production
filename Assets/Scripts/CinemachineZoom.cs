using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineZoom : MonoBehaviour {

    [SerializeField][Range(0, 100)] float minDist = 5;
    [SerializeField][Range(0, 100)] float defDist = 10;
    [SerializeField][Range(0, 100)] float maxDist = 20;
    [SerializeField] float scrollRate = -0.5f;

    [SerializeField] InputActionReference zoomAxis;
    [SerializeField] InputActionReference recenterBtn;

    [SerializeField, HideInInspector] CinemachineVirtualCamera virtualCamera;
    [SerializeField, HideInInspector] CinemachineFramingTransposer framingTransposer;

    private void Awake() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera?.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    private void OnEnable() {
        if (zoomAxis != null) {
            zoomAxis.action.Enable();
            zoomAxis.action.performed += c => AddCamDist(c.ReadValue<float>());
        }
        if (recenterBtn != null) {
            recenterBtn.action.Enable();
            recenterBtn.action.performed += c => SetCamDistDefault();
        }
    }
    private void OnDisable() {
        if (zoomAxis != null) {
            zoomAxis.action.Disable();
        }
        if (recenterBtn != null) {
            recenterBtn.action.Disable();
        }
    }
    private void Start() {
        SetCamDistDefault();
    }
    public void SetCamDistDefault() {
        SetCamDist(defDist);
    }
    void AddCamDist(float dist) {
        if (framingTransposer == null) return;
        float curDist = framingTransposer.m_CameraDistance;
        curDist += dist * scrollRate;
        SetCamDist(curDist);
    }
    void SetCamDist(float dist) {
        if (framingTransposer == null) return;
        if (Time.timeScale == 0) return;
        dist = Mathf.Clamp(dist, minDist, maxDist);
        framingTransposer.m_CameraDistance = dist;
    }
}
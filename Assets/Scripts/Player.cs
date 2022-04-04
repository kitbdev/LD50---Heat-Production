using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class Player : MonoBehaviour {

    [Header("Move")]
    public float moveSpeed;
    public float turnRate;
    [SerializeField, ReadOnly] Vector3 vel;

    [SerializeField] float minStepPitch = 0.8f;
    [SerializeField] float maxStepPitch = 1.2f;
    [SerializeField] float stepDelay = 0.2f;
    float stepLastTime = 0;

    [Header("Input")]
    Controls controls;
    [SerializeField][ReadOnly] Vector2 moveInput;

    CharacterController cc;
    AudioSource audioSource;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        controls = new Controls();
        controls.Enable();
        controls.Player.Move.performed += c => moveInput = c.ReadValue<Vector2>();
        controls.Player.Move.canceled += c => moveInput = Vector2.zero;
    }
    private void OnDisable() {
        controls.Disable();
    }

    private void Update() {
        // move
        vel = new Vector3(moveInput.x, 0, moveInput.y);
        vel = Vector3.ClampMagnitude(vel, 1f);
        vel *= moveSpeed * Time.deltaTime;
        cc.Move(vel);
        if (cc.velocity.sqrMagnitude >= 0.01f) {
            Vector3 dir = cc.velocity.normalized;
            Quaternion newRot = Quaternion.LookRotation(dir, Vector3.up);
            if (turnRate > 0) newRot = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * turnRate);
            transform.rotation = newRot;

            if (!audioSource.isPlaying && Time.time > stepLastTime + stepDelay) {
                // audioSource.loop = true;
                audioSource.pitch = Random.Range(minStepPitch, maxStepPitch);
                audioSource.Play();
                stepLastTime = Time.time;
                // Debug.Log("playing ");
            }
        } else {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }
}
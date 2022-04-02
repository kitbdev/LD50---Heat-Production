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
    [Header("Input")]
    Controls controls;
    [SerializeField][ReadOnly] Vector2 moveInput;

    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
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
        }
    }
}
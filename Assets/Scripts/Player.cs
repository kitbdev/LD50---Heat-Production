using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    public float moveSpeed;


    [SerializeField, ReadOnly] Vector3 vel;
    [Header("Input")]
    Controls controls;
    [SerializeField][ReadOnly] Vector2 moveInput;

    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        controls = new Controls();
    }

    private void OnEnable() {
        controls.Enable();
        controls.Player.Move.performed += c => moveInput = c.ReadValue<Vector2>();
        controls.Player.Move.performed += c => moveInput = Vector2.zero;
    }
    private void OnDisable() {
        controls.Disable();
    }

    private void Update() {
        // move
        vel = new Vector3(moveInput.x, 0, moveInput.y);
        vel = Vector3.ClampMagnitude(vel, 1f);
        vel *= moveSpeed;
        cc.Move(vel);
    }
}
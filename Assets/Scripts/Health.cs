using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {

    [SerializeField, ReadOnly] float health;
    public float maxHealth;
    public float regenRate;
    public bool regenActive = true;

    // float regenTimer = 0;

    public bool IsDead => health <= 0;
    public float CurHealth => health;
    public float HealthPercent => Mathf.Clamp01(health / maxHealth);

    [Header("Events")]
    public UnityEvent<float> onHealthUpdateEvent;
    public UnityEvent onDieEvent;

    private void Awake() {
        FullHeal();
    }
    public void FullHeal() {
        health = maxHealth;
        onHealthUpdateEvent?.Invoke(0);
    }

    private void Update() {
        if (health < maxHealth && regenActive) {
            // regen
            Heal(regenRate * Time.deltaTime);
        }
    }
    public void Heal(float amount) {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        onHealthUpdateEvent?.Invoke(Mathf.InverseLerp(maxHealth, 0f, health));
    }
    public void TakeDamage(float amount) {
        if (IsDead) return;
        health -= amount;
        // onHealthUpdateEvent?.Invoke(health);
        onHealthUpdateEvent?.Invoke(Mathf.InverseLerp(maxHealth, 0f, health));
        if (IsDead) {
            onDieEvent?.Invoke();
        }
    }
}
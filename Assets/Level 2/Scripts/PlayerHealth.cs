using UnityEngine;
using System;

/// <summary>
/// Player health system implementing IDamageable.
/// Handles taking damage and healing (negative damage).
/// Attach to the Player GameObject.
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    /// <summary>
    /// Fired when health changes. Passes (current, max).
    /// </summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>
    /// Fired when the player dies.
    /// </summary>
    public event Action OnDied;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage, string weaponName, string hitObjectName)
    {
        // Allow healing even when dead (for respawn)
        if (isDead && damage > 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Revive if healed above 0 while dead
        if (isDead && currentHealth > 0f)
        {
            isDead = false;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (damage > 0f)
        {
            Debug.Log($"[PlayerHealth] Took {damage} damage from [{weaponName}]. HP: {currentHealth}/{maxHealth}");
        }
        else
        {
            Debug.Log($"[PlayerHealth] Healed {-damage}. HP: {currentHealth}/{maxHealth}");
        }

        if (currentHealth <= 0f && !isDead)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        OnDied?.Invoke();
        Debug.Log("[PlayerHealth] Player died!");
        // Death handling (respawn, game over) will be
        // added in later phases.
    }
}

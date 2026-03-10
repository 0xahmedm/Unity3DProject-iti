using UnityEngine;
using System;

/// <summary>
/// Enemy health system implementing IDamageable.
/// Triggers death on the EnemyAI when HP reaches zero.
/// Attach to the Enemy GameObject.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    /// <summary>
    /// Fired when health changes. Passes (current, max).
    /// </summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>
    /// Fired when the enemy dies.
    /// </summary>
    public event Action OnDied;

    private float currentHealth;
    private bool isDead;
    private EnemyAI enemyAI;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, string weaponName, string hitObjectName)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[EnemyHealth] {gameObject.name} took {damage} damage from [{weaponName}]. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        OnDied?.Invoke();

        if (enemyAI != null)
        {
            enemyAI.Die();
        }

        Debug.Log($"[EnemyHealth] {gameObject.name} died.");
        Destroy(gameObject, 3f);
    }
}

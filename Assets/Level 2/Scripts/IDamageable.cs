using UnityEngine;

/// <summary>
/// Interface for anything that can receive damage.
/// Implemented by PlayerHealth, EnemyHealth, BossHealth, etc.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage, string weaponName, string hitObjectName)
    {
        Debug.Log($"[{weaponName}] Hit: {hitObjectName}");
    }
}

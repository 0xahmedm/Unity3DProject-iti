using UnityEngine;

/// <summary>
/// Health pickup item. When the player walks over it,
/// heals the player and destroys itself.
/// Attach to a pickup GameObject with a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float healAmount = 25f;

    [Header("Visual")]
    [SerializeField] private float rotateSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null) return;

        // Negative damage = healing. PlayerHealth will handle this.
        damageable.TakeDamage(-healAmount, "HealthPickup", "Player");
        Debug.Log($"[HealthPickup] +{healAmount} HP");
        Destroy(gameObject);
    }
}

using UnityEngine;

/// <summary>
/// Ammo pickup item. When the player walks over it,
/// adds reserve ammo to the AmmoManager.
/// If RespawnPickup is present, it handles hide/respawn.
/// Otherwise, destroys itself.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AmmoPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int ammoAmount = 30;

    [Header("Visual")]
    [SerializeField] private float rotateSpeed = 90f;

    public int AmmoAmount => ammoAmount;

    private bool hasRespawn;

    private void Awake()
    {
        hasRespawn = GetComponent<RespawnPickup>() != null;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If RespawnPickup is present, it handles the collection
        if (hasRespawn) return;

        AmmoManager ammoManager = other.GetComponent<AmmoManager>();
        if (ammoManager == null) return;

        ammoManager.AddReserveAmmo(ammoAmount);
        Debug.Log($"[AmmoPickup] +{ammoAmount} ammo");
        Destroy(gameObject);
    }
}

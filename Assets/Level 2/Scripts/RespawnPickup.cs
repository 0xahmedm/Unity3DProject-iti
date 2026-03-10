using UnityEngine;

/// <summary>
/// Makes a pickup respawn after a delay instead of being destroyed.
/// Attach to the same GameObject as AmmoPickup or HealthPickup.
/// </summary>
public class RespawnPickup : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 10f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Collider pickupCollider;
    private Renderer[] renderers;
    private bool isCollected;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        pickupCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (other.GetComponent<AmmoManager>() == null) return;

        Collect();
    }

    private void Collect()
    {
        isCollected = true;

        // Give ammo to the player
        AmmoManager manager = FindAnyObjectByType<AmmoManager>();
        AmmoPickup ammoPickup = GetComponent<AmmoPickup>();

        if (manager != null && ammoPickup != null)
        {
            manager.AddReserveAmmo(ammoPickup.AmmoAmount);
        }

        SetVisible(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isCollected = false;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (pickupCollider != null)
        {
            pickupCollider.enabled = visible;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }
}

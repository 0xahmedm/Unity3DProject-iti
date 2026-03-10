using UnityEngine;

/// <summary>
/// Portal trigger zone. When the player enters,
/// triggers the level complete event.
/// Attach to a trigger collider at the portal location.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Debug.Log("[PortalTrigger] Player reached the portal!");

        if (gameManager != null)
        {
            gameManager.TriggerLevelComplete();
        }
    }
}

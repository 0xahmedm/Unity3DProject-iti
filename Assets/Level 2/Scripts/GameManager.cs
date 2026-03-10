using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager: handles win/lose conditions,
/// respawn, and level progression.
/// Attach to an empty "GameManager" GameObject.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WaveManager waveManager;

    [Header("Doors")]
    [SerializeField] private DoorController finalDoor;

    [Header("UI Panels")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button respawnButton;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Player Animation")]
    [SerializeField] private Animator playerAnimator;

    private static readonly int DieHash = Animator.StringToHash("Die");

    private bool gameOver;

    private void Start()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (respawnButton != null)
        {
            respawnButton.onClick.AddListener(Respawn);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDeath;
        }

        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared += HandleAllWavesCleared;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandlePlayerDeath;
        }

        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared;
        }
    }

    private void HandlePlayerDeath()
    {
        if (gameOver) return;

        Debug.Log("[GameManager] Player died!");

        // Play die animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(DieHash);
        }

        // Show death UI
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        // Unlock cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private void HandleAllWavesCleared()
    {
        Debug.Log("[GameManager] All waves cleared! Opening final door.");

        if (finalDoor != null)
        {
            finalDoor.Open();
        }
    }

    /// <summary>
    /// Called by the level complete trigger (portal).
    /// </summary>
    public void TriggerLevelComplete()
    {
        gameOver = true;
        Debug.Log("[GameManager] Level complete!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private void Respawn()
    {
        Time.timeScale = 1f;

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        // Reset player health
        PlayerHealth health = playerHealth;
        if (health != null)
        {
            // Re-initialize health by reloading component state
            health.TakeDamage(-health.MaxHealth, "Respawn", "Player");
        }

        // Move player to respawn point
        if (respawnPoint != null)
        {
            CharacterController cc = playerHealth.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                playerHealth.transform.position = respawnPoint.position;
                playerHealth.transform.rotation = respawnPoint.rotation;
                cc.enabled = true;
            }
        }

        // Re-lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[GameManager] Player respawned.");
    }
}

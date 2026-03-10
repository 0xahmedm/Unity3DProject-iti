using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the level start sequence:
/// 3-second countdown → opens the start door.
/// Attach to a UI manager or empty GameObject.
/// </summary>
public class LevelStart : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownDuration = 3f;

    [Header("Door")]
    [SerializeField] private DoorController startDoor;

    [Header("Player Lock")]
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        // Disable player movement during countdown
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        float timer = countdownDuration;

        while (timer > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(timer).ToString();
            }

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (countdownText != null)
        {
            countdownText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        // Open the start door
        if (startDoor != null)
        {
            startDoor.Open();
        }

        // Enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Debug.Log("[LevelStart] Countdown finished. Door opened.");
    }
}

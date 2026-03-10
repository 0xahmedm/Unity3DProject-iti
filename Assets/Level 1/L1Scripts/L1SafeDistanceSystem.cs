using UnityEngine;
using System.Collections;

public class L1SafeDistanceSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public L1PlayerMovement movement;
    public Animator playerAnimator;
    public L1EnemyChase enemy;
    public L1PlayerHealth playerHealth;
    private L1Score score;
    [Header("Distance")]
    public float safeDistance = 500f;
    public float distanceMultiplier = 0.1f;

    [Header("Spawn")]
    public GameObject robotPrefab;
    public GameObject portalPrefab;
    public AudioClip Win;
    public Vector3 robotOffset = new Vector3(-20f, 0f, 0f);
    public Vector3 portalOffset = new Vector3(-35f, 4f, 0f);

    [Header("Timing")]
    public float WaitingTime = 3f;

    private bool completed;

    void Update()
    {
        if (completed)
            return;

        float distance = (player.position.x + 300f) * distanceMultiplier;

        if (distance >= safeDistance)
        {
            StartCoroutine(LevelCompleteSequence());
        }
    }

    IEnumerator LevelCompleteSequence()
    {
        completed = true;
        AudioManager.Instance.StopLoopingSound("BackgroundID");
        AudioManager.Instance.PlaySound(Win,1);
        SpawnRobot();
        SpawnPortal();

        // Trigger spawn protection when portal appears
        if (playerHealth != null)
        {
            playerHealth.spawnProtectionDuration=9;
            StartCoroutine(playerHealth.SpawnProtection());
        }
        if (movement != null)
            movement.stopMoving = true;

        if (enemy != null)
            enemy.stopChasing = true;

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Idle");

        yield return new WaitForSeconds(WaitingTime);
        PlayerPrefs.DeleteKey("CheckpointX");
        score.ResetSave();
        PlayerPrefs.Save();
        if (movement != null)
            movement.stopMoving = false;
    }

    void SpawnRobot()
    {
        if (robotPrefab == null) return;
        Vector3 pos = new Vector3(player.position.x + robotOffset.x,player.position.y + robotOffset.y,-29f);
        Instantiate(robotPrefab, pos, Quaternion.Euler(-90f, -90f, 0f));
    }

    void SpawnPortal()
    {
        if (portalPrefab == null) return;

        float middleLaneZ = movement != null ? movement.middleLaneZ : player.position.z;

        Vector3 pos = new Vector3(player.position.x + portalOffset.x,player.position.y + portalOffset.y,middleLaneZ);

        Instantiate(portalPrefab, pos, Quaternion.Euler(0f, 90f, 0f));
    }
}
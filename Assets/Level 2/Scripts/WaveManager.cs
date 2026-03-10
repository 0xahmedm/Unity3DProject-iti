using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages enemy wave spawning across multiple zones.
/// Zone 1 spawns first. After all Zone 1 enemies are killed,
/// remaining zones spawn simultaneously.
/// Attach to an empty GameObject in the scene.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnZone
    {
        public string zoneName;
        public Transform[] patrolPoints;
        public int enemyCount = 3;
        public Transform spawnPoint;
    }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private SpawnZone[] zones;
    [SerializeField] private float spawnInterval = 0.5f;

    /// <summary>
    /// Fired when all enemies from all waves are dead.
    /// </summary>
    public event Action OnAllWavesCleared;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentWaveGroup;
    private bool allWavesSpawned;

    public int TotalEnemiesAlive => activeEnemies.Count;

    private void Start()
    {
        if (zones.Length == 0) return;

        // Spawn Zone 1 (index 0)
        StartCoroutine(SpawnZoneCoroutine(zones[0]));
        currentWaveGroup = 1;
    }

    private void Update()
    {
        // Clean up destroyed enemies
        activeEnemies.RemoveAll(e => e == null);

        // Check if current wave group is cleared
        if (activeEnemies.Count == 0 && !allWavesSpawned)
        {
            SpawnNextWaveGroup();
        }
        else if (activeEnemies.Count == 0 && allWavesSpawned)
        {
            allWavesSpawned = false; // Prevent re-firing
            Debug.Log("[WaveManager] All waves cleared!");
            OnAllWavesCleared?.Invoke();
        }
    }

    private void SpawnNextWaveGroup()
    {
        if (currentWaveGroup >= zones.Length)
        {
            allWavesSpawned = true;
            return;
        }

        // Spawn all remaining zones simultaneously
        for (int i = currentWaveGroup; i < zones.Length; i++)
        {
            StartCoroutine(SpawnZoneCoroutine(zones[i]));
        }

        currentWaveGroup = zones.Length;
        allWavesSpawned = true;
    }

    private IEnumerator SpawnZoneCoroutine(SpawnZone zone)
    {
        Debug.Log($"[WaveManager] Spawning zone: {zone.zoneName}");

        for (int i = 0; i < zone.enemyCount; i++)
        {
            Vector3 spawnPos = zone.spawnPoint.position;
            spawnPos += UnityEngine.Random.insideUnitSphere * 2f;
            spawnPos.y = zone.spawnPoint.position.y;

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Assign patrol points to the spawned enemy's AI
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                SetPatrolPoints(ai, zone.patrolPoints);
            }

            activeEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Assigns patrol points to the spawned enemy via public setter.
    /// </summary>
    private void SetPatrolPoints(EnemyAI ai, Transform[] points)
    {
        ai.SetPatrolPoints(points);
    }
}

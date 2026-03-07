using System.Collections.Generic;
using UnityEngine;

public class L1SegmentSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;
    public int segmentsOnScreen = 4;
    public float segmentLength = 500f;
    public bool useCheckpoint;

    private float spawnX = 500f;
    private List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        if (useCheckpoint && PlayerPrefs.HasKey("CheckpointX"))
        {
            float savedX = PlayerPrefs.GetFloat("CheckpointX");
            spawnX = Mathf.Floor(savedX / segmentLength) * segmentLength;
        }

        for (int i = 0; i < segmentsOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player.position.x > spawnX - (segmentsOnScreen * segmentLength))
        {
            SpawnSegment();
        }

        if (activeSegments.Count > 0)
        {
            GameObject firstSegment = activeSegments[0];

            if (player.position.x > firstSegment.transform.position.x + segmentLength)
            {
                DeleteSegment();
            }
        }
    }

    void SpawnSegment()
    {
        int index = Random.Range(0, segmentPrefabs.Length);

        GameObject segment = Instantiate(
            segmentPrefabs[index],
            new Vector3(spawnX, 0, 0),
            Quaternion.identity);

        activeSegments.Add(segment);
        spawnX += segmentLength;
    }

    void DeleteSegment()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
}
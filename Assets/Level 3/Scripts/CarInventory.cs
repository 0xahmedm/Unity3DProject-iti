using System.Collections.Generic;
using UnityEngine;

public class CarInventory : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform boxesParent;
    [SerializeField] private Transform[] spawnPoints;

    private List<GameObject> boxes = new List<GameObject>();
    private int currentBoxIndex = 0;

    public int BoxCount => boxes.Count;
    public int MaxBoxes => spawnPoints.Length;

    public void Refill()
    {
        // Clear existing
        foreach (var b in boxes) Destroy(b);
        boxes.Clear();
        currentBoxIndex = 0;

        // Spawn full load
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject box = Instantiate(boxPrefab, spawnPoints[i].position, spawnPoints[i].rotation, boxesParent);
            boxes.Add(box);
        }

        Debug.Log("Inventory refilled!");
    }

    public void ConsumeBoxes(int count)
    {
        for (int i = 0; i < count && boxes.Count > 0; i++)
        {
            currentBoxIndex = (currentBoxIndex - 1 + spawnPoints.Length) % spawnPoints.Length;
            Destroy(boxes[boxes.Count - 1]);
            boxes.RemoveAt(boxes.Count - 1);
        }
    }
}
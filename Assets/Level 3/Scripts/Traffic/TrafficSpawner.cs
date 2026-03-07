using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] vehiclePrefabs;
    [SerializeField] private TrafficWaypoint[] spawnPoints;
    [SerializeField] private int maxVehicles = 10;
    [SerializeField] private float spawnInterval = 3f;

    private int activeVehicles = 0;
    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activeVehicles < maxVehicles)
        {
            SpawnVehicle();
            spawnTimer = 0;
        }
    }

    void SpawnVehicle()
    {
        if (vehiclePrefabs.Length == 0 || spawnPoints.Length == 0)
        return;

        TrafficWaypoint spawnWp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Debug.Log(spawnWp.name);

        if (spawnWp == null)
            return;

        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];

        GameObject vehicle = Instantiate(
        prefab,
        spawnWp.transform.position,
        spawnWp.transform.rotation
        );

        TrafficVehicle trafficVehicle = vehicle.GetComponent<TrafficVehicle>();

        if (trafficVehicle != null)
        {
            trafficVehicle.Init(spawnWp);
            activeVehicles++;
        }
        else
        {
            Destroy(vehicle);
        }
    }
}

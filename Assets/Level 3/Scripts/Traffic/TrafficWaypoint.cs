using UnityEngine;

public class TrafficWaypoint : MonoBehaviour
{
    [SerializeField] private TrafficWaypoint[] nextWaypoints;
    public  float speedLimit = 10f;

    public TrafficWaypoint GetNextWaypoint()
    {
        if (nextWaypoints.Length == 0) return null;
        return nextWaypoints[Random.Range(0, nextWaypoints.Length)];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
        foreach (var next in nextWaypoints)
        {
            if (next != null)
                Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
}

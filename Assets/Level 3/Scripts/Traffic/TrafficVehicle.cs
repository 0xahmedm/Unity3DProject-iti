using UnityEngine;

public class TrafficVehicle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float waypointReachDistance = 1.5f;

    [Header("Braking")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask vehicleLayer;

    [SerializeField] private TrafficWaypoint currentWaypoint;
    private float currentSpeed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Init(TrafficWaypoint startWaypoint)
    {
        currentWaypoint = startWaypoint;
        transform.position = startWaypoint.transform.position;
    }

    void FixedUpdate()
    {
        if (currentWaypoint == null)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        CheckForVehiclesAhead();
        MoveTowardWaypoint();

        if (ReachedWaypoint())
            currentWaypoint = currentWaypoint.GetNextWaypoint();
    }

    void MoveTowardWaypoint()
    {
        Vector3 direction = currentWaypoint.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)   // حماية من LookRotation error
        {
            direction.Normalize();

            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.fixedDeltaTime
            );
        }

        float targetSpeed = Mathf.Min(maxSpeed, currentWaypoint.speedLimit);
        currentSpeed = Mathf.MoveTowards(
        currentSpeed,
        targetSpeed,
        acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = transform.forward * currentSpeed;
    }

    void CheckForVehiclesAhead()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, detectionRange, vehicleLayer))
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * 2f * Time.fixedDeltaTime);
            rb.linearVelocity = transform.forward * currentSpeed;
        }
    }

    bool ReachedWaypoint()
    {
        float dist = Vector3.Distance(transform.position, currentWaypoint.transform.position);
        return dist < waypointReachDistance;
    }
}

using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target")]
    public  Transform target;

    [Header("Follow Axes")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;
    [SerializeField] private bool followZ = true;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Smoothing")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            followX ? target.position.x + offset.x : transform.position.x,
            followY ? target.position.y + offset.y : transform.position.y,
            followZ ? target.position.z + offset.z : transform.position.z
        );

        if (useSmoothing)
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        else
            transform.position = desiredPosition;
    }
}

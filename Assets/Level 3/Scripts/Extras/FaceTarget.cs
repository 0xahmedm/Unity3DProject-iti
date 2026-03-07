using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothing")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float rotationSpeed = 10f;

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return; 

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (useSmoothing)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        else
            transform.rotation = targetRotation;
    }
}

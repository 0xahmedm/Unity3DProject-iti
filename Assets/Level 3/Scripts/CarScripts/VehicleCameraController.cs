using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float positionSmoothing = 5f;
    [SerializeField] private float rotationSmoothing = 3f;
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minHeight = 3f;
    [SerializeField] private float maxHeight = 15f;
    
    private float currentHeight;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        currentHeight = offset.y;
    }

    private void Update()
    {
        // Handle zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentHeight -= scrollInput * zoomSpeed;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        // Update offset height
        Vector3 currentOffset = offset;
        currentOffset.y = currentHeight;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + target.rotation * currentOffset;
        
        // Smooth position with SmoothDamp (much better for rotation shake)
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / positionSmoothing);
        
        // Smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }
}
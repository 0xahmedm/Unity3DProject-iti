using UnityEngine;

/// <summary>
/// FPS-style mouse look. Rotates the Player body on Y-axis
/// and a CameraLookTarget on X-axis with configurable clamping.
/// Both FPS and TPS Cinemachine cameras should follow/look-at
/// the CameraLookTarget so they respect the same look direction.
/// Attach to the Player GameObject.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float upperLookLimit = 80f;
    [SerializeField] private float lowerLookLimit = 80f;

    [Header("References")]
    [Tooltip("Empty child at head height. Cinemachine cameras Follow/LookAt this.")]
    [SerializeField] private Transform cameraLookTarget;

    private float verticalRotation;

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        HandleHorizontalLook();
        HandleVerticalLook();
    }

    /// <summary>
    /// Rotates the player body left/right on the Y-axis.
    /// </summary>
    private void HandleHorizontalLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Tilts the CameraLookTarget up/down on the X-axis,
    /// clamped between the configured limits.
    /// </summary>
    private void HandleVerticalLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -upperLookLimit,
            lowerLookLimit
        );

        cameraLookTarget.localRotation = Quaternion.Euler(
            verticalRotation, 0f, 0f
        );
    }

    /// <summary>
    /// Locks and hides the cursor for FPS gameplay.
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

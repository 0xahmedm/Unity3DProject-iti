using UnityEngine;

/// <summary>
/// Modular ground detection using Physics.CheckSphere.
/// Attach to the Player GameObject alongside PlayerMovement.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private Transform checkTransform;
    [SerializeField] private float checkRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;

    /// <summary>
    /// True when the player is standing on a surface
    /// matching the ground layer mask.
    /// </summary>
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(
            checkTransform.position,
            checkRadius,
            groundMask
        );
    }

    private void OnDrawGizmos()
    {
        if (checkTransform == null) return;

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkTransform.position, checkRadius);
    }
}

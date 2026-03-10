using UnityEngine;

/// <summary>
/// Bridges PlayerMovement and GroundChecker to the Animator.
/// Drives animation parameters based on movement state.
/// Attach to the Player GameObject (same as PlayerMovement).
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Smoothing")]
    [SerializeField] private float speedDampTime = 0.1f;

    // Cached parameter hashes for performance
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    private GroundChecker groundChecker;

    private void Awake()
    {
        groundChecker = GetComponent<GroundChecker>();
    }

    private void Start()
    {
        // Disable root motion so animations don't move/float
        // the character — CharacterController handles all movement.
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        UpdateLocomotion();
        UpdateGroundedState();
        UpdateJumpTrigger();
    }

    /// <summary>
    /// Sets the Speed float based on raw input magnitude.
    /// More reliable than controller.velocity since it reads
    /// player intent directly. 0 = idle, 1 = full speed.
    /// </summary>
    private void UpdateLocomotion()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        animator.SetFloat(SpeedHash, inputMagnitude, speedDampTime, Time.deltaTime);
    }

    /// <summary>
    /// Sets the IsGrounded bool for the Animator.
    /// </summary>
    private void UpdateGroundedState()
    {
        animator.SetBool(IsGroundedHash, groundChecker.IsGrounded);
    }

    /// <summary>
    /// Fires the Jump trigger when the player presses jump while grounded.
    /// </summary>
    private void UpdateJumpTrigger()
    {
        if (Input.GetButtonDown("Jump") && groundChecker.IsGrounded)
        {
            animator.SetTrigger(JumpHash);
        }
    }
}

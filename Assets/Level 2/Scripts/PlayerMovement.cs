using UnityEngine;

/// <summary>
/// Handles WASD movement, jumping, and gravity
/// using CharacterController. Requires GroundChecker
/// on the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private GroundChecker groundChecker;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundChecker = GetComponent<GroundChecker>();
    }

    private void Update()
    {
        HandleGravityReset();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    /// <summary>
    /// Resets downward velocity when grounded to avoid
    /// accumulating gravity while standing on a surface.
    /// </summary>
    private void HandleGravityReset()
    {
        if (groundChecker.IsGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    /// <summary>
    /// Reads horizontal and vertical input axes and moves
    /// the character relative to its facing direction.
    /// </summary>
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal
                              + transform.forward * vertical;

        controller.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Applies an upward velocity impulse when the player
    /// presses Jump while grounded.
    /// </summary>
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && groundChecker.IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// Applies gravity to the vertical velocity and moves
    /// the controller downward each frame.
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

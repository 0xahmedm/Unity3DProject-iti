using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class L2PlayerMovement : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed = 6f;

    [Header("Jump")] public float jumpForce = 8f;

    [Header("Gravity")] public float gravity = -20f;

    [Header("Ground Check")] public Transform groundCheck;
    public float groundRadius = 0.3f;
    public LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpForce;
        }
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
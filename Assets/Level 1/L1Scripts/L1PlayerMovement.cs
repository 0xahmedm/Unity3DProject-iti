using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class L1PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 12f;

    public float leftLaneZ = -24f;
    public float middleLaneZ = -39.5f;
    public float rightLaneZ = -55f;

    public float laneSwitchSpeed = 12f;

    public float jumpForce = 10f;
    public float gravity = -25f;

    public bool stopMoving;
    public bool useCheckpoint;

    [Header("Auto Save")]
    public float autoSaveInterval = 3f; // saves every 3 seconds

    private float autoSaveTimer;
    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity;
    private int currentLane = 1;
    private float targetZ;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        targetZ = middleLaneZ;
        autoSaveTimer = autoSaveInterval;

        if (useCheckpoint && PlayerPrefs.HasKey("CheckpointX"))
        {
            float savedX = PlayerPrefs.GetFloat("CheckpointX");

            controller.enabled = false;

            Vector3 pos = transform.position;
            pos.x = savedX;
            transform.position = pos;

            controller.enabled = true;

            Debug.Log("Player spawned at checkpoint X: " + savedX);
        }
    }

    void Update()
    {
        HandleLaneInput();
        HandleJump();
        MovePlayer();
        UpdateAnimator();

        if (useCheckpoint)
            AutoSave();
    }

    void AutoSave()
    {
        autoSaveTimer -= Time.deltaTime;

        if (autoSaveTimer <= 0)
        {
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.Save();
            autoSaveTimer = autoSaveInterval;
            Debug.Log("Auto saved at X: " + transform.position.x);
        }
    }

    void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            SetTargetLane();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            SetTargetLane();
        }
    }

    void SetTargetLane()
    {
        if (currentLane == 0)
            targetZ = leftLaneZ;
        else if (currentLane == 1)
            targetZ = middleLaneZ;
        else
            targetZ = rightLaneZ;
    }

    void HandleJump()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                verticalVelocity = jumpForce;

                if (animator)
                    animator.SetTrigger("Jump");
            }
        }

        if (!controller.isGrounded)
        {
            animator.SetTrigger("Falling");
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    void MovePlayer()
    {
        float moveX = stopMoving ? 0 : forwardSpeed;
        float difference = targetZ - transform.position.z;
        float moveZ = difference * laneSwitchSpeed;

        Vector3 move = new Vector3(
            moveX,
            verticalVelocity,
            moveZ
        );

        controller.Move(move * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        if (!animator) return;

        animator.SetFloat("Speed", forwardSpeed);
        animator.SetBool("Grounded", controller.isGrounded);
    }
}
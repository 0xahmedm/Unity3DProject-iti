using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class L4Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    public Vector3 CurrentMoveInput { get; private set; }

    [Header("Audio")]
    private float timer;
    [SerializeField] private float intervalBetweenFootsteps = 0.5f;
    [SerializeField] private Transform footStepPos;
    [SerializeField] private Vector2 minMaxPitch = new Vector2(0.8f, 1.2f);

    [SerializeField] private AudioClip footStepSound;



    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        
    }
    private Vector3 MovementDir()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        return move; 
    }

    void HandleMovement()
    {
        Vector3 move = MovementDir();
        if (move.magnitude > 0)
        {
            timer += Time.deltaTime;
            if (timer >= intervalBetweenFootsteps)
            {
                timer = 0;
                AudioManager.Instance.PlayClipAtPosition(footStepSound, footStepPos.position, 1, minMaxPitch.x, minMaxPitch.y);
            }
        }
        CurrentMoveInput = move;

        controller.Move(move * moveSpeed * Time.deltaTime);


        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}

using UnityEngine;

public class L4Camera : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 60f;
    [SerializeField] private Transform playerBody;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    [SerializeField] private float mouseSmoothTime = 0.05f;


    [Header("Head Bob")]
    [SerializeField] private float bobFrequency = 1.2f;   // Slow
    [SerializeField] private float bobAmplitude = 0.03f;  // Subtle
    [SerializeField] private float bobSmoothness = 6f;

    private float xRotation = 0f;
    private float bobTimer = 0f;
    private Vector3 initialLocalPosition;

    private L4Movement controller;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        controller = playerBody.GetComponent<L4Movement>();
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
        );

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            mouseSmoothTime
        );

        float mouseX = currentMouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = currentMouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleHeadBob()
    {
        if (controller == null) return;

        if (controller.CurrentMoveInput.magnitude > 0.1f && controller.GetComponent<CharacterController>().isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;

            Vector3 targetPosition = initialLocalPosition + new Vector3(0f, bobOffsetY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * bobSmoothness);
        }
        else
        {
            bobTimer = 0f;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                initialLocalPosition,
                Time.deltaTime * bobSmoothness
            );
        }
    }
}

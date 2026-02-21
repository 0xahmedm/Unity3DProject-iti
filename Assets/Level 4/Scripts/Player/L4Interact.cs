    using UnityEngine;

public class L4Interact : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Camera playerCamera;

    [Header("Hold Settings")]
    [SerializeField] private float holdThreshold = 0.4f;
    [SerializeField] private HingeJoint HoldPoint;

    private float holdTimer;

    [SerializeField] private IHoldable currentHoldable;
    [SerializeField] private Rigidbody HoldableRb;
    private bool isHolding;

    public float ThrowingForce = 10f;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            holdTimer = 0f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (!isHolding && holdTimer >= holdThreshold)
            {
                TryHold();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!isHolding)
                TryInteract();
        }

        if (isHolding && Input.GetKeyDown(KeyCode.E))
        {
            Drop();
        }

        if( isHolding && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Throw();
        }
    }

    void TryInteract()
    {
        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out RaycastHit hit,
                            interactDistance,
                            interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    void TryHold()
    {
        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out RaycastHit hit,
                            interactDistance,
                            interactLayer))
        {
            IHoldable holdable = hit.collider.GetComponent<IHoldable>();
            if (holdable != null)
            {
                currentHoldable = holdable;
                isHolding = true;
                HoldableRb= hit.collider.GetComponent<Rigidbody>();
                HoldPoint.connectedBody = HoldableRb;
            }
        }
    }

    void Throw()
    {
        if(currentHoldable != null)
        {
             
            if (HoldableRb != null)
            {
                HoldPoint.connectedBody = null;
                HoldableRb.AddForce(playerCamera.transform.forward * ThrowingForce, ForceMode.VelocityChange);
                currentHoldable = null;
                isHolding = false;
            }
        }
    }
    void Drop()
    {
        HoldPoint.connectedBody = null;
        currentHoldable = null;
        isHolding = false;
    }
}

using UnityEngine;

/// <summary>
/// Moves a cube/door downward to simulate a door opening.
/// Call Open() to start the animation.
/// Attach to the door cube GameObject.
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openDistance = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.down * openDistance;
    }

    private void Update()
    {
        if (!isOpening) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            openPosition,
            openSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, openPosition) < 0.01f)
        {
            isOpening = false;
        }
    }

    /// <summary>
    /// Starts moving the door downward.
    /// </summary>
    public void Open()
    {
        isOpening = true;
        Debug.Log($"[DoorController] {gameObject.name} opening.");
    }
}

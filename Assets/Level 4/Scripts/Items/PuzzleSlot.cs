using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    [SerializeField] private string requiredPieceID;
    [SerializeField] private Transform snapPoint;

    private bool isOccupied = false;

    // Reference to the parent manager
    private PuzzleManager puzzleManager;

    private void Awake()
    {
        // Auto-find the PuzzleManager on parent
        puzzleManager = GetComponentInParent<PuzzleManager>();
        if (puzzleManager == null)
        {
            Debug.LogError($"PuzzleSlot '{name}' has no PuzzleManager in parent hierarchy!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;

        IPuzzlePiece piece = other.GetComponent<IPuzzlePiece>();
        if (piece == null) return;

        Debug.Log("Piece detected: " + piece.PieceID);

        if (piece.PieceID == requiredPieceID)
        {
            PlacePiece(other.gameObject, piece);
        }
        else
        {
            Debug.Log("Wrong piece!");
        }
    }

    private void PlacePiece(GameObject pieceObj, IPuzzlePiece piece)
    {
        isOccupied = true;

        pieceObj.transform.position = snapPoint.position;
        pieceObj.transform.rotation = snapPoint.rotation;

        Rigidbody rb = pieceObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        piece.OnPuzzlePiecePlaced();

        // Notify the manager that this slot is filled
        puzzleManager.OnSlotFilled(this);
    }

    public bool IsOccupied => isOccupied;
}
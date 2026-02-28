using UnityEngine;

public class PuzzlePiece : MonoBehaviour,
    IPuzzlePiece, ICollectable, IInteractable, IHoldable
{
    [SerializeField] private string pieceID;
    [SerializeField] private string interactionText;

    public string PieceID => pieceID;

    public void Interact()
    {
        Debug.Log("Interacted with " + interactionText);
    }

    public void OnCollect() { }

    public void OnPuzzlePiecePlaced()
    {
        Debug.Log($"{pieceID} placed correctly!");
    }
}
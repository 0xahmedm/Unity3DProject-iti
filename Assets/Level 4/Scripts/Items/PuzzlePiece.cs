using UnityEngine;

public class PuzzlePiece : MonoBehaviour,
    IPuzzlePiece, ICollectable, IInteractable, IHoldable
{
    [SerializeField] private string pieceID;
    [SerializeField] private string interactionText;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private float interactVolume = 1f;

    public string PieceID => pieceID;

    public void Interact()
    {
        Debug.Log("Interacted with " + interactionText);
        if (interactSound != null)
            L4AudioManager.Instance.PlaySound(interactSound, interactVolume);
    }

    public void OnCollect() { }

    public void OnPuzzlePiecePlaced()
    {
        Debug.Log($"{pieceID} placed correctly!");
    }
}
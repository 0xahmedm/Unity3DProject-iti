using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private PuzzleSolvedEvent puzzleSolvedEvent;

    [Header("Settings")]
    [Tooltip("Auto-collected from children if left empty")]
    [SerializeField] private List<PuzzleSlot> slots = new List<PuzzleSlot>();
    [SerializeField] private AudioClip puzzleSolvedSound;
    private int slotsFilled = 0;
    private bool isSolved = false;

    private void Awake()
    {
        // Auto-collect slots from children if not manually assigned
        if (slots.Count == 0)
        {
            GetComponentsInChildren<PuzzleSlot>(slots);
        }

        Debug.Log($"PuzzleManager '{name}' tracking {slots.Count} slots.");
    }

    /// <summary>
    /// Called by a PuzzleSlot when a correct piece is placed.
    /// </summary>
    public void OnSlotFilled(PuzzleSlot slot)
    {
        if (isSolved) return;

        slotsFilled++;
        Debug.Log($"[{name}] Slot filled: {slotsFilled}/{slots.Count}");

        if (puzzleSolvedSound != null)
        {
            AudioManager.Instance.PlaySound(puzzleSolvedSound, 1);
        }

        if (slotsFilled >= slots.Count)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        Debug.Log($"[{name}] Puzzle solved!");

        if (puzzleSolvedEvent != null)
        {
            puzzleSolvedEvent.Raise(this);
        }
    }

    public bool IsSolved => isSolved;
    public float Progress => slots.Count > 0 ? (float)slotsFilled / slots.Count : 0f;
}
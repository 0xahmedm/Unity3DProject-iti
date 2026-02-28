using UnityEngine;

public class L4GameManager : MonoBehaviour
{
    [SerializeField] private PuzzleSolvedEvent puzzleSolvedEvent;
    [SerializeField] private int totalPuzzlesRequired = 3;

    private int puzzlesSolved = 0;

    private void OnEnable()
    {
        puzzleSolvedEvent.OnEventRaised += HandlePuzzleSolved;
    }

    private void OnDisable()
    {
        puzzleSolvedEvent.OnEventRaised -= HandlePuzzleSolved;
    }

    private void HandlePuzzleSolved(PuzzleManager puzzle)
    {
        puzzlesSolved++;
        Debug.Log($"Puzzle '{puzzle.name}' solved! Total: {puzzlesSolved}/{totalPuzzlesRequired}");

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (puzzlesSolved >= totalPuzzlesRequired)
        {
            Debug.Log("All puzzles completed! You win!");
            // Trigger end-game logic here
        }
    }
}
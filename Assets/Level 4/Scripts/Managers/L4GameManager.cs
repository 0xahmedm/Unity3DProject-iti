using UnityEngine;

public class L4GameManager : MonoBehaviour
{
    [SerializeField] private PuzzleSolvedEvent puzzleSolvedEvent;
    [SerializeField] private int totalPuzzlesRequired = 3;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private float volume = 1;
    public GameObject Portal;
    public GameObject Robot;

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
            Robot.SetActive(true);
            Portal.SetActive(true);

            // Trigger end-game logic here
            if (winSound != null)
            {
                L4AudioManager.Instance.PlaySound(winSound, 1);
            }
        }
    }
}
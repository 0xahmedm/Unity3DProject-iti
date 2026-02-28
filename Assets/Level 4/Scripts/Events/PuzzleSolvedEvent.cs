using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Puzzle Solved")]
public class PuzzleSolvedEvent : ScriptableObject
{
    public Action<PuzzleManager> OnEventRaised;

    public void Raise(PuzzleManager puzzle)
    {
        OnEventRaised?.Invoke(puzzle);
    }
}
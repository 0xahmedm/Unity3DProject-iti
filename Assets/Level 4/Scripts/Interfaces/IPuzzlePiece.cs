using UnityEngine;

public interface IPuzzlePiece
{
    string PieceID { get; }
    void OnPuzzlePiecePlaced()
        {
        // Implement logic for when the puzzle piece is placed correctly
    }

}

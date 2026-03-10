using UnityEngine;

/// <summary>
/// Draws a simple crosshair at screen center using OnGUI.
/// No external assets needed. Attach to any active GameObject.
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private float size = 10f;
    [SerializeField] private float thickness = 2f;
    [SerializeField] private float gap = 5f;
    [SerializeField] private Color color = Color.white;

    private void OnGUI()
    {
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        GUI.color = color;
        Texture2D tex = Texture2D.whiteTexture;

        // Top
        GUI.DrawTexture(new Rect(
            centerX - thickness / 2f,
            centerY - gap - size,
            thickness, size), tex);

        // Bottom
        GUI.DrawTexture(new Rect(
            centerX - thickness / 2f,
            centerY + gap,
            thickness, size), tex);

        // Left
        GUI.DrawTexture(new Rect(
            centerX - gap - size,
            centerY - thickness / 2f,
            size, thickness), tex);

        // Right
        GUI.DrawTexture(new Rect(
            centerX + gap,
            centerY - thickness / 2f,
            size, thickness), tex);
    }
}

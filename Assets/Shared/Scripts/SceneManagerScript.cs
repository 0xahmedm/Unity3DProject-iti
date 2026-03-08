using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    public Portal portal;
    public string currentLevel;
    public string nextScene;

    void Start()
    {
        if (portal != null)
        {
            portal.currentLevel = currentLevel;
            portal.nextScene = nextScene;
        }
    }
}
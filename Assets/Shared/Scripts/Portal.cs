using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string currentLevel;
    public string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveSystem.MarkLevelComplete(currentLevel);
            SaveSystem.SetCheckpoint(currentLevel);
            SceneManager.LoadScene(nextScene);
        }
    }
}
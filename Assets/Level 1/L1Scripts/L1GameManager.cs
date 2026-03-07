using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class L1GameManager : MonoBehaviour
{
    public static L1GameManager instance;

    [Header("References")]
    public Transform player;
    public L1PlayerHealth playerHealth;
    public L1Score scoreSystem;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI toysText;
    public TextMeshProUGUI highScoreText;

    [Header("Settings")]
    public float distanceMultiplier = 0.1f;

    private bool gameOver;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel)
            gameOverPanel.SetActive(false);
    }

    // Called automatically by Unity when the application quits
    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("Toys");
        PlayerPrefs.Save();
        Debug.Log("Save data cleared on quit");
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        Time.timeScale = 0f;

        int distance = Mathf.FloorToInt((player.position.x + 300) * distanceMultiplier);
        int coins = scoreSystem.coins;
        int toys = scoreSystem.toys;

        SaveHighScore(distance);

        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        if (distanceText)
            distanceText.text = "Distance: " + distance;

        if (coinsText)
            coinsText.text = "Coins: " + coins;

        if (toysText)
            toysText.text = "Toys: " + toys;

        if (highScoreText)
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
    }

    void SaveHighScore(int distance)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (distance > highScore)
        {
            PlayerPrefs.SetInt("HighScore", distance);
            PlayerPrefs.Save();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartFromBeginning()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("Toys");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("Toys");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        Application.Quit();
    }
}
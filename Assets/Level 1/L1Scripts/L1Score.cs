using TMPro;
using UnityEngine;

public class L1Score : MonoBehaviour
{
    [Header("Counters")]
    public int coins;
    public int toys;

    [Header("UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI toyText;

    void Start()
    {
        Load();
        UpdateUI();
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        Save();
        UpdateUI();
    }

    public void AddToy(int amount)
    {
        toys += amount;
        Save();
        UpdateUI();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Toys", toys);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        toys = PlayerPrefs.GetInt("Toys", 0);
    }

    public void ResetSave()
    {
        coins = 0;
        toys = 0;
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("Toys");
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText)
            coinText.text = coins.ToString();

        if (toyText)
            toyText.text = toys.ToString();
    }
}
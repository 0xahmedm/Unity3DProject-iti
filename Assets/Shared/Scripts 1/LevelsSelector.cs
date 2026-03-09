

using UnityEngine;
using UnityEngine.UI;


public class LevelsSelector : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private int level1;
    [SerializeField] private int level2;
    [SerializeField] private int level3;
    [SerializeField] private int level4;

    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;



    private void Start()
    {
        level1Button.onClick.AddListener(() => LoadSelectedLevel(level1));
        level2Button.onClick.AddListener(() => LoadSelectedLevel(level2));
        level3Button.onClick.AddListener(() => LoadSelectedLevel(level3));
        level4Button.onClick.AddListener(() => LoadSelectedLevel(level4));

    }

    private void LoadSelectedLevel(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);
    }


}

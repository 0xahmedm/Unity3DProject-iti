
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public enum CurrentMenu { Settings, Game, MainMenu, Inventory }
public class UIHandeler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Button backButtonSettings;



    [Header("Game")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Button backButtonGame;




    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingsButton;


    [Header("Levels")]
    [SerializeField] private GameObject Levels;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button backButtonLevels;

    [Header("CurrentMenu")]
    [SerializeField] private CurrentMenu currentMenu = CurrentMenu.MainMenu;

    private void Start()
    {

        HandelUIChange();
        startButton.onClick.AddListener(StartGame);

        settingsButton.onClick.AddListener(() => ChangeMenu(CurrentMenu.Settings));
        levelsButton.onClick.AddListener(() => ChangeMenu(CurrentMenu.Inventory));


        backButtonSettings.onClick.AddListener(() => ChangeMenu(CurrentMenu.MainMenu));
        backButtonGame.onClick.AddListener(() => ChangeMenu(CurrentMenu.MainMenu));
        backButtonLevels.onClick.AddListener(() => ChangeMenu(CurrentMenu.MainMenu));



        exitButton.onClick.AddListener(() => Application.Quit());
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Day2Side");
    }
    private void ChangeMenu(CurrentMenu menu)
    {
        currentMenu = menu;
        HandelUIChange();
    }


    private void HandelUIChange()
    {
        switch (currentMenu)
        {
            case CurrentMenu.MainMenu:
                mainMenuUI.SetActive(true);
                Levels.SetActive(false);
                settingsUI.SetActive(false);
                gameUI.SetActive(false);
                break;
            case CurrentMenu.Settings:
                settingsUI.SetActive(true);
                gameUI.SetActive(false);
                break;
            case CurrentMenu.Game:
                settingsUI.SetActive(false);
                gameUI.SetActive(true);
                break;
            case CurrentMenu.Inventory:

                mainMenuUI.SetActive(false);
                Levels.SetActive(true);
                break;
        }
    }



}

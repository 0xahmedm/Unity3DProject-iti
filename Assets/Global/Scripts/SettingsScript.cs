using UnityEngine;
using UnityEngine.UI;


public enum CurrentMenuInSettings { Graphics, Audio, Controls }
public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Button GraphicsButton;
    [SerializeField] private Button AudioButton;
    [SerializeField] private Button ControlsButton;



    [SerializeField] private GameObject GraphicsMenu;
    [SerializeField] private GameObject AudioMenu;
    [SerializeField] private GameObject ControlsMenu;



    private GameObject CurrentMenu;


    public void Start()
    {
        GraphicsButton.onClick.AddListener(() => ChangeMenu(CurrentMenuInSettings.Graphics));
        AudioButton.onClick.AddListener(() => ChangeMenu(CurrentMenuInSettings.Audio));
        ControlsButton.onClick.AddListener(() => ChangeMenu(CurrentMenuInSettings.Controls));
    }

    public void ChangeMenu(CurrentMenuInSettings menu)
    {
        if (CurrentMenu != null)
        CurrentMenu.SetActive(false);
        switch (menu)
        {
            case CurrentMenuInSettings.Graphics:
                CurrentMenu = GraphicsMenu;
                break;
            case CurrentMenuInSettings.Audio:
                CurrentMenu = AudioMenu;
                break;
            case CurrentMenuInSettings.Controls:
                CurrentMenu = ControlsMenu;
                break;
        }
        CurrentMenu.SetActive(true);
    }
}

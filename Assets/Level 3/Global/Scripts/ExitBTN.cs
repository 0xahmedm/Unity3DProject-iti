using UnityEngine;
using UnityEngine.UI;

public class ExitBTN : MonoBehaviour
{
    [SerializeField] private Button button;
    void Start()
    {
        button.onClick.AddListener(() => Application.Quit());
    }


}

using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int count;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private Button items;
    [SerializeField] private GameObject parent;

    void Start()
    {
        UpgradeButton.onClick.AddListener(Upgrade);
    }

    private void Upgrade()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(items, parent.transform);
        }
    }
}

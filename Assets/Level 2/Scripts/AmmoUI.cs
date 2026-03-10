using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays magazine ammo / reserve ammo on a UI Text element.
/// Listens to AmmoManager.OnAmmoChanged event.
/// Attach to the Player or a UI manager GameObject.
/// </summary>
public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private AmmoManager ammoManager;

    private void OnEnable()
    {
        if (ammoManager != null)
        {
            ammoManager.OnAmmoChanged += UpdateAmmoDisplay;
        }
    }

    private void OnDisable()
    {
        if (ammoManager != null)
        {
            ammoManager.OnAmmoChanged -= UpdateAmmoDisplay;
        }
    }

    private void UpdateAmmoDisplay(int magazine, int maxMagazine, int reserve)
    {
        if (ammoText == null) return;
        ammoText.text = $"{magazine} / {reserve}";
    }
}

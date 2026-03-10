using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Player health bar UI. Listens to PlayerHealth.OnHealthChanged.
/// Uses a UI Slider for the bar and optional TMP text for numbers.
/// Attach to the Player or a UI manager GameObject.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color criticalColor = Color.red;
    [SerializeField] private float criticalThreshold = 0.3f;

    private Image fillImage;

    private void Awake()
    {
        if (healthSlider != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
            healthText.color = Color.green;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float current, float max)
    {
        float ratio = current / max;

        if (healthSlider != null)
        {
            healthSlider.value = ratio;
        }

        if (healthText != null)
        {
            healthText.text = $"Health {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        if (fillImage != null)
        {
            fillImage.color = ratio <= criticalThreshold
                ? criticalColor
                : healthyColor;
            healthText.color = fillImage.color;
        }
    }
}

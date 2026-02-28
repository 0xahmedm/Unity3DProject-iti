using UnityEngine;
using UnityEngine.UI;

public class L1HealthUI : MonoBehaviour
{
    public Slider slider;

    public L1PlayerHealth playerHealth;


    void Update()
    {
        slider.maxValue=playerHealth.maxHealth;
        slider.value =Mathf.Lerp(slider.value,playerHealth.currentHealth,5);
    }
}
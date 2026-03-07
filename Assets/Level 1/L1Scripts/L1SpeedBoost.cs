using UnityEngine;
using UnityEngine.UI;

public class L1SpeedBoost : MonoBehaviour
{
    private L1PlayerMovement movement;

    private float timer;

    private float originalSpeed;

    private bool active;
    public RawImage energy;
    public Slider slider;

    void Start()
    {
        movement = GetComponent<L1PlayerMovement>();

        originalSpeed = movement.forwardSpeed;

    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;
        energy.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);

        slider.value =Mathf.Lerp(slider.value,timer,10);

        if (timer <= 0)
        {
            movement.forwardSpeed =originalSpeed;
            energy.gameObject.SetActive(false);
            slider.gameObject.SetActive(false);
            active = false;
        }
    }

    public void ActivateBoost(float duration,float boostAmount)
    {
        movement.forwardSpeed =originalSpeed + boostAmount;
        timer = duration;
        active = true;
        slider.maxValue=duration;
    }
}
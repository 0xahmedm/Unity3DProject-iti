using UnityEngine;

public class L1SpeedBoost : MonoBehaviour
{
    private L1PlayerMovement movement;

    private float timer;

    private float originalSpeed;

    private bool active;

    void Start()
    {
        movement =
            GetComponent<L1PlayerMovement>();

        originalSpeed =
            movement.forwardSpeed;
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            movement.forwardSpeed =
                originalSpeed;

            active = false;
        }
    }

    public void ActivateBoost(
        float duration,
        float boostAmount)
    {
        movement.forwardSpeed =
            originalSpeed + boostAmount;

        timer = duration;

        active = true;
    }
}
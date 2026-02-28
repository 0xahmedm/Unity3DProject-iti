using UnityEngine;

public class L1MagnetEffect : MonoBehaviour
{
    public bool magnetActive;

    public float attractDistance = 20f;
    public float attractSpeed = 25f;

    private float timer;

    void Update()
    {
        if (!magnetActive) return;

        timer -= Time.deltaTime;

        AttractCoins();

        if (timer <= 0)
            magnetActive = false;
    }

    public void ActivateMagnet(float duration)
    {
        magnetActive = true;
        timer = duration;
    }

    void AttractCoins()
    {
        Collider[] coins =
            Physics.OverlapSphere(
                transform.position,
                attractDistance);

        foreach (Collider c in coins)
        {
            if (c.CompareTag("Coin"))
            {
                Vector3 dir =
                    (transform.position -
                    c.transform.position).normalized;

                c.transform.position +=
                    dir *
                    attractSpeed *
                    Time.deltaTime;
            }
        }
    }
}
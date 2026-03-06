using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class L1MagnetEffect : MonoBehaviour
{
    public bool magnetActive;

    public float attractDistance = 20f;
    public float attractSpeed = 25f;
    public RawImage magnet;
    public Slider slider;
    private float timer;

    void Update()
    {
        if (!magnetActive) return;

        timer -= Time.deltaTime;
        magnet.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        AttractCoins();
        slider.value =Mathf.Lerp(slider.value,timer,50);

        if (timer <= 0){
            magnetActive = false;
            magnet.gameObject.SetActive(false);
            slider.gameObject.SetActive(false);
        }
    }

    public void ActivateMagnet(float duration)
    {
        magnetActive = true;
        timer = duration;
        slider.maxValue=duration;
    }

    void AttractCoins()
    {
        Collider[] coins = Physics.OverlapSphere(transform.position,attractDistance);

        foreach (Collider c in coins)
        {
            if (c.CompareTag("Coin"))
            {
                Vector3 dir =
                    (transform.position -c.transform.position).normalized;

                c.transform.position +=dir *attractSpeed *Time.deltaTime;
            }
        }
    }
}
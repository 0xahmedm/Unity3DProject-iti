using UnityEngine;

public class L1EnergyOrb : MonoBehaviour
{
    public float duration = 10f;
    public float speedBoost = 8f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            L1SpeedBoost boost =
                other.GetComponent<L1SpeedBoost>();

            if (boost != null)
                boost.ActivateBoost(duration, speedBoost);

            Destroy(gameObject);
        }
    }
}
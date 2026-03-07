using UnityEngine;

public class L1Obstacle : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            L1PlayerHealth health =other.GetComponent<L1PlayerHealth>();

            health.TakeDamage(damage);
            if(health.maxHealth>=1)
            Destroy(this.gameObject);
        }
        if(other.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}
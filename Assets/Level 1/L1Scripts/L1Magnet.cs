using UnityEngine;

public class L1Magnet : MonoBehaviour
{
    public float duration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            L1MagnetEffect magnet =
                other.GetComponent<L1MagnetEffect>();

            if (magnet != null)
                magnet.ActivateMagnet(duration);

            Destroy(gameObject);
        }
    }
}
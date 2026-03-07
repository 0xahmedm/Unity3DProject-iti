using UnityEngine;

public class L1Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            L1Score score =
                other.GetComponent<L1Score>();

            if (score != null)
                score.AddCoin(value);

            Destroy(gameObject);
        }
    }
}
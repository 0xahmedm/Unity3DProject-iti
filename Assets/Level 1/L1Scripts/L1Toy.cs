using UnityEngine;

public class L1Toy : MonoBehaviour
{
    public AudioClip ToyPickup;
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            L1Score score = other.GetComponent<L1Score>();
            AudioManager.Instance.PlaySound(ToyPickup,0.3f);
            if (score != null)
                score.AddToy(value);

            Destroy(gameObject);
        }
    }
}
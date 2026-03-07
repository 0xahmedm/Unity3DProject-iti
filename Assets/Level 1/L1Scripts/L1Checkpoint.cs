using UnityEngine;

public class L1Checkpoint : MonoBehaviour
{
    public bool saveCoins = true;
    public bool saveDistance = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (saveDistance)
            {
                PlayerPrefs.SetFloat("CheckpointX", other.transform.position.x);
                Debug.Log("Checkpoint saved at X: " + other.transform.position.x);
            }

            if (saveCoins)
            {
                L1Score score = other.GetComponent<L1Score>();

                if (score != null)
                    score.Save();
            }

            PlayerPrefs.Save();

            Debug.Log("Checkpoint Saved");
        }
    }
}
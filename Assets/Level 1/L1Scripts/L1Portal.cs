using UnityEngine;

public class L1Portal : MonoBehaviour
{
    public GameObject winPanel;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Time.timeScale=0f;

            if(winPanel!=null)
                winPanel.SetActive(true);

            Debug.Log("PLAYER WON");
        }
    }
}
using TMPro;
using UnityEngine;

public class L1DistanceUI : MonoBehaviour
{
    public Transform player;

    public TextMeshProUGUI distanceText;

    public float multiplier = 0.1f;

    void Update()
    {
        int distance =Mathf.FloorToInt((player.position.x+300) *multiplier);

        distanceText.text =distance.ToString();
    }
}
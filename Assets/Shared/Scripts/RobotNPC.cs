using UnityEngine;
using System.Collections;

public class RobotNPC : MonoBehaviour
{
    public GameObject bubbleCanvas;
    public float bubbleDelay = 2f;

    void Start()
    {
        if(bubbleCanvas != null)
            bubbleCanvas.SetActive(false);

        StartCoroutine(ShowBubble());
    }

    IEnumerator ShowBubble()
    {
        yield return new WaitForSeconds(bubbleDelay);

        if(bubbleCanvas != null)
            bubbleCanvas.SetActive(true);
    }
}
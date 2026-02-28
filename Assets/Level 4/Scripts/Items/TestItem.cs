using UnityEngine;

public class TestItem : MonoBehaviour, IHoldable, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}

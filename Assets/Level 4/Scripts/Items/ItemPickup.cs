using UnityEngine;

public class ItemPickup : MonoBehaviour,IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private InventorySystem inventorySystem;

    private void Pickup()
    {
        inventorySystem.AddItem(itemData);
        Destroy(gameObject);
    }
     public void Interact()
    {
        Pickup();
    }
}

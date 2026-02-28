using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private InventoryChangedEvent inventoryChangedEvent;

    private List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            Debug.Log("Added item: " + item.displayName);

            inventoryChangedEvent.Raise(item);
        }
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            inventoryChangedEvent.Raise(item);
        }
    }
}
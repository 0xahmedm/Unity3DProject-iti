using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Inventory Changed")]
public class InventoryChangedEvent : ScriptableObject
{
    public Action<ItemData> OnEventRaised;

    public void Raise(ItemData item)
    {
        OnEventRaised?.Invoke(item);
    }
}
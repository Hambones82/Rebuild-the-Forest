using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryItem 
{
    [SerializeField]
    private InventoryItemType _itemType;
    public InventoryItemType ItemType { get => _itemType; }

    [SerializeField]
    float amount;

    public InventoryItem(InventoryItemType itemType)
    {
        _itemType = itemType;
        amount = 1;
    }

    public InventoryItem(InventoryItemType itemType, float inAmount)
    {
        _itemType = itemType;
        amount = inAmount;
    }
}

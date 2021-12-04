using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryItem 
{
    [SerializeField]
    private InventoryItemType _itemType;
    public InventoryItemType ItemType { get => _itemType; }

    public InventoryItem(InventoryItemType itemType)
    {
        _itemType = itemType;
    }
}

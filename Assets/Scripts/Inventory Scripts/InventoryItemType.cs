using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemType", menuName = "ScriptableObjects/Types/Inventory Item Type")]
public class InventoryItemType : SOType
{
    [SerializeField]
    protected string name;

    [SerializeField]
    protected bool _buildable;

    [SerializeField]
    protected Building _buildingPrefab;

    [SerializeField]
    protected bool _cleanable;
    public bool Cleanable { get => _cleanable; }

    [SerializeField]
    protected InventoryItemType _cleanedItemType;
    public InventoryItemType CleanedItemType
    {
        get => _cleanedItemType;
    }
}

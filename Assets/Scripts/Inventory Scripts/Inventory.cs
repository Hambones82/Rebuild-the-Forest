using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> inventoryItems;

    public void AddItem(InventoryItemType inventoryItemType)
    {
        InventoryItem newItem = new InventoryItem(inventoryItemType);
        inventoryItems.Add(newItem);
    }

    public bool RemoveItem(InventoryItemType inventoryItemType)
    {
        foreach(InventoryItem item in inventoryItems)
        {
            if(item.ItemType == inventoryItemType)
            {
                inventoryItems.Remove(item);
                return true;
            }
        }
        return false;
    }

    public bool HasItem(InventoryItemType inventoryItemType)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.ItemType == inventoryItemType)
            {
                return true;
            }
        }
        return false;
    }

    public List<InventoryItem> GetCleanableInventoryItems()
    {
        List<InventoryItem> retVal = new List<InventoryItem>();
        foreach(InventoryItem item in inventoryItems)
        {
            if(item.ItemType.Cleanable)
            {
                retVal.Add(item);
            }
        }
        return retVal;
    }

    public void CleanAllInventoryItems()
    {
        List<InventoryItem> cleanables = new List<InventoryItem>();
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.ItemType.Cleanable)
            {
                cleanables.Add(item);
            }
        }
        foreach(InventoryItem item in cleanables)
        {
            InventoryItem cleanedItem = new InventoryItem(item.ItemType.CleanedItemType);
            inventoryItems.Add(cleanedItem);
            inventoryItems.Remove(item);
        }
    }
}

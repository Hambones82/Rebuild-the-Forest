using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> inventoryItems;
    //need to add functionality related to amount here...
    //maybe to fix all of the problems, AddItem and RemoveItem need to specify float amount.
    //otherwise, we might be leaving things in the code that don't properly account for the amount idea.
    public void AddItem(InventoryItemType inventoryItemType, float amount)
    {
        InventoryItem itemToAdd = GetItem(inventoryItemType);
        if(itemToAdd == null)
        {
            itemToAdd = new InventoryItem(inventoryItemType, amount);
            inventoryItems.Add(itemToAdd);
        }
        else
        {
            itemToAdd.Amount += amount;
        }
                
    }

    public List<InventoryItem> GetAllInventoryItems()
    {
        return new List<InventoryItem>(inventoryItems);
    }

    //returns whether it was removed
    public bool RemoveItem(InventoryItemType inventoryItemType, float amount)
    {
        InventoryItem foundItem = GetItem(inventoryItemType);
        if (foundItem != null)
        {
            if(foundItem.Amount >= amount)
            {
                foundItem.Amount -= amount;
                if(foundItem.Amount == 0)
                {
                    inventoryItems.Remove(foundItem);
                }
                return true;
            }        
        }        
        return false;
    }

    public bool HasItem(InventoryItemType inventoryItemType)
    {
        if (GetItem(inventoryItemType) != null) return true;
        return false;        
    }

    public InventoryItem GetItem(InventoryItemType inventoryItemType)
    {
        return inventoryItems.Find(item => item.ItemType == inventoryItemType);
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

    //should have clean 1 --> and have an amount...
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
            InventoryItem cleanedItem = new InventoryItem(item.ItemType.CleanedItemType, item.Amount);
            inventoryItems.Add(cleanedItem);
            inventoryItems.Remove(item);
        }
    }
}

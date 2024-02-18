using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> inventoryItems;

    public delegate void InventoryChangeEvent(InventoryItemType item, float resultingAmount);
    public event InventoryChangeEvent OnInventoryChange;
    
    public void AddItem(InventoryItemType inventoryItemType, float amount)
    {
        InventoryItem itemToAdd = GetItem(inventoryItemType);
        if(itemToAdd == null)
        {
            itemToAdd = new InventoryItem(inventoryItemType, amount);
            inventoryItems.Add(itemToAdd);
            OnInventoryChange?.Invoke(inventoryItemType, amount);
        }
        else
        {
            itemToAdd.Amount += amount;
            OnInventoryChange?.Invoke((InventoryItemType)inventoryItemType, itemToAdd.Amount);
        }
                
    }

    public List<InventoryItem> GetAllInventoryItems()
    {
        return new List<InventoryItem>(inventoryItems);
    }

    //returns whether it was removed
    //can remove nested ifs here for simplicity...
    public bool RemoveItem(InventoryItemType inventoryItemType, float amount)
    {
        InventoryItem foundItem = GetItem(inventoryItemType);
        if (foundItem == null) return false;
        
        if(foundItem.Amount >= amount)
        {
            foundItem.Amount -= amount;
            if(foundItem.Amount == 0)
            {
                inventoryItems.Remove(foundItem);
            }
            //only invoked when there's an actual change.  the amount it's changed to is the amount left in item
            OnInventoryChange?.Invoke(inventoryItemType, foundItem.Amount);
            return true;
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

    public List<InventoryItem> GetItems(Func<InventoryItem, bool> test)
    {
        return inventoryItems.Where(test).ToList();        
    }

    public bool HasItem(Func<InventoryItem, bool> test) 
    { 
        return inventoryItems.Select(test).Any(testVal => testVal == true);
    }

    public float GetItemAmount(InventoryItemType itemType)
    {
        return GetItem(itemType)?.Amount ?? 0;
    }
}

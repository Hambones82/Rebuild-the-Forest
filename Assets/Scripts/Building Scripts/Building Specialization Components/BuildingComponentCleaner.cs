using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentCleaner : BuildingComponentOperator
{
    
    private bool IsCleanable(InventoryItem item)
    {
        return item.ItemType.Cleanable;
    } 
    public override bool IsOperatable(GameObject inGameObject)
    {
        return inGameObject.GetComponent<Inventory>().HasItem(IsCleanable);
    }

    protected override void Trigger(GameObject inGameObject)
    {
        //all the code from the inventory belongs here.  
        Inventory inventory = inGameObject.GetComponent<Inventory>();
        List<InventoryItem> items = inventory.GetItems(IsCleanable);
        foreach(InventoryItem item in items) 
        {
            inventory.AddItem(item.ItemType.CleanedItemType, item.Amount);
            inventory.RemoveItem(item.ItemType, item.Amount);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingComponentStoreToInventory : BuildingComponentOperator
{
    private Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    [SerializeField]
    private List<InventoryItemType> allowedInventoryItems;
    public override bool IsOperatable(GameObject inGameobject)
    {
        Inventory incomingInventory = inGameobject.GetComponent<Inventory>();
        if (incomingInventory == null) return false;
        return GetAllowedIncomingItems(incomingInventory).Any();
    }

    protected override void Trigger(GameObject gameObject = null)
    {
        if (gameObject == null) return;
        Inventory incomingInventory = gameObject.GetComponent<Inventory>();
        foreach(InventoryItem item in GetAllowedIncomingItems(incomingInventory))
        {
            inventory.AddItem(item.ItemType, item.Amount);
            incomingInventory.RemoveItem(item.ItemType, item.Amount);
        }
    }

    private List<InventoryItem> GetAllowedIncomingItems(Inventory incomingInventory)
    {
        //what we need is...  the intersection between what incoming has and what is allowed
        return incomingInventory.GetAllInventoryItems()
            .Where(iItem => allowedInventoryItems.Contains(iItem.ItemType)).ToList();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentCleaner : BuildingComponentOperator
{
    public override bool IsOperatable(GameObject inGameObject)
    {
        List<InventoryItem> cleanableInventoryItems = inGameObject.GetComponent<Inventory>().GetCleanableInventoryItems();

        return cleanableInventoryItems.Count > 0;

    }

    protected override void Trigger(GameObject inGameObject)
    {
        inGameObject.GetComponent<Inventory>().CleanAllInventoryItems();
    }
}

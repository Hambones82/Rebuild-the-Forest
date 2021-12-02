using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentHarvestable : BuildingComponentOperator
{
    [SerializeField]
    InventoryItemType inventoryItemType;

    public override bool IsOperatable(GameObject inGameObject)
    {
        return true;
    }
    
    protected override void Trigger(GameObject inGameObject)
    {
        inGameObject.GetComponent<Inventory>().AddItem(inventoryItemType);
        BuildingManager.Instance.DestroyBuilding(GetComponent<Building>());
    }
}

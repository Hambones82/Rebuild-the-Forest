using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentHarvestable : BuildingComponentOperator
{
    [SerializeField]
    private InventoryItemType inventoryItemType;
    //needs an amount...
    [SerializeField] 
    private float amount;
    public override bool IsOperatable(GameObject inGameObject)
    {
        return true;
    }
    
    protected override void Trigger(GameObject inGameObject)
    {        
        inGameObject.GetComponent<Inventory>().AddItem(inventoryItemType, amount);
        BuildingManager.Instance.DestroyBuilding(GetComponent<Building>());
    }
}

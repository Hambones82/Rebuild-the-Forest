using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingComponentFueledBlocker : MonoBehaviour
{
    [SerializeField]
    private InventoryItemType fuelType;

    [SerializeField]
    private float fuelThreshold; 
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private MapEffectComponent effectComponent;
    [SerializeField]
    private MapEffectType blockingEffectType;

    //for viewing only - not intended to be changed in the editor   
    [SerializeField]
    private bool blockingIsEnabled;
    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        Assert.IsNotNull(inventory, "Fueled Blocker component requires inventory.");
        effectComponent = GetComponent<MapEffectComponent>();
        Assert.IsNotNull(effectComponent, "Fueled Blocker component requires effect component.");
        blockingIsEnabled = false;
        float fuelAmount = inventory.GetItemAmount(fuelType);
        ProcessFuelChange(fuelAmount);
        inventory.OnInventoryChange += ProcessInventoryChangeEvent;        
    }

    private void ProcessInventoryChangeEvent(InventoryItemType itemType, float amount) 
    {        
        if(itemType == fuelType)
        {        
            ProcessFuelChange(amount);
        }
    }

    private void ProcessFuelChange(float fuelAmount)
    {        
        if (HasEnoughFuel(fuelAmount) && !blockingIsEnabled)
        {            
            SwitchBlockingOn();
        }
        else if (!HasEnoughFuel(fuelAmount) && blockingIsEnabled)
        {
            SwitchBlockingOff();
        }
    }

    private void SwitchBlockingOn()
    {
        blockingIsEnabled = true;
        effectComponent.EnableEffect(blockingEffectType);
    }

    private void SwitchBlockingOff()
    {
        blockingIsEnabled = false;
        effectComponent.DisableEffect(blockingEffectType);
    }

    private void OnEnable()
    {

    }

    //also ondisable to get rid of effects???

    private bool HasEnoughFuel(float fuelAmount)
    {
        return fuelAmount >= fuelThreshold;
    }

}

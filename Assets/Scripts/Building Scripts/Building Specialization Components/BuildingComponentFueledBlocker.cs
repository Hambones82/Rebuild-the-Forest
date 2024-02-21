using System;
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
    [SerializeField]
    float amountLostOnFuelBlock;

    public Action EnableShieldEvent;
    public Action DisableShieldEvent;

    public delegate void ShieldChangeDelegate(float strength);
    public ShieldChangeDelegate ShieldChangeEvent;

    //for viewing only - not intended to be changed in the editor   
    [SerializeField]
    private bool blockingIsEnabled;
    public bool BlockingIsEnabled { get => blockingIsEnabled; }
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
        effectComponent.NotifyEffect += ProcessNotifyEvent;
    }

    private void ProcessNotifyEvent(MapEffectType effectType)
    {
        if(effectType == blockingEffectType)
        {
            inventory.RemoveItem(fuelType, amountLostOnFuelBlock);
        }
    }

    private void ProcessInventoryChangeEvent(InventoryItemType itemType, float amount) 
    {
        //Debug.Log($"processing inventory change event to {amount}; blocking enabled: {blockingIsEnabled}; HasEnough: {HasEnoughFuel(amount)}");
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
        if(blockingIsEnabled)
        {
            ShieldChangeEvent?.Invoke(fuelAmount);
        }
    }

    private void SwitchBlockingOn()
    {
        //Debug.Log("we are switching blocking on"); //so... this isn't happening...???
        blockingIsEnabled = true;
        effectComponent.EnableEffect(blockingEffectType);
        EnableShieldEvent?.Invoke();
    }

    private void SwitchBlockingOff()
    {
        //Debug.Log("trying to switch blocking off");
        blockingIsEnabled = false;
        effectComponent.DisableEffect(blockingEffectType);
        DisableShieldEvent?.Invoke();
    }

    private void OnEnable()
    {

    }

    //also ondisable to get rid of effects???

    private bool HasEnoughFuel(float fuelAmount)
    {
        return fuelAmount >= fuelThreshold;
    }

    public int GetRange()
    {
        return effectComponent.GetEffect(blockingEffectType).Range;
    }

}

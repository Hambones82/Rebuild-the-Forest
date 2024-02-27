using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingComponentFueled : MonoBehaviour
{    
    [SerializeField]
    private InventoryItemType fuelType;
    [SerializeField]
    private MapEffectType effectType;
    [SerializeField]
    private float fuelThreshold;    
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private MapEffectComponent effectComponent;
    [SerializeField]
    private bool effectIsEnabled;
    public bool EffectIsEnabled {get => effectIsEnabled; }
    [SerializeField]
    private float amountLostOnMapEffectEvent;
    
    public Action EnableEffectEvent;
    public Action DisableEffectEvent;

    public delegate void EffectTagDelegate(Vector2Int hitPosition);
    public EffectTagDelegate EffectTagEventOut;

    public Action<float> FuelChangeEvent;

    private void Awake()
    {        
        inventory = GetComponent<Inventory>();
        Assert.IsNotNull(inventory, "Fueled component requires inventory.");
        effectComponent = GetComponent<MapEffectComponent>();
        Assert.IsNotNull(effectComponent, "Fueled component requires effect component.");
        effectIsEnabled = false;
        ProcessFuelChange(inventory.GetItemAmount(fuelType));
        inventory.OnInventoryChange += ProcessInventoryChangeEvent;
        effectComponent.TagEffectEvent += ProcessTagEffectEvent;
    }

    private void ProcessTagEffectEvent(MapEffectType effectType, Vector2Int cell)
    {
        if (effectType == this.effectType)
        {     
            inventory.RemoveItem(fuelType, amountLostOnMapEffectEvent);
            EffectTagEventOut?.Invoke(cell); 
        }
    }

    private void ProcessInventoryChangeEvent(InventoryItemType itemType, float amount)
    {        
        if (itemType == fuelType)
        {
            ProcessFuelChange(amount);
        }
    }

    private void ProcessFuelChange(float amount)
    {        
        if(HasEnoughFuel(amount) && !effectIsEnabled)
        {
            SwitchEffectOn();
        }
        else if(!HasEnoughFuel(amount) && effectIsEnabled)
        {
            SwitchEffectOff();
        }
        FuelChangeEvent?.Invoke(amount);        
    }

    private void SwitchEffectOn()
    {
        effectIsEnabled = true;
        effectComponent.EnableEffect(effectType);
        EnableEffectEvent?.Invoke();
    }

    private void SwitchEffectOff()
    {
        effectIsEnabled = false;
        effectComponent.DisableEffect(effectType);
        DisableEffectEvent?.Invoke();
    }
    
    private bool HasEnoughFuel(float amount)
    {
        return amount >= fuelThreshold;
    }

    public int GetRange()
    {
        return effectComponent.GetEffect(effectType).Range;
    }

}

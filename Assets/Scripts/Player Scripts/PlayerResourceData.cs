using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//redo the resource stuff to be much more normal?  resources go places between turns...  
//but the player should have money...
//ok in that case... don't put the money in the resources thing...
public class PlayerResourceData : MonoBehaviour {
    [SerializeField]
    private bool initialProdIsOn = true;
    [SerializeField]
    private List<Building> buildings;
    private bool resourcesCalculatedForTurn = false;
    [SerializeField]
    private ResourceSet currentTurnResources;
    [SerializeField]
    private ResourceSet carryOverResources;
    

    //just have a few functions for buying?

    public void setProductionIsOn(Building building)
    {
        //check if prod is on is off...  
    }

    [SerializeField]
    private ResourceSet currentTurnDeductions;

    public UnityEvent OnBuildingsChanged;

    public int numDisplayResources
    {
        get
        {
            return displayResources.Count;
        }
    }

    public List<ResourceType> displayResourceTypes
    {
        get
        {
            List<ResourceType> retVal = new List<ResourceType>();
            foreach (Resource r in displayResources.resources)
            {
                retVal.Add(r.resourceType);
            }
            return retVal;
        }
    }

    public List<float> displayResourceAmounts
    {
        get
        {
            List<float> retVal = new List<float>();
            foreach (Resource r in displayResources.resources)
            {
                retVal.Add(r.currentAmount);
            }
            return retVal;
        }
    }

    public ResourceSet displayResources
    {
        get
        {
            return currentTurnResources + carryOverResources - currentTurnDeductions;
        }
    }

    public void StartTurn()
    {
        CalculateCurrentTurnResources();
    }

    public void Awake()
    {
        StartTurn();
    }
    
    private void CalculateCurrentTurnResources()
    {
        currentTurnResources.ClearResources();
        foreach(Building b in buildings)
        {
            if(b.productionIsOn)
            {
                currentTurnResources += b.ResourceProduction;
                currentTurnDeductions += b.IntakeRequirements;
            }
        }
        resourcesCalculatedForTurn = true;
    }

    public bool CanBuild(Building building)
    {
        if(resourcesCalculatedForTurn == false)
        {
            CalculateCurrentTurnResources();
        }

        if (displayResources.HasAtLeast(building.BuildRequirements))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //add the delay
    public void TurnReset()
    {
        resourcesCalculatedForTurn = false;
        //deduct from carryover if necessary...
        ResourceSet excess = currentTurnDeductions.GetExcessOver(currentTurnResources);
        carryOverResources -= excess;
        currentTurnDeductions.ClearResources();
        currentTurnResources.ClearResources();

        CalculateCurrentTurnResources();
        OnBuildingsChanged.Invoke();
    }

    //move to playercontroller rather than data...
    public void BuildBuilding(Building building)
    {
        buildings.Add(building);
        building.playerData = this;
        building.setProdIsOn(initialProdIsOn);
        currentTurnDeductions += building.BuildRequirements;
        //CalculateCurrentTurnResources();
        OnBuildingsChanged.Invoke();
    }
}

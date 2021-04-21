using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{
    private static int defaultConnectionLength = 3;

    public int maxConnectionLength = defaultConnectionLength; // manhattan distance?
    //don't serialize... or maybe do?  not sure...
    public UnityAction endTurnCallbacks;

    [SerializeField]
    private float costInMoney;
    public float CostInMoney
    {
        get => costInMoney;
    }

    //callbacks for i guess modifying connections?
    public void AddCallbacks(ConnectionDelegate onChange, ConnectionDelegate onAdd, ConnectionDelegate onDel)
    {
        connectionSet.onChangeConnectionEvent += onChange;
        connectionSet.onDeleteConnectionEvent += onDel;
        connectionSet.onAddConnectionEvent += onAdd;
    }

    public void ConnectForTransfer(Building inputBuilding, ResourceSet amount)
    {
        int dist = GetComponent<GridTransform>().ManhattanDistanceTo(inputBuilding.GetComponent<GridTransform>());
        Debug.Log($"Manhattan distance to input building: {dist}");
        int maxDist = Mathf.Min(maxConnectionLength, inputBuilding.maxConnectionLength);
        if(dist > maxDist)
        {
            Debug.Log("buildings too far to connect");
            return;
        }
        
        if(!(inputBuilding.AvailableProducedResources.HasAtLeast(amount)))
        {
            return;
        }
        BuildingConnection connection = connectionSet.FindConnection(inputBuilding);
        if (connection == null)
        {
            connection = CreateInboundConnection(inputBuilding); //side effects?
        }
        
        connection.ConnectionResources = amount;
        if(RemainingIntakeNeeds.IsZero())
        {
            setProdIsOn(true);
        }
        
        connection = inputBuilding.connectionSet.FindConnection(this);
        if(connection == null)
        {
            connection = inputBuilding.CreateOutboundConnection(this);
        }
        connection.ConnectionResources = amount;
        inputBuilding.CalculateCurrentResourceValues();
    }

    public PlayerResourceData playerData;
    [SerializeField]
    private ResourceSet intakeRequirements;
    public ResourceSet IntakeRequirements
    {
        get
        {
            return intakeRequirements;
        }
        set
        {
            intakeRequirements = new ResourceSet(value);
            CalculateCurrentResourceValues();
        }
    }
    [SerializeField]
    private ResourceSet resourceProduction;
    public ResourceSet ResourceProduction
    {
        get
        {
            return resourceProduction;
        }
        set
        {
            resourceProduction = new ResourceSet(value);
            CalculateCurrentResourceValues();
        }
    }

    [SerializeField]
    private ResourceSet buildRequirements;
    public ResourceSet BuildRequirements
    {
        get
        {
            return buildRequirements;
        }
    }
    [SerializeField]
    private ResourceSet AvailableProducedResources;
    
    public ResourceSet RemainingIntakeNeeds
    {
        get
        {
            return intakeRequirements - connectionSet.TotalIntake;
        }
    }
        //this should be... just amount taken in...
    public bool productionIsOn = false;
    [SerializeField]
    private ConnectionSet connectionSet = new ConnectionSet();
    
    public ResourceSet totalIntake
    {
        get => connectionSet.TotalIntake;
    }

    public void Awake()
    {
        connectionSet.parent = this;
    }

    private void CalculateCurrentResourceValues()
    {
        if (productionIsOn)
        {
            if(connectionSet.TotalIntake.HasAtLeast(intakeRequirements))
            {
                AvailableProducedResources = new ResourceSet(resourceProduction);
            }
            else
            {
                setProdIsOn(false);
            }
            
        }
        foreach (BuildingConnection c in connectionSet.GetConnectionsHavingDirection(BuildingConnection.ConnectionDirection.output))
        {
            AvailableProducedResources -= c.ConnectionResources;
        }
    }
  
    public bool setProdIsOn(bool value)
    {
        if(value == productionIsOn)
        {
            return value;
        }
        if(value == true)
        {
            if(connectionSet.TotalIntake.HasAtLeast(intakeRequirements))
            {
                productionIsOn = true;
            }
            else
            {
                bool goodConnectionFound = false;
                foreach(BuildingConnection c in connectionSet.GetConnectionsHavingDirection(BuildingConnection.ConnectionDirection.input))
                {
                    if(c.ConnectedBuilding.HasAvailableResources(intakeRequirements)) 
                    {
                        c.ConnectionResources = intakeRequirements;
                        c.ConnectedBuilding.connectionSet.FindConnection(this).ConnectionResources = intakeRequirements;
                        c.ConnectedBuilding.CalculateCurrentResourceValues();
                        goodConnectionFound = true;
                        break;
                    }
                }
                productionIsOn = goodConnectionFound;
            }
        }
        else if(value == false)
        {
            productionIsOn = false;

            foreach(BuildingConnection c in connectionSet.Connections)
            {
                //this doesn't work?
                var debit = c.ConnectionResources;
                c.RemoveResources(debit); // zero out the connection...
                c.ConnectedBuilding.connectionSet.FindConnection(this).RemoveResources(debit);
                c.ConnectedBuilding.CalculateCurrentResourceValues();
            }
        }
        CalculateCurrentResourceValues();
        return productionIsOn;
    }
    
    public bool HasAvailableResources(ResourceSet resources)
    {
        return AvailableProducedResources.HasAtLeast(resources);
    }
    
    public BuildingConnection CreateInboundConnection(Building target)
    {
        if(target == this)
        {
            throw new InvalidOperationException("Cannot form a connection to self");
        }
        BuildingConnection retVal = connectionSet.AddConnection(BuildingConnection.ConnectionDirection.input, ResourceSet.Nothing, target);
        return retVal;
    }
    public BuildingConnection CreateOutboundConnection(Building target)
    {
        if(target == this)
        {
            throw new InvalidOperationException("Cannot form a connection to self");
        }
        BuildingConnection retVal = connectionSet.AddConnection(BuildingConnection.ConnectionDirection.output, ResourceSet.Nothing, target);
        return retVal;
    }
}

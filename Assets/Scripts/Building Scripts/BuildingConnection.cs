using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingConnection
{
    public enum ConnectionDirection { input, output };
    [SerializeField]
    private ConnectionDirection connectionType;
    public ConnectionDirection connectionDirection
    {
        get
        {
            return connectionType;
        }
    }

    [SerializeField]
    private ResourceSet connectionResources;
    public ResourceSet ConnectionResources
    {
        get
        {
            return new ResourceSet(connectionResources);
        }
        set
        {
            connectionResources = new ResourceSet(value);
        }
    }
    [SerializeField]
    private Building connectedBuilding;
    public Building ConnectedBuilding
    {
        get
        {
            return connectedBuilding;
        }
    }




    public BuildingConnection(ConnectionDirection connectionDirection, ResourceSet resources, Building target)
    {
        connectionType = connectionDirection;
        connectionResources = resources;
        connectedBuilding = target;
    }

    public bool targetIs(Building building)
    {
        if (building == connectedBuilding)
            return true;
        else
            return false;
    }

    public void AddResources(ResourceSet resourceSet)
    {
        connectionResources += resourceSet;
    }

    public void RemoveResources(ResourceSet resourceSet)
    {
        connectionResources -= resourceSet;
        if(connectionResources.HasANegativeResource())
        {
            throw new InvalidOperationException("cannot deduct resources in a manner that would leave negative resources in this connection");
        }
    }
}

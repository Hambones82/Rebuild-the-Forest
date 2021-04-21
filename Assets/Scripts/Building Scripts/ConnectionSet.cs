using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ConnectionDelegate(BuildingConnection c);

[Serializable]
public class ConnectionSet 
{
    public event ConnectionDelegate onChangeConnectionEvent;
    public event ConnectionDelegate onDeleteConnectionEvent;
    public event ConnectionDelegate onAddConnectionEvent;

    public Building parent;
    [SerializeField]
    private List<BuildingConnection> connections = new List<BuildingConnection>();

    public List<BuildingConnection> Connections
    {
        get
        {
            return connections;
        }
    }

    public bool HasConnectionTo(Building building, BuildingConnection.ConnectionDirection direction)
    {
        BuildingConnection connection = FindConnection(building);
        if(connection == null)
        {
            return false;
        }
        else if(connection.connectionDirection != direction)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool HasConnectionTo(Building building)
    {
        BuildingConnection connection = FindConnection(building);
        if(connection != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeleteConnection(BuildingConnection connection)
    {
        onDeleteConnectionEvent(connection);
        connections.Remove(connection);
    }

    public BuildingConnection AddConnection(BuildingConnection.ConnectionDirection connectionDirection, ResourceSet resources, Building target)
    {
        if(target == parent)
        {
            throw new InvalidOperationException("Cannot form connection to self");
        }
        BuildingConnection retVal;
        if(FindConnection(target) != null)
        {
            throw new InvalidOperationException($"a connection from {parent.name} to {target.name} already exists");
        }
        retVal = new BuildingConnection(connectionDirection, resources, target);
        connections.Add(retVal);
        //not the best...
        onAddConnectionEvent(retVal);
        return retVal;
    }

    public void SetConnectionAmount(ResourceSet resources, Building target)
    {
        BuildingConnection connection = FindConnection(target);
        if (connection == null)
        {
            throw new InvalidOperationException($"a connection from {parent.name} to {target.name} does not exist");
        }
        connection.ConnectionResources = resources;
        onChangeConnectionEvent(connection);
    }
    /*
    public void AddResourcesToConnection(BuildingConnection.ConnectionDirection connectionDirection, ResourceSet resources, Building target)
    {
        BuildingConnection connection = FindConnection(target);
        if(connection == null)
        {
            throw new InvalidOperationException($"a connection from {parent.name} to {target.name} does not exist");
        }
        if(connectionDirection != connection.connectionDirection)
        {
            throw new InvalidOperationException($"cannot add to a connection of a different type");
        }
        connection.AddResources(resources);
        onChangeConnectionEvent(connection);
    }*/

    public BuildingConnection FindConnection(Building target)
    {
        foreach (BuildingConnection c in connections)
        {
            if (c.targetIs(target))
            {
                return c;
            }
        }
        return null;
    }
    public List<BuildingConnection> GetConnectionsHavingDirection(BuildingConnection.ConnectionDirection direction)
    {
        List<BuildingConnection> retVal = new List<BuildingConnection>();
        foreach(BuildingConnection c in connections)
        {
            if (c.connectionDirection == direction)
            {
                retVal.Add(c);
            }
                
        }
        return retVal;
    }

    public ResourceSet TotalIntake
    {
        get
        {
            ResourceSet retVal = new ResourceSet();
            foreach (BuildingConnection c in GetConnectionsHavingDirection(BuildingConnection.ConnectionDirection.input))
            {
                retVal += c.ConnectionResources;
            }
            return retVal;
        }
    }



}

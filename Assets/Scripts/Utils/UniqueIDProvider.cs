using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UniqueIDProvider 
{
    private SortedSet<int> freeIDs;
    private int highestAssignedID;

    public UniqueIDProvider() 
    {
        freeIDs = new SortedSet<int>();
        highestAssignedID = -1;
    }

    public override string ToString()
    {
        string retval = "";
        foreach (int i in freeIDs) { retval += $"{i}, "; }
        retval += "\n";
        retval += $"Highest assigned: {highestAssignedID}";
        return retval;
    }

    public int GetNewID()
    {
        if (freeIDs.Count == 0)
        {
            highestAssignedID++;
            return highestAssignedID;
        }
        int retval = freeIDs.Min;
        freeIDs.Remove(retval);
        return retval;
    }

    //let's just completely forget about the compression of the highest portion of the range.
    //just keep it all open.
    public void ReturnID(int id)
    {
        if (id < 0 || id > highestAssignedID)
        {
            throw new InvalidOperationException($"ID value {id} is not in use as it is out of bounds.");
        }
        //this logic is not correct...
        //free IDs doesn't contain all IDs in use...
        if (freeIDs.Contains(id))
        {
            throw new InvalidOperationException($"ID value {id} cannot be returned as it is already free.");
        }
        freeIDs.Add(id);
    }
}

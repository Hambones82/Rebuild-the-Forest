//probably want to convert this into a class.
//with a class, you'd want to specify whether it's like a "raw" resource, a building, maybe even money... although maybe we could
//get rid of the money since we refer to that explicitly?
//if we make this a class, we will need to implement equality at least.
using UnityEngine;
using System;

//wait, what am i doing??
//ok... select a type...  enum for that...
//then out of those types, select from that...
[System.Serializable]
public class ResourceType
{
    [SerializeField]
    private ResourceCategory resourceSelection;
    [SerializeField]
    private RawResourceType rawResourceType;
    [SerializeField] //why do we even have this... ok... it's because... it can make a building.  but shouldnt' we just make this 
                     //flat anyway and have some other thing link it to buildings?
    private BuildingType buildingType;

    public ResourceType(ResourceType otherResource)
    {
        resourceSelection = otherResource.resourceSelection;
        rawResourceType = otherResource.rawResourceType;
        buildingType = otherResource.buildingType;
    }

    public ResourceType(RawResourceType rawRType)
    {
        resourceSelection = ResourceCategory.RawResource;
        rawResourceType = rawRType;
    }

    public ResourceType(BuildingType bType)
    {
        resourceSelection = ResourceCategory.Building;
        buildingType = bType;
    }

    public override string ToString()
    {
        string retVal;
        if(resourceSelection == ResourceCategory.Building)
        {
            retVal = buildingType.ToString();
        }
        else if(resourceSelection == ResourceCategory.RawResource)
        {
            retVal = rawResourceType.ToString();
        }
        else
        {
            retVal = "invalid Resource Type";
        }
        return retVal;
    }

    public override bool Equals(object obj)
    {
        Type otherType = obj.GetType();
        bool retVal = false;
        if ((obj == null) || !this.GetType().Equals(otherType))
        {
            retVal = false;
        }
        else
        {
            ResourceType otherObj = (ResourceType)obj;
            if (otherObj.resourceSelection != resourceSelection)
            {
                retVal = false;
            }
            else if (resourceSelection == ResourceCategory.Building)
            {
                retVal = buildingType == otherObj.buildingType;
            }
            else if (resourceSelection == ResourceCategory.RawResource)
            {
                retVal = rawResourceType == otherObj.rawResourceType;
            }
        }

        //Debug.Log(retVal);
        return retVal;
    }

    public static bool operator ==(ResourceType r1, ResourceType r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(ResourceType r1, ResourceType r2)
    {
        return !r1.Equals(r2);
    }

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }
}


public enum ResourceCategory
{
    RawResource,
    Building
}

public enum RawResourceType
{
    Wood,
    BuyContract,
    BuildContract,
    MilledWood,
    Worker,
    Food
};

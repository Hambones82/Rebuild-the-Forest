using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ResourceDict : ListDict<ResourceType, Resource> { }

[System.Serializable]
public class ResourceSet //, IEquatable<ResourceSet>, IComparable<ResourceSet> struct?
{
    public List<Resource> resources = new List<Resource>();
    public static readonly ResourceSet Nothing = new ResourceSet();

    public int Count
    {
        get
        {
            return resources.Count;
        }
    }


    public float this[ResourceType resourceType]
    {
        get
        {
            foreach(Resource resource in resources)
            {
                if (resource.resourceType == resourceType)
                    return resource.currentAmount;
            }
            return 0;
        }
        set
        {
            foreach (Resource resource in resources)
            {
                if (resource.resourceType == resourceType)
                {
                    resource.currentAmount = value;
                    break;
                }
            }
        }
    }

    public void AddResource(Resource resource)
    {
        foreach(Resource r in resources)
        {
            if(r.resourceType == resource.resourceType)
            {
                r.currentAmount+=resource.currentAmount;
                return;
            }
        }
        resources.Add(new Resource(resource));
    }

    public ResourceSet() { }

    public ResourceSet(ResourceSet resourceSet)
    {
        foreach(Resource resource in resourceSet.resources)
        {
            resources.Add(new Resource(resource));
        }
    }

    public ResourceSet(List<Resource> resources)
    {
        foreach(Resource resource in resources)
        {
            resources.Add(new Resource(resource));
        }
    }

    public bool HasANegativeResource()
    {
        foreach(Resource r in resources)
        {
            if(r.currentAmount < 0)
            {
                return true;
            }
        }
        return false;
    }

    public ResourceSet GetExcessOver(ResourceSet target)
    {
        ResourceSet temp = target - this;
        ResourceSet retVal = new ResourceSet();
        foreach(Resource r in temp.resources)
        {
            if(r.currentAmount < 0)
            {
                retVal.AddResource(-r);
            }
        }
        return retVal;
    }

    public void ClearResources()
    {
        resources.Clear();
    }

    public static ResourceSet operator +(ResourceSet rs1, ResourceSet rs2)
    {
        ResourceSet retVal = new ResourceSet(rs1);
        foreach(Resource resource in rs2.resources)
        {
            retVal.AddResource(resource);
        }
        return retVal;
    }

    public static ResourceSet operator -(ResourceSet rs1, ResourceSet rs2)
    {
        ResourceSet retVal = new ResourceSet(rs1);
        foreach (Resource resource in rs2.resources)
        {
            retVal.AddResource(-resource);
        }
        return retVal;
    }

    public bool HasAtLeast(ResourceSet other)
    {
        bool retVal = true;
        foreach(Resource resource in other.resources)
        {
            if (this[resource.resourceType] < other[resource.resourceType])
            {
                retVal = false;
            }
        }
        return retVal;
    }

    public bool IsZero()
    {
        foreach(Resource r in resources)
        {
            if(r.currentAmount != 0)
            {
                return false;
            }
        }
        return true;
    }
}

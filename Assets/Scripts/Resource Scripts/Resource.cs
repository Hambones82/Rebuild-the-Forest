using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Resource {
    public const float minResource = 0;
    public const float maxResource = 100;

    public ResourceType resourceType;

    /*
    [SerializeField]
    private Money costInMoney;
    public Money CostInMoney
    {
        get => costInMoney;
    }
    */
    
    [SerializeField] //slider for range?
    [Range(minResource, maxResource)]
    private float _currentAmount;
    public float currentAmount
    {
        get
        {
            return _currentAmount;
        }
        set
        {
            _currentAmount = value;
        }
    }

    public Resource(float amount, ResourceType rType)
    {
        _currentAmount = amount;
        resourceType = new ResourceType(rType);
    }

    public Resource(Resource resource)
    {
        _currentAmount = resource.currentAmount;
        resourceType = resource.resourceType;
    }

    public static Resource operator -(Resource rs1)
    {
        Resource retVal = new Resource(rs1);
        retVal.currentAmount = -1 * (rs1._currentAmount);
        return retVal;
    }
}

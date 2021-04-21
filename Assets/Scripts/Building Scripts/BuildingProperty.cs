using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//wtf is this even?  probably just get rid of this... this ist stupid...
public enum BuildingPropertyType
{
    name
}

public abstract class BuildingProperty  {
    public string label;
    public BuildingPropertyType type;
    public string GetLabel()
    {
        return label;
    }
    public abstract string GetValueAsString();
    public abstract object value
    {
      get;
    }
    
}

[System.Serializable]
public class BuildingPropertyInt : BuildingProperty
{
    [SerializeField]
    private int _value;
    public override string GetValueAsString()
    {
        return _value.ToString();
    }

    public override object value
    {
        get
        {
            return _value;
        }
    }

}

[System.Serializable]
public class BuildingPropertyString : BuildingProperty
{
    [SerializeField]
    private string _value;
    public override string GetValueAsString()
    {
        return _value;
    }
    public override object value
    {
        get
        {
            return _value;
        }
    }
}

[System.Serializable]
public class BuildingPropertyBool: BuildingProperty
{
    [SerializeField]
    private bool _value;
    public override string GetValueAsString()
    {
        return _value.ToString();
    }
    public override object value
    {
        get
        {
            return _value;
        }
    }
}

//to iterate through all properties?  is all of this necessary???  what's the point of a bunch of 
[System.Serializable]
public class BuildingPropertySet : IEnumerable
{
    public List<BuildingPropertyInt> buildingPropertyIntList;
    public List<BuildingPropertyBool> buildingPropertyBoolList;
    public List<BuildingPropertyString> buildingPropertyStringList;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }

    public BuildingPropertySetEnum GetEnumerator()
    {
        return new BuildingPropertySetEnum(buildingPropertyIntList, buildingPropertyBoolList, buildingPropertyStringList);
    }

}

public class BuildingPropertySetEnum : IEnumerator
{
    public List<BuildingPropertyInt> buildingPropertyIntList;
    public List<BuildingPropertyBool> buildingPropertyBoolList;
    public List<BuildingPropertyString> buildingPropertyStringList;

    private enum PropertyEnumerationState
    {
        intList,
        boolList,
        stringList
    }

    private PropertyEnumerationState propertyEnumerationState = PropertyEnumerationState.intList;

    private int intPosition = -1;
    private int boolPosition = -1;
    private int stringPosition = -1;

    

    public BuildingPropertySetEnum(List<BuildingPropertyInt> intList, List<BuildingPropertyBool> boolList, List<BuildingPropertyString> stringList)
    {
        buildingPropertyIntList = intList;
        buildingPropertyBoolList = boolList;
        buildingPropertyStringList = stringList;
    }

    public void Reset()
    {
        intPosition = -1;
        boolPosition = -1;
        stringPosition = -1;
        propertyEnumerationState = PropertyEnumerationState.intList;
    }

    public bool MoveNext()
    {
        while (true)
        {
            switch (propertyEnumerationState)
            {
                case PropertyEnumerationState.intList:
                    intPosition++;
                    if (intPosition < buildingPropertyIntList.Count)
                    {
                        return true;
                    }
                    propertyEnumerationState = PropertyEnumerationState.boolList;
                    break;
                case PropertyEnumerationState.boolList:
                    boolPosition++;
                    if (boolPosition < buildingPropertyBoolList.Count)
                    {
                        return true;
                    }
                    propertyEnumerationState = PropertyEnumerationState.stringList;
                    break;
                case PropertyEnumerationState.stringList:
                    stringPosition++;
                    if (stringPosition < buildingPropertyStringList.Count)
                    {
                        return true;
                    }
                    return false;
            }
        }
    }

    public object Current
    {
        get
        {
            switch(propertyEnumerationState)
            {
                case PropertyEnumerationState.intList:
                    return buildingPropertyIntList[intPosition];
                case PropertyEnumerationState.boolList:
                    return buildingPropertyBoolList[boolPosition];
                case PropertyEnumerationState.stringList:
                    return buildingPropertyStringList[stringPosition];
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this should definitely be tested.  also, do we want this to be system.serializable?  probably
[System.Serializable]
public class ALSet<T, X> where T:TypedNumericalComparable<X> 
{
    [SerializeField]
    private List<T> setElements;
    public List<T> SetElements { get => setElements; }

    public ALSet()
    {
        setElements = new List<T>();
    }

    public bool HasAtLeast(ALSet<T, X> otherSet)
    {
        bool retVal = true;
        foreach(T element in otherSet.setElements) //this isn't exactly correct...
        {
            T thisElement = Get(element.Type);
            if(thisElement == null)
            {
                
                if(element.Data == 0)
                {
                    break;
                }
                else
                {
                    retVal = false;
                    break;
                }
            }
            else if(thisElement.Data.CompareTo(element.Data) < 0)
            {
                retVal = false;
                break;
            }
        }
        return retVal;
    }

    public T Get(X key)
    {
        foreach(T element in setElements)
        {
            if (EqualityComparer<X>.Default.Equals(element.Type, key))
                return element;
        }
        return null;
    }
}

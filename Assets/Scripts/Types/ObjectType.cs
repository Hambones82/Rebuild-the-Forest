using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//T is the underlying data type that this "type" object is meant to represent
//in other words, the object type is supposed to replace enums as a "typing" system for the game
//such enums are typically associated with a class -- the typing enum provides a typing system for such class
//in an example, the class is Building, and the objecttype describes buildings for that type
//that underlying class is the parameter T

    //in the building usage, the object is a building prefab.
    //what about for the effect type?  maybe... efect.instance?
public abstract class ObjectType<T> : ScriptableObject
{
    public abstract T GetObject();
}

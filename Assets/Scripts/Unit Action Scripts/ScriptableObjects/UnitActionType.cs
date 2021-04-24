using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitActionType : ScriptableObject
{
    public abstract UnitAction GetAction();
    
}

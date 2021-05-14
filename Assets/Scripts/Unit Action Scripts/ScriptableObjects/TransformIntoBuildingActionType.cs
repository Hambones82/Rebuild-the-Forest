using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformIntoBuildingActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        return ObjectPool.Get<TransformIntoBuildingAction>();
    }
}

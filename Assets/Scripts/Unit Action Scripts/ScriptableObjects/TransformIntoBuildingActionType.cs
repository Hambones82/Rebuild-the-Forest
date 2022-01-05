using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformIntoBuildingActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        var retVal = ObjectPool.Get<TransformIntoBuildingAction>();
        retVal.actionType = this;
        return retVal;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move Action Type", menuName = "ScriptableObjects/Types/Move Action Type")]
public class MoveActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        var retVal = ObjectPool.Get<MoveAction>();
        retVal.actionType = this;
        return retVal;
    }
}

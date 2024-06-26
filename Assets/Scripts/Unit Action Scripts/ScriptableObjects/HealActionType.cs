using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Action Type", menuName = "ScriptableObjects/Types/Heal Action Type")]
public class HealActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        var retVal = ObjectPool.Get<HealAction>();
        retVal.actionType = this;
        return retVal;
    }
}

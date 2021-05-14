using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clean Pollution Action Type", menuName = "ScriptableObjects/Types/Clean Pollution Action Type")]
public class CleanPollutionActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        return ObjectPool.Get<CleanPollutionAction>();
    }
}

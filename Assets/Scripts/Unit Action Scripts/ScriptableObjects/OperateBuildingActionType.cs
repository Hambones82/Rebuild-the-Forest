using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Operate Building Action Type", menuName = "ScriptableObjects/Types/Operate Building Action Type")]
public class OperateBuildingActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        return ObjectPool.Get<OperateAction>();
    }
}

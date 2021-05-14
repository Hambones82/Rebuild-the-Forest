using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Move Action Type", menuName = "ScriptableObjects/Types/Move Action Type")]
public class MoveActionType : UnitActionType
{
    public override UnitAction GetObject()
    {
        return ObjectPool.Get<MoveAction>();
    }
}

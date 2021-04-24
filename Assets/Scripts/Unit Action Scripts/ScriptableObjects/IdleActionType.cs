using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Action Type", menuName = "ScriptableObjects/Types/Idle Action Type")]
public class IdleActionType : UnitActionType
{
    public override UnitAction GetAction()
    {
        return IdleAction.Instance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(UnitActionController))]
public class ActorUnitContextClick : ContextClickComponent
{
    UnitActionController actorUnit;
    GridMap gridMap;

    public override void Awake()
    {
        actorUnit = GetComponent<UnitActionController>();
        if (actorUnit == null)
            throw new InvalidOperationException("ActorUnitContextClick cannot find a correcponding ActorUnit - this should not happen");
        gridMap = GetComponent<GridTransform>().gridMap;
    }

    public override void DoContextClick(Vector2Int mapPosition)
    {

        MoveAction action = ObjectPool.Get<MoveAction>();
        action.Initialize(gameObject);
        action.SetMapDestination(mapPosition);
        actorUnit.DoAction(action);
        Pollution target = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);
        if (target!=null)
        {
            CleanPollutionAction cpAction = ObjectPool.Get<CleanPollutionAction>();
            cpAction.Initialize(gameObject);
            cpAction.SetTargetPollution(target);
            actorUnit.DoAction(cpAction);
        }
        //Debug.Log("doing context click");
    }
}



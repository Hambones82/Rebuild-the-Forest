using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(UnitActionController))]
public class ActorUnitContextClick : ContextClickComponent
{
    UnitActionController actorUnit;
    GridMap gridMap;

    [SerializeField]
    UnitActionType moveAction;
    [SerializeField]
    UnitActionType cleanPollutionAction;

    public override void Awake()
    {
        actorUnit = GetComponent<UnitActionController>();
        if (actorUnit == null)
            throw new InvalidOperationException("ActorUnitContextClick cannot find a correcponding ActorUnit - this should not happen");
        gridMap = GetComponent<GridTransform>().gridMap;
    }

    public override void DoContextClick(Vector2Int mapPosition)
    {

        MoveAction action = (MoveAction)moveAction.GetAction();
        action.Initialize(gameObject);
        action.SetMapDestination(mapPosition);
        actorUnit.DoAction(action);
        Pollution target = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);
        if (target!=null)
        {
            CleanPollutionAction cpAction = (CleanPollutionAction)cleanPollutionAction.GetAction();
            cpAction.Initialize(gameObject);
            cpAction.SetTargetPollution(target);
            actorUnit.DoAction(cpAction);
        }
        //Debug.Log("doing context click");
    }
}



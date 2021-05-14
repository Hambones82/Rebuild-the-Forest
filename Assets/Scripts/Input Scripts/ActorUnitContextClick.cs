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
#pragma warning disable CS0649 // Field 'ActorUnitContextClick.moveAction' is never assigned to, and will always have its default value null
    UnitActionType moveAction;
#pragma warning restore CS0649 // Field 'ActorUnitContextClick.moveAction' is never assigned to, and will always have its default value null
    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitContextClick.cleanPollutionAction' is never assigned to, and will always have its default value null
    UnitActionType cleanPollutionAction;
#pragma warning restore CS0649 // Field 'ActorUnitContextClick.cleanPollutionAction' is never assigned to, and will always have its default value null

    public override void Awake()
    {
        actorUnit = GetComponent<UnitActionController>();
        if (actorUnit == null)
            throw new InvalidOperationException("ActorUnitContextClick cannot find a correcponding ActorUnit - this should not happen");
        gridMap = GetComponent<GridTransform>().gridMap;
    }

    public override void DoContextClick(Vector2Int mapPosition)
    {

        MoveAction action = (MoveAction)moveAction.GetObject();
        action.Initialize(gameObject);
        action.SetMapDestination(mapPosition);
        actorUnit.DoAction(action);
        Pollution target = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);
        if (target!=null)
        {
            CleanPollutionAction cpAction = (CleanPollutionAction)cleanPollutionAction.GetObject();
            cpAction.Initialize(gameObject, target);
            actorUnit.DoAction(cpAction);
        }
        //Debug.Log("doing context click");
    }
}



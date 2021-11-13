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
    [SerializeField]
    UnitActionType operateBuildingAction;

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
        BuildingComponentOperator targetBuilding = gridMap.GetObjectAtCell<BuildingComponentOperator>(mapPosition, MapLayer.buildings);
        if (targetBuilding != null)
        {
            OperateAction obAction = (OperateAction)operateBuildingAction.GetObject();
            obAction.Initialize(gameObject, targetBuilding);
            actorUnit.DoAction(obAction);
        }
        else
        {
            Pollution target = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);
            if (target != null)
            {
                CleanPollutionAction cpAction = (CleanPollutionAction)cleanPollutionAction.GetObject();
                cpAction.Initialize(gameObject, target);
                actorUnit.DoAction(cpAction);
            }
        }
        //Debug.Log("doing context click");
    }
}



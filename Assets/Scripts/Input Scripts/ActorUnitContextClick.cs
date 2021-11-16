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
        Pollution targetPollution = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);
        if (targetPollution != null)
        {
            CleanPollutionAction cpAction = (CleanPollutionAction)cleanPollutionAction.GetObject();
            cpAction.Initialize(gameObject, targetPollution);
            actorUnit.DoAction(cpAction);
        }
        else if (targetBuilding != null)
        {
            OperateAction obAction = (OperateAction)operateBuildingAction.GetObject();
            obAction.Initialize(gameObject, targetBuilding);
            actorUnit.DoAction(obAction);
        }
        
        //Debug.Log("doing context click");
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(UnitActionController))]
public class ActorUnitContextClick : ContextClickComponent
{
    ActorUnit actorUnit;
    UnitActionController actorUnitController;
    GridMap gridMap;

    [SerializeField]
    UnitActionType moveAction;
    [SerializeField]
    UnitActionType cleanPollutionAction;
    [SerializeField]
    UnitActionType operateBuildingAction;
    [SerializeField]
    UnitActionType healAction;

    public override void Awake()
    {
        actorUnit = GetComponent<ActorUnit>();
        actorUnitController = GetComponent<UnitActionController>();
        if (actorUnitController == null)
            throw new InvalidOperationException("ActorUnitContextClick cannot find a correcponding ActorUnit - this should not happen");
        gridMap = GetComponent<GridTransform>().gridMap;
    }

    public override void DoContextClick(Vector2Int mapPosition)
    {
        ActorUnit targetActorUnit = gridMap.GetObjectAtCell<ActorUnit>(mapPosition, MapLayer.playerUnits);
        List<UnitAction> actionsToAdd = new List<UnitAction>();
        if (targetActorUnit != null && targetActorUnit != actorUnit)
        {
            MoveAction action = (MoveAction)moveAction.GetObject();
            action.Initialize(gameObject);
            action.SetMapDestination(mapPosition, true);
            actionsToAdd.Add(action);
            //actorUnit.DoAction(action);
            HealAction hAction = (HealAction)healAction.GetObject();
            hAction.Initialize(gameObject);
            hAction.SetTargetActor(targetActorUnit);
            actionsToAdd.Add(hAction);
            //actorUnit.DoAction(hAction);            
        }
        else
        {
            MoveAction action = (MoveAction)moveAction.GetObject();
            action.Initialize(gameObject);
            action.SetMapDestination(mapPosition);
            actionsToAdd.Add(action);
            //actorUnit.DoAction(action);
            BuildingComponentOperator targetBuilding = gridMap.GetObjectAtCell<BuildingComponentOperator>(mapPosition, MapLayer.buildings);
            Pollution targetPollution = gridMap.GetObjectAtCell<Pollution>(mapPosition, MapLayer.pollution);

            if (targetPollution != null)
            {
                CleanPollutionAction cpAction = (CleanPollutionAction)cleanPollutionAction.GetObject();
                cpAction.Initialize(gameObject, targetPollution);
                actionsToAdd.Add(cpAction);
                //actorUnit.DoAction(cpAction);
            }
            else if (targetBuilding != null)
            {
                OperateAction obAction = (OperateAction)operateBuildingAction.GetObject();
                obAction.Initialize(gameObject, targetBuilding);
                actionsToAdd.Add(obAction);
            }
        }
        if(actionsToAdd.Count >0)
        {
            actorUnitController.CancelAllActions();
            foreach(UnitAction action in actionsToAdd)
            {
                actorUnitController.DoAction(action);
            }
        }
    }
}



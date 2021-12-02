using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateAction : UnitActionWithTarget<BuildingComponentOperator>, IObjectPoolInterface
{
    private StatLine operatingStat;
    private BuildingComponentOperator buildingComponentOperator;
    private bool cancel = false;
    private Building targetBuilding;

    public override bool CanDo()
    {
        return !targetBuilding.GetComponent<GridTransform>().AtLeastOneCellIsOccupiedBy(MapLayer.pollution);
    }

    public override void Initialize(GameObject inGameObject, BuildingComponentOperator inBuildingComponentOperator)
    {
        base.Initialize(inGameObject, inBuildingComponentOperator);
        operatingStat = inGameObject.GetComponent<ActorUnitStats>().Operating;
        buildingComponentOperator = inBuildingComponentOperator;
        cancel = false;
        targetBuilding = inBuildingComponentOperator.GetComponent<Building>();
        targetBuilding.onBuildingDeathEvent += BuildingDied;
    }

    public void BuildingDied(Building building)
    {
        cancel = true;
    }

    public override bool AdvanceAction(float dt)
    {
        if(cancel || !CanDo())
        {
            return false;
        }
        else
        {
            return buildingComponentOperator.Operate(dt, gameObject);
        }        
    }

    public OperateAction()
    {
        actionName = "Operating";
    }

    public override void EndAction()
    {
        targetBuilding.onBuildingDeathEvent -= BuildingDied;
        ObjectPool.Return(this);
    }

    public override void ImproveStat(float ImproveAmount)
    {
        operatingStat.ImproveStat(ImproveAmount);
    }

    public override void StartAction()
    {
        
    }

    public void Reset()
    {
        
    }

    public override void Cancel()
    {
        cancel = true;
    }
}

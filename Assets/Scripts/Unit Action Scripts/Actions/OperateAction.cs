using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateAction : UnitActionWithTarget<BuildingComponentOperator>, IObjectPoolInterface
{
    private StatLine operatingStat;
    private BuildingComponentOperator buildingComponentOperator;

    public override void Initialize(GameObject inGameObject, BuildingComponentOperator inBuildingComponentOperator)
    {
        base.Initialize(inGameObject, inBuildingComponentOperator);
        operatingStat = inGameObject.GetComponent<ActorUnitStats>().Operating;
        buildingComponentOperator = inBuildingComponentOperator;
        //I guess also a callback for when the building dies?
    }

    public override bool AdvanceAction(float dt)
    {
        return buildingComponentOperator.Operate(dt);
    }

    public override void EndAction()
    {
        throw new System.NotImplementedException();
    }

    public override void ImproveStat(float ImproveAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void StartAction()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}

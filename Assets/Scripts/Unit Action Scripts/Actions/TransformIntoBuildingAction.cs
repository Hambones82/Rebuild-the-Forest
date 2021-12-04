using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformIntoBuildingAction : UnitActionWithTarget<Building>, IObjectPoolInterface
{
    //so...  gameobject is the actorunit.
    //building is the building


    private float timerValue = 0;
    private float buildRate = 1;
    public void Reset() { }
    
    private Building building { get => target; }

    public Vector3 worldBuildLocation;

    public Vector2Int mapBuildLocation;

    private ActorUnit actorUnit;

    private bool cancel;

    public TransformIntoBuildingAction()
    {
        actionName = "Transform Into Building";
    }

    public override bool CanDo()
    {
        //need to check for stat and inventory requirements.
        bool mapCanHaveBuilding = BuildingManager.Instance.CanPlaceBuildingAt(building, mapBuildLocation);
        bool actorUnitHasInventory =
            (!building.ItemRequiredToBuild || actorUnit.GetComponent<Inventory>().HasItem(building.RequiredItem));
        return mapCanHaveBuilding && actorUnitHasInventory;
    }

    public override bool AdvanceAction(float dt)
    {
        //Debug.Log("building a building");
        if(cancel)
        {
            //Debug.Log("canceling build");
            return false;
        }
        timerValue += dt;
        
        if(timerValue >= buildRate)
        {
            if(CanDo())
            {
                BuildingManager.Instance.SpawnBuildingAt(building, worldBuildLocation);
                if(building.KillActorUnit)
                {
                    ActorUnitManager.Instance.KillActorUnit(actorUnit);
                }
                if(building.RequiredItem)
                {
                    actorUnit.GetComponent<Inventory>().RemoveItem(building.RequiredItem);
                }
            }
            return false;
        }
        return true;
    }

    public override void EndAction()
    {
        ObjectPool.Return(this);
    }

    public override void ImproveStat(float ImproveAmount) { }

    public override void Initialize(GameObject inGameObject, Building inComponent)
    {
        base.Initialize(inGameObject, inComponent);
        actorUnit = inGameObject.GetComponent<ActorUnit>();
        cancel = false;
    }

    public override void StartAction()
    {
        timerValue = 0;
    }

    public override void Cancel()
    {
        cancel = true;
    }
}

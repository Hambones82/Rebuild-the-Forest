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

    public Vector3 buildLocation;

    private ActorUnit actorUnit;

    public TransformIntoBuildingAction()
    {
        actionName = "Transform Into Building";
    }

    public override bool AdvanceAction(float dt)
    {
        timerValue += dt;
        
        if(timerValue >= buildRate)
        {
            //build building... but we don't seem to have any of this idea, e.g., building or location.  
            //location can be center of actor unit's position converted to top left corner...
            BuildingManager.Instance.SpawnBuildingAt(building, buildLocation);
            //kill the actor unit.
            ActorUnitManager.Instance.KillActorUnit(actorUnit);
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
    }

    public override void StartAction()
    {
        timerValue = 0;
    }
}

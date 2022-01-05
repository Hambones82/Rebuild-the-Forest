using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealAction : UnitAction, IObjectPoolInterface
{
    private ActorUnit targetActorUnit;
    private bool cancel = false;

    private GridTransform gridTransform;
    private GridTransform targetGridTransform;
    private ActorUnitHealthComponent targetHealth;

    [SerializeField]
    private float _healPeriod = .1f;
    [SerializeField]
    private float _healAmount = .9f;
    private float healTimer = 0;

    public HealAction()
    {
        actionName = "Heal";
    }

    public void SetTargetActor(ActorUnit actorUnit)
    {
        targetActorUnit = actorUnit;
        targetGridTransform = actorUnit.GetComponent<GridTransform>();
        targetHealth = actorUnit.GetComponent<ActorUnitHealthComponent>();
    }

    public override void Initialize(GameObject inGameObject) //gameobject is the object that is doing the action.
    {
        //Debug.Log("initialize is called for move action");
        base.Initialize(inGameObject);
        cancel = false;
        gridTransform = gameObject.GetComponent<GridTransform>();
        //initialize some values such as the target...?
        healTimer = 0;
    }

    public override bool AdvanceAction(float dt, out float progressAmount)
    {
        if(cancel)
        {
            progressAmount = 0;
            return false;
        }
        else if(CanDo())
        {
            healTimer += dt;
            while(healTimer >= _healPeriod)
            {
                targetHealth.Heal(_healAmount);
                healTimer -= _healPeriod;
            }
            progressAmount = targetHealth.Health / targetHealth.MaxHealth;
            return true;
        }
        else
        {
            progressAmount = 0;
            return false;
        }
    }

    public override void Cancel()
    {
        cancel = true;
    }

    public override bool CanDo()
    {
        List<Vector2Int> adjacentPositions = gridTransform.GetAdjacentTiles();
        return adjacentPositions.Contains(targetGridTransform.topLeftPosMap);
    }

    public override void EndAction()
    {
        ObjectPool.Return(this);
    }

    public override void ImproveStat(float ImproveAmount)
    {
        //for now, do nothing
    }

    public override void StartAction()
    {
       
    }

    public void Reset()
    {
        
    }
}

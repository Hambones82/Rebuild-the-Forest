using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//starting thing should be at current position...  actually... this big will be fixed with further coding for whatever...
[System.Serializable]
public class MoveAction : UnitAction, IObjectPoolInterface
{
    private Vector3 targetWorldPos; // in world coords???
    private Vector3 startWorldPos;
    private float totalTravelDistance;
    private float currentTravelDistance;
    private Vector2Int targetMapPos;//grid coords
    private GridTransform gridTransform;
    private StatLine speedStat;
    private Transform transform;

    private bool arrivedAtWorldCoords = false;
    private bool arrivedAtMapCoords = false;

    public MoveAction()
    {
        actionName = "Move";
    }

    public override void EndAction()
    {
        ObjectPool.Return(this);
    }

    public override void Initialize(GameObject inGameObject) 
    {
        //Debug.Log("initialize is called for move action");
        base.Initialize(inGameObject);
        gridTransform = inGameObject.GetComponent<GridTransform>();
        speedStat = inGameObject.GetComponent<ActorUnitStats>()?.MovementSpeed;
        if (gridTransform == null || speedStat == null)
        {
            throw new InvalidOperationException("cannot create an actorunitmovingstate for a gameobject that does not have a grid transform or an actorUnit");
        }
        transform = inGameObject.transform;
        targetWorldPos = transform.position;
        targetMapPos = gridTransform.topLeftPosMap;
        arrivedAtMapCoords = false;
        arrivedAtWorldCoords = false;
        currentTravelDistance = 0;
        totalTravelDistance = 0;
    }

    public override void StartAction()
    {
        startWorldPos = transform.position;
        totalTravelDistance = Vector3.Distance(startWorldPos, targetWorldPos);
    }

    public override bool AdvanceAction(float dt)
    {
        bool continueMovement = false;
        if (!arrivedAtWorldCoords)
        {
            currentTravelDistance += dt * (Mathf.Floor(speedStat.Amount));
            float normalizedTravelDistance = currentTravelDistance / totalTravelDistance;
            Vector3 newPos = Vector3.Lerp(startWorldPos, targetWorldPos, normalizedTravelDistance);
            gridTransform.MoveToWorldCoords(newPos);
            if (normalizedTravelDistance >= 1)
            {
                arrivedAtMapCoords = true;
                arrivedAtWorldCoords = true;
            }
            else
            {
                arrivedAtWorldCoords = false;
                arrivedAtMapCoords = false;
            }
            
        }
        
        if (arrivedAtMapCoords && arrivedAtWorldCoords)
        {
            continueMovement = false;
        }
        else
        {
            continueMovement = true;
        }
        return continueMovement;
    }

    public void SetMapDestination(Vector2Int mapDestination)
    {
        targetMapPos = mapDestination;
        targetWorldPos = gridTransform.gridMap.MapToWorld(targetMapPos);
        arrivedAtWorldCoords = false;
        arrivedAtMapCoords = false; //is this necessary?
        currentTravelDistance = 0;
    }
    
    public void Reset()
    {
        
    }

    public override void ImproveStat(float improveAmount)
    {
        speedStat.ImproveStat(improveAmount);
    }
}

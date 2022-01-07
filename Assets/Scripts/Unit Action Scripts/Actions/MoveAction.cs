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

    private bool adjacent = false;

    private bool arrivedAtWorldCoords = false;
    private bool arrivedAtMapCoords = false;

    List<Vector2Int> currentPath;
    int currentPathIndex = 0;
    private bool cancel;

    private float speedFactor = 1;

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
        arrivedAtMapCoords = true;
        arrivedAtWorldCoords = true;
        currentTravelDistance = 0;
        totalTravelDistance = 0;
        cancel = false;
        adjacent = false;
    }

    public override void StartAction()
    {
        startWorldPos = transform.position;
        totalTravelDistance = Vector3.Distance(startWorldPos, targetWorldPos);
        bool pathFound = PathFinder.GetPath(gridTransform.topLeftPosMap, targetMapPos, out currentPath);
        if(adjacent && pathFound)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
        }
        currentPathIndex = 0;
    }

    public override void Cancel()
    {
        cancel = true;
    }

    public override bool CanDo()
    {
        return currentPath != null;
    }

    public override bool AdvanceAction(float dt, out float progressAmount)
    {
        /* 
         * if we arrived at the next cell, pop the next cell dest
         * if no such cell dest, then return false
         * then perform the dt movement, which sets the bool vars
         */
        //if (cancel) Debug.Log("cancel is set");
        if (currentPath == null)
        {
            progressAmount = 0;
            return false;
        }
            
        if(cancel && currentPath.Count > 1)
        {
            //Debug.Log("removing nodes");
            currentPath.RemoveRange(1, currentPath.Count-1);
        }
        Vector2Int target;
        if(arrivedAtMapCoords && arrivedAtWorldCoords)
        {
            if (currentPath == null)
            {
                progressAmount = 0;
                return false;
            }
            else
            {
                if(currentPathIndex < currentPath.Count)
                {
                    target = currentPath[currentPathIndex];
                    SetMapDestination(target);
                    arrivedAtMapCoords = false;
                    arrivedAtWorldCoords = false;
                    startWorldPos = transform.position;
                    totalTravelDistance = Vector3.Distance(startWorldPos, targetWorldPos);
                    currentPathIndex++;
                }
                else
                {
                    progressAmount = 0;
                    return false;
                }
            }   
        }
        SetSpeedFactor();
        MoveCellToCell(dt);
        progressAmount = (float)currentPathIndex / (float)currentPath.Count;
        return true;
        
    }

    private void SetSpeedFactor()
    {
        if(PollutionManager.Instance.IsEffectAtCell(gridTransform.topLeftPosMap, PollutionManager.Instance.SlowEffect))
        {
            speedFactor = PollutionManager.Instance.SlowEffect.SlowFactor;
        }
        else
        {
            speedFactor = 1;
        }
    }

    private bool MoveCellToCell(float dt)
    {
        bool continueMovement = false;
        if (!arrivedAtWorldCoords)
        {
            currentTravelDistance += dt * (Mathf.Floor(speedStat.Amount)) * speedFactor;
            float normalizedTravelDistance = 0;
            if(totalTravelDistance != 0)
            {
                normalizedTravelDistance = currentTravelDistance / totalTravelDistance; 
            }
            //float normalizedTravelDistance = currentTravelDistance / totalTravelDistance;
            Vector3 newPos = Vector3.Lerp(startWorldPos, targetWorldPos, normalizedTravelDistance);
            gridTransform.MoveToWorldCoords(newPos);
            if (normalizedTravelDistance >= 1 || totalTravelDistance == 0)
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

    public void SetMapDestination(Vector2Int mapDestination, bool inAdjacent = false)
    {
        targetMapPos = mapDestination;
        targetWorldPos = gridTransform.gridMap.MapToWorld(targetMapPos);
        currentTravelDistance = 0;
        adjacent = inAdjacent;
    }
    
    public void Reset()
    {
        
    }

    public override void ImproveStat(float improveAmount)
    {
        speedStat.ImproveStat(improveAmount);
    }
}

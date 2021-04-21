using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//for this one... start off with something simple -- just move in a direct line to the target position.  
//we'll do the more complicated stuff later
public class NavigatorMovingState : State<float, Vector2Int>
{
    private Vector3 targetWorldPos; // in world coords???
    private Vector3 startWorldPos;
    private float totalTravelDistance;
    private float currentTravelDistance;
    private Vector2Int targetMapPos;//grid coords
    private GridTransform gridTransform;
    private Navigator actorUnit;
    private Transform transform;

    private bool arrivedAtWorldCoords = false;
    private bool arrivedAtMapCoords = false;

    public NavigatorMovingState(GameObject inGameObject, StateMachine<float, Vector2Int> inStateMachine) : base(inGameObject, inStateMachine)
    {
        gridTransform = inGameObject.GetComponent<GridTransform>();
        actorUnit = inGameObject.GetComponent<Navigator>();
        if (gridTransform == null || actorUnit == null)
        {
            throw new InvalidOperationException("cannot create an actorunitmovingstate for a gameobject that does not have a grid transform or an actorUnit");
        }
        transform = inGameObject.transform;
        targetWorldPos = transform.position;
        targetMapPos = gridTransform.topLeftPosMap;
    }

    public override State<float> ProcessInput(float dt, out bool stateChange)
    {
        if(!arrivedAtWorldCoords)
        {
            currentTravelDistance += dt * actorUnit.Speed;
            float normalizedTravelDistance = currentTravelDistance / totalTravelDistance;
            Vector3 newPos = Vector3.Lerp(startWorldPos, targetWorldPos, normalizedTravelDistance);
            gridTransform.MoveToWorldCoords(newPos);
            if(normalizedTravelDistance>=1)
            {
                arrivedAtMapCoords = true;
                arrivedAtWorldCoords = true;
            }
            else
            {
                arrivedAtWorldCoords = false;
                arrivedAtMapCoords = false;
            }
            //interpolate btw current position and target position
            //if >= 1, set to done...
        }


        State<float> outState;
        if(arrivedAtMapCoords && arrivedAtWorldCoords)
        {
            stateChange = true;
            outState = ((NavigatorStateMachine)stateMachine).idleState;
        }
        else
        {
            outState = this;
            stateChange = false;
        }
        return outState;
    }

    public override State<float, Vector2Int> ProcessInput(Vector2Int mapDestination, out bool stateChange)
    {
        SetMapDestination(mapDestination);
        stateChange = false;
        return this;
    }

    //need to implement Enter
    public override void Enter(Vector2Int mapDestination)
    {
        SetMapDestination(mapDestination);
    }

    private void SetMapDestination(Vector2Int inMapDestination)
    {
        targetMapPos = inMapDestination;
        targetWorldPos = gridTransform.gridMap.MapToWorld(targetMapPos);
        arrivedAtWorldCoords = false;
        arrivedAtMapCoords = false; //is this necessary?
        startWorldPos = transform.position;
        totalTravelDistance = Vector3.Distance(startWorldPos, targetWorldPos);
        currentTravelDistance = 0;
    }
}

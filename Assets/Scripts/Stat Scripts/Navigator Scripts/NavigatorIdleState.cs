using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavigatorIdleState : State<float, Vector2Int>
{
    private GridTransform gridTransform;
    //private ActorUnitStateMachine stateMachine;
    public NavigatorIdleState(GameObject inGameObject, StateMachine<float, Vector2Int> inStateMachine) : base(inGameObject, inStateMachine)
    {
        gridTransform = inGameObject.GetComponent<GridTransform>();
        if(gridTransform == null)
        {
            throw new InvalidOperationException("cannot create an actorunitidlestate for a gameobject that does not have a grid transform");
        }

    }

    public override State<float> ProcessInput(float input, out bool stateChange)
    {
        stateChange = false;
        return this;
    }

    public override State<float, Vector2Int> ProcessInput(Vector2Int input, out bool stateChange)
    {
        State<float, Vector2Int> outState = this;
        stateChange = false;
        if(input != gridTransform.topLeftPosMap)
        {
            stateChange = true;
            outState = ((NavigatorStateMachine)stateMachine).movingState;
        }
        return outState;
        
    }
}

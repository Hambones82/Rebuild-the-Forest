using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatorStateMachine : StateMachine<float, Vector2Int>
{
    //first of all, these should probably be private.
    //those states should have constructors that provide the states they need.
    //that should be the role of the derived state machine class (like this one)
    public State<float, Vector2Int> idleState;
    public State<float, Vector2Int> movingState;

    //then all the logic will be in the states
    public void SetTarget(Vector2Int targetPos)
    {
        ProcessInput(targetPos);
    }

    public void AdvanceTime(float dt)
    {
        ProcessInput(dt);
    }
    

    public NavigatorStateMachine(GameObject inGameObject) : base(inGameObject)
    {
        idleState = new NavigatorIdleState(inGameObject, this);
        movingState = new NavigatorMovingState(inGameObject, this);
        _currentState = idleState;
        _currentState.Enter();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine <T>
{
    protected GameObject gameObject;

    protected State<T> _currentState;

    public StateMachine(GameObject inGameObject)
    {
        gameObject = inGameObject;
    }

    protected void ProcessInput(T input)
    {
        State<T> newState = _currentState.ProcessInput(input, out bool changeState);
        if(changeState)
        {
            StateTransition(newState, input);
        }
    }

    protected void StateTransition(State<T> newState, T input)
    {
        _currentState.Exit();
        _currentState = newState;
        newState.Enter(input);
    }
}

public abstract class StateMachine<T1, T2> : StateMachine<T1>
{
    public StateMachine(GameObject inGameObject) : base(inGameObject) { }

    protected void StateTransition(State<T1, T2> newState, T2 input)
    {
        _currentState.Exit();
        _currentState = newState;
        newState.Enter(input);
    }

    protected void ProcessInput(T2 input)
    {
        State<T1, T2> newState = ((State<T1, T2>)_currentState).ProcessInput(input, out bool changeState);
        if (changeState)
        {
            StateTransition(newState, input);
        }
    }
}

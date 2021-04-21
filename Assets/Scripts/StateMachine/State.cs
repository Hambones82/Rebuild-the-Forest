using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> 
{
    protected GameObject gameObject;

    protected StateMachine<T> stateMachine;

    public State(GameObject inGameObject, StateMachine<T> inStateMachine)
    {
        gameObject = inGameObject;
        stateMachine = inStateMachine;
    }
    
    public abstract State<T> ProcessInput(T input, out bool stateChange);
    public virtual void Enter() { }
    public virtual void Enter(T input) { }
    public virtual void Exit() { }
}

public abstract class State<T1, T2> : State<T1>
{
    public State(GameObject inGameObject, StateMachine<T1, T2> inStateMachine) : base(inGameObject, inStateMachine) { }
    public abstract State<T1, T2> ProcessInput(T2 input, out bool stateChange);
    public virtual void Enter(T2 input) { }
}
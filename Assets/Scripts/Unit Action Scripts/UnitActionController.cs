using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitActionController : MonoBehaviour
{
    private Queue<UnitAction> unitActionQueue = new Queue<UnitAction>();
    private UnitAction currentAction;
    [SerializeField]
    private string currentActionName = "no action loaded";

    [SerializeField]
    private float actionProgress = 0;
    public float ActionProgress { get => actionProgress; }

    StatLine learning;

    public delegate void AdvanceActionDelegate(float amount);
    public event AdvanceActionDelegate OnAdvanceAction;

    public delegate void ActionStatusDelegate(UnitAction action);
    public event ActionStatusDelegate OnActionStart;
    public event Action OnActionEnd;


    private void Awake()
    {
        currentAction = IdleAction.Instance;
        learning = GetComponent<ActorUnitStats>()?.Learning;
        if (learning == null)
            throw new InvalidOperationException("no learning stat present");
    }

    private void OnEnable()
    {
        EndAllActions();
        currentAction = IdleAction.Instance;
        actionProgress = 0;
    }

    //this class needs to return the action objects to the pool
    private void Update()
    {
        bool continueCurrentAction = true;
        continueCurrentAction = currentAction.AdvanceAction(Time.deltaTime, out actionProgress);
        if (continueCurrentAction) OnAdvanceAction?.Invoke(actionProgress);
        currentAction.ImproveStat(Time.deltaTime * learning.Amount);
        
        if(continueCurrentAction == false)
        {
            //end current action
            if(unitActionQueue.Count > 0)
            {
                SwitchToAction(unitActionQueue.Dequeue());
                if(!currentAction.CanDo())
                {
                    CancelAllActions();
                }
            }
            else
            {
                if(currentAction!= IdleAction.Instance)
                {
                    SwitchToAction(IdleAction.Instance);
                }
            }
        }
    }

    public void EndAllActions()
    {
        currentAction.EndAction();
        currentAction = null;
        foreach(UnitAction action in unitActionQueue)
        {
            action.EndAction();
        }
        unitActionQueue.Clear();
        actionProgress = 0;
    }

    public void CancelAllActions()
    {
        currentAction.Cancel();
        foreach(UnitAction action in unitActionQueue)
        {
            action.Cancel();
        }
        actionProgress = 0;
    }

    private void SwitchToAction(UnitAction action)
    {
        actionProgress = 0;
        currentAction.EndAction();
        currentAction = action;
        currentAction.StartAction();
        currentActionName = currentAction.ActionName;
        if (action == IdleAction.Instance) OnActionEnd?.Invoke();
        else OnActionStart?.Invoke(currentAction);
    }

    public void DoAction(UnitAction action) //prevent the same action from being queued multiple times on same target, IF you shouldn't repeat
    {
        unitActionQueue.Enqueue(action);
    }

}

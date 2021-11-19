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

    StatLine learning;

    private void Awake()
    {
        currentAction = IdleAction.Instance;
        learning = GetComponent<ActorUnitStats>()?.Learning;
        if (learning == null)
            throw new InvalidOperationException("no learning stat present");
    }

    //this class needs to return the action objects to the pool
    private void Update()
    {
        bool continueCurrentAction = true;
        continueCurrentAction = currentAction.AdvanceAction(Time.deltaTime);
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
                //if(currentaction . cannot do, cancel all actions.
            }
            else
            {
                SwitchToAction(IdleAction.Instance);
            }
            
        }
    }

    public void CancelAllActions()
    {
        currentAction.Cancel();
        foreach(UnitAction action in unitActionQueue)
        {
            action.Cancel();
        }
    }

    private void SwitchToAction(UnitAction action)
    {
        currentAction.EndAction();
        currentAction = action;
        currentAction.StartAction();
        currentActionName = currentAction.ActionName;
    }

    public void DoAction(UnitAction action) //prevent the same action from being queued multiple times on same target, IF you shouldn't repeat
    {
        unitActionQueue.Enqueue(action);
    }

}

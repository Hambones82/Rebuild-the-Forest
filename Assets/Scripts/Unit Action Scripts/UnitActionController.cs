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
        currentAction.ImproveStat(Time.deltaTime * learning.Amount);//GOING TO NEED A TOUGH FIX.  or maybe we'll just have the action do the improve again?
        if(continueCurrentAction == false)
        {
            //end current action
            if(unitActionQueue.Count > 0)
            {
                SwitchToAction(unitActionQueue.Dequeue());
                
            }
            else
            {
                SwitchToAction(IdleAction.Instance);

            }
            
        }
    }

    private void SwitchToAction(UnitAction action)
    {
        //ok, so how do i return the action to the correct pool if i don't have its type?  i guess make sure to get its type...
        //but we should just do this in the objectpool class
        currentAction.EndAction();
        currentAction = action;
        currentAction.StartAction();
        currentActionName = currentAction.ActionName;
    }

    public void DoAction(UnitAction action)
    {
        unitActionQueue.Enqueue(action);
    }

}

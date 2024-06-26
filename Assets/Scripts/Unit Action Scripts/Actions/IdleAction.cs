using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : UnitAction
{
    private static IdleAction instance;

    public static IdleAction Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new IdleAction();
            }
            return instance;
        }
    }

    public override bool CanDo()
    {
        return true;
    }

    public override void Cancel() { }

    public IdleAction()
    {
        actionName = "Idle";
    }

    public override void EndAction() { }
    
    public override bool AdvanceAction(float dt, out float progressAmount)
    {
        progressAmount = 0;
        return false; //always "yield" if possible
    }

    public override void StartAction()
    {
        
    }

    public override void ImproveStat(float ImproveAmount)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CleanPollutionAction : UnitActionWithTarget<Pollution>, IObjectPoolInterface
{
    //it'd be nice to include this info in a scriptable object.
    private float cleanTimer = 0;
    private float cleanPeriod = 1;
    private StatLine cleaningStat;
    private bool targetHasDied;
    Pollution targetPollution;
    private bool cancel = false;

    public override void Cancel()
    {
        cancel = true;
    }

    public override void Initialize(GameObject inGameObject, Pollution inPollution)
    {
        base.Initialize(inGameObject, inPollution);
        cleaningStat = inGameObject.GetComponent<ActorUnitStats>().CleaningSpeed;
        //register for clearing...
        targetHasDied = false;
        inPollution.OnDisableEvent.AddListener(KillTarget);
        targetPollution = inPollution;
        cancel = false;
    }

    public void KillTarget()
    {
        targetHasDied = true;
    }

    public CleanPollutionAction()
    {
        actionName = "Cleaning Pollution";
    }

    public override void EndAction()
    {
        targetPollution.OnDisableEvent.RemoveListener(KillTarget);
        ObjectPool.Return(this);
    }

    public override bool AdvanceAction(float dt, out float progressAmount)
    {
        if (cancel)
        {
            progressAmount = 0;
            return false;
        }
            
        cleanTimer += dt;
        bool retVal = true;
        //if pollution is dead, return false
        float modifiedPeriod = cleanPeriod / Mathf.Floor(cleaningStat.Amount);
        while(cleanTimer >= modifiedPeriod) 
        {
            if (!CanDo())
            {
                //Debug.Log("Cannot clean pollution");
                progressAmount = 0;
                return false;
            }
                
            cleanTimer -= modifiedPeriod;
            retVal = DoCleanPollutionTick();
            if (!retVal) break; //if the tick fully cleans up the pollution, don't continue the while loop
        }
        progressAmount = 1 - targetPollution.Amount / targetPollution.MaxAmount;
        return retVal;
    }

    public void SetTargetPollution(Pollution targetPol)
    {
        target = targetPol;
    }

    private bool DoCleanPollutionTick()
    {
        float resultingAmount = target.Amount - ((CleaningSpeedStat)(cleaningStat.StatType)).AmountPerClean;
        target.SetAmount(resultingAmount);
        return (resultingAmount > 0);
    }

    public override bool CanDo()
    {
        return !((targetPollution == null) || (targetHasDied));
    }

    public override void StartAction()
    {
        
    }

    public void Reset()
    {

    }

    public override void ImproveStat(float improveAmount)
    {
        cleaningStat.ImproveStat(improveAmount);
    }
}

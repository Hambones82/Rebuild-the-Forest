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

    public override void Initialize(GameObject inGameObject, Pollution inPollution)
    {
        base.Initialize(inGameObject, inPollution);
        cleaningStat = inGameObject.GetComponent<ActorUnitStats>().CleaningSpeed;
        //register for clearing...
        targetHasDied = false;
        inPollution.OnDisableEvent.AddListener(KillTarget);
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
        ObjectPool.Return(this);
    }

    public override bool AdvanceAction(float dt)
    {
        cleanTimer += dt;
        bool retVal = true;
        //if pollution is dead, return false
        float modifiedPeriod = cleanPeriod / Mathf.Floor(cleaningStat.Amount);
        while(cleanTimer >= modifiedPeriod) 
        {
            if (targetHasDied) return false;
            cleanTimer -= modifiedPeriod;
            retVal = DoCleanPollutionTick();
            if (!retVal) break; //if the tick fully cleans up the pollution, don't continue the while loop
        }
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

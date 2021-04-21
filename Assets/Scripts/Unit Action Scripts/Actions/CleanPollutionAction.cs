using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CleanPollutionAction : UnitAction, IObjectPoolInterface
{
    private float cleanTimer = 0;
    private float cleanPeriod = 1;
    private ActorUnitPollutionStats stats;
    private Pollution targetPollution;

    public override void Initialize(GameObject inGameObject)
    {
        base.Initialize(inGameObject);
        stats = inGameObject.GetComponent<ActorUnitPollutionStats>();
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
        float modifiedPeriod = cleanPeriod / Mathf.Floor(stats.CleaningSpeed);
        while(cleanTimer >= modifiedPeriod) 
        {
            cleanTimer -= modifiedPeriod;
            retVal = DoCleanPollutionTick();
            if (!retVal) break; //if the tick fully cleans up the pollution, don't continue the while loop
        }
        return retVal;
    }

    public void SetTargetPollution(Pollution targetPol)
    {
        targetPollution = targetPol;
    }

    private bool DoCleanPollutionTick()
    {
        float resultingAmount = targetPollution.Amount - stats.PerCleanAmount;
        targetPollution.SetAmount(resultingAmount);
        return (resultingAmount > 0);
    }

    public override void StartAction()
    {
        
    }

    public void Reset()
    {

    }

    public override void ImproveStat(float factor)
    {
        stats.ImproveSpeed(factor);
    }
}

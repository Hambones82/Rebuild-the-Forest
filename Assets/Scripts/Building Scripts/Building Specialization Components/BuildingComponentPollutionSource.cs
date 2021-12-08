using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentPollutionSource : BuildingComponentOperator
{
    public override bool IsOperatable(GameObject inGameobject)
    {
        return true;
    }

    protected override void Trigger(GameObject gameObject = null)
    {
        BuildingManager.Instance.DestroyBuilding(GetComponent<Building>());
    }
}

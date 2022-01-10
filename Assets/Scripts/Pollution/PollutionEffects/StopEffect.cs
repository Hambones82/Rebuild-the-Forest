using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StopPollutionEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Stop Effect")]
public class StopEffect : PollutionEffect
{
    public override void OnDeath(Pollution pollution)
    {
        PathingController.Instance.UpdatePassable(pollution.GetComponent<GridTransform>().topLeftPosMap, true);
    }

    public override void OnSpawn(Pollution pollution)
    {
        //Debug.Log("spawning pollution");
        PathingController.Instance.UpdatePassable(pollution.GetComponent<GridTransform>().topLeftPosMap, false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowPollutionEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Slow Pollution Effect")]
public class SlowEffect : PollutionEffect
{
    public override void OnDeath(Pollution pollution)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSpawn(Pollution pollution)
    {
        throw new System.NotImplementedException();
    }
}

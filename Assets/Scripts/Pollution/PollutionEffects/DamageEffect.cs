using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamagePollutionEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Damage Effect")]
public class DamageEffect : PollutionEffect
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

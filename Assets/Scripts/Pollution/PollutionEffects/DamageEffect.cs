using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamagePollutionEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Damage Effect")]
public class DamageEffect : PollutionEffect
{
    [SerializeField]
    private float _damageAmount;
    public float DamageAmount { get => _damageAmount; }
    public override void OnDeath(Pollution pollution) { }

    public override void OnSpawn(Pollution pollution) { }
}

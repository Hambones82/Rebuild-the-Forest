using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowPollutionEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Slow Pollution Effect")]
public class SlowEffect : PollutionEffect
{
    [SerializeField]
    private float _slowFactor;
    public float SlowFactor { get => _slowFactor; }
    public override void OnDeath(Pollution pollution) { }

    public override void OnSpawn(Pollution pollution) { }
}

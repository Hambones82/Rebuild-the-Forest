using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ActorUnitStats))]
public class ActorUnit : MonoBehaviour
{
    private ActorUnitStats stats;
    public ActorUnitStats Stats { get => stats; }

    public UnityEvent OnDeath;

    //probably want an ondeath thing.
    public void Kill()
    {
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
    }

    private void Awake()
    {
        stats = GetComponent<ActorUnitStats>();
    }

    public bool CanTransformInto(Building buildingToBecome)
    {
        return stats.Stats.HasAtLeast(buildingToBecome.StatRequirements);
    }
}

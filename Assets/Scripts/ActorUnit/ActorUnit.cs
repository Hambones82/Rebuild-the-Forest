using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorUnitStats))]
public class ActorUnit : MonoBehaviour
{
    private ActorUnitStats stats;
    public ActorUnitStats Stats { get => stats; }

    private void Awake()
    {
        stats = GetComponent<ActorUnitStats>();
    }

    public bool CanTransformInto(Building buildingToBecome)
    {
        return stats.Stats.HasAtLeast(buildingToBecome.StatRequirements);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorUnitStats : MonoBehaviour
{
    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitStats.movementSpeed' is never assigned to, and will always have its default value null
    private StatLine movementSpeed;
#pragma warning restore CS0649 // Field 'ActorUnitStats.movementSpeed' is never assigned to, and will always have its default value null
    public StatLine MovementSpeed { get => movementSpeed; }
    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitStats.cleaningSpeed' is never assigned to, and will always have its default value null
    private StatLine cleaningSpeed;
#pragma warning restore CS0649 // Field 'ActorUnitStats.cleaningSpeed' is never assigned to, and will always have its default value null
    public StatLine CleaningSpeed { get => cleaningSpeed; }
    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitStats.learning' is never assigned to, and will always have its default value null
    private StatLine learning;
#pragma warning restore CS0649 // Field 'ActorUnitStats.learning' is never assigned to, and will always have its default value null
    public StatLine Learning { get => learning; }

    private StatLineSet stats;
    public StatLineSet Stats
    {
        get => stats;
    }

    void Awake()
    {
        stats = new StatLineSet();
        stats.SetElements.Add(movementSpeed);
        stats.SetElements.Add(cleaningSpeed);
        stats.SetElements.Add(learning);
    }

    public StatLine GetStat(StatType statType)
    {
        return stats.Get(statType);
    }
}

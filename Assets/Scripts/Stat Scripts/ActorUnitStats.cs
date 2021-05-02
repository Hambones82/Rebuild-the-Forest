using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorUnitStats : MonoBehaviour
{
    [SerializeField]
    private StatLine movementSpeed;
    public StatLine MovementSpeed { get => movementSpeed; }
    [SerializeField]
    private StatLine cleaningSpeed;
    public StatLine CleaningSpeed { get => cleaningSpeed; }
    [SerializeField]
    private StatLine learning;
    public StatLine Learning { get => learning; }

    private StatLineSet stats;

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

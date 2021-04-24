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

    private List<StatLine> stats;
    
    void Awake()
    {
        stats = new List<StatLine>()
        {
            movementSpeed,
            cleaningSpeed,
            learning
        };
    }

    public StatLine GetStat(StatType statType)
    {
        for(int i = 0; i < stats.Count; i++)
        {
            if(stats[i].StatType == statType)
            {
                return stats[i];
            }
        }
        return null;
    }
}

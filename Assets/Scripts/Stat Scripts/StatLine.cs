using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatLine 
{
    [SerializeField]
    private StatType statType;
    public StatType StatType { get => statType; }
    [SerializeField]
    private float amount;
    public float Amount { get => amount; }
    public void ImproveStat(float improveAmount)
    {
        amount += improveAmount;
    }
}

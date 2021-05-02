using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatLine : TypedNumericalComparable<StatType>
{
    public StatLine(StatType statType, float f) : base(statType, f) { }

    public StatType StatType { get => type; }
    
    public float Amount { get => data; private set => data = value; }

    public void ImproveStat(float improveAmount)
    {
        Amount += improveAmount;
    }
}

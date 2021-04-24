using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cleaning Speed Stat Type", menuName = "ScriptableObjects/Types/Stats/Cleaning Speed")]
public class CleaningSpeedStat : StatType
{
    [SerializeField]
    private float amountPerClean;
    public float AmountPerClean { get => amountPerClean; }
}
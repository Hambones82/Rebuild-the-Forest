using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cleaning Speed Stat Type", menuName = "ScriptableObjects/Types/Stats/Cleaning Speed")]
public class CleaningSpeedStat : StatType
{
    [SerializeField]
#pragma warning disable CS0649 // Field 'CleaningSpeedStat.amountPerClean' is never assigned to, and will always have its default value 0
    private float amountPerClean;
#pragma warning restore CS0649 // Field 'CleaningSpeedStat.amountPerClean' is never assigned to, and will always have its default value 0
    public float AmountPerClean { get => amountPerClean; }
}
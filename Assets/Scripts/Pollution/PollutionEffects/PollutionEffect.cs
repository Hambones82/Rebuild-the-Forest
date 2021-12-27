using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "PollutionEffect", menuName = "ScriptableObjects/Types/Pollution Effect")]
public abstract class PollutionEffect : ScriptableObject
{
    public abstract void OnSpawn(Pollution pollution);
    public abstract void OnDeath(Pollution pollution);
}

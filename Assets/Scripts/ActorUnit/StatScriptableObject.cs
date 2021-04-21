using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor Unit Stat", menuName = "Actor Unit Stat")]
public class StatScriptableObject : ScriptableObject
{
    [SerializeField]
    private string name;
    [SerializeField]
    private float baseLearnSpeed;
}

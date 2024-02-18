using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapEffectType", menuName = "ScriptableObjects/Types/Map Effect")]
public class MapEffectType : SOType
{
    [SerializeField]
    private string _effectName;
    public string EffectName { get => _effectName; }

    [SerializeField]
    private int defaultRange;
    public int DefaultRange { get => defaultRange; }
}

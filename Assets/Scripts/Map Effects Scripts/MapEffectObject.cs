using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEffectObject 
{
    [SerializeField]
    protected MapEffectType _effectType;
    public MapEffectType EffectType { get => _effectType; }

    [SerializeField]
    protected float _amount;
    public float Amount { get => _amount; }

    //enabled...
    //range

    public override string ToString()
    {
        return $"Effect: {_effectType.EffectName} - Amount: {_amount}";
    }
}

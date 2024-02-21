using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEffectObject 
{
    [SerializeField]
    protected MapEffectType _effectType;
    public MapEffectType EffectType { get => _effectType; set => _effectType = value; }

    [SerializeField]
    protected float _amount;
    public float Amount { get => _amount; set => _amount = value; }

    [SerializeField]
    protected MapEffectComponent source;
    public MapEffectComponent Source { get =>  source; set => source = value; }
    //enabled...
    //range

    public override string ToString()
    {
        return $"Effect: {_effectType.EffectName} - Amount: {_amount} - Source: {Source.gameObject.name}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEffect 
{
    //effect type... don't want to use enums.  do i use map effect type scriptable objects?  if so, how do i get that into a thing?
    //ok i think this could actually be possible...  each building can have an effect type and a radius.
    [SerializeField]
    protected MapEffectType _effectType;
    public MapEffectType EffectType { get => _effectType; }

    [SerializeField]
    protected float _amount;
    public float Amount { get => _amount; }

    public override string ToString()
    {
        return $"Effect: {_effectType.EffectName} - Amount: {_amount}";
    }
}

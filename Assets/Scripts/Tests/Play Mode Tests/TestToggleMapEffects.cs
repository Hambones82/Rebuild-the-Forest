using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToggleMapEffects : MonoBehaviour
{
    [SerializeField]
    private MapEffectComponent mapEffectComponent;
    [SerializeField]
    private MapEffectType effectType;

    public void ToggleEffect() 
    {
        Debug.Log("calling toggle effect");
        if(mapEffectComponent.IsEffectEnablbed(effectType))
        {
            Debug.Log("calling disable effect");
            mapEffectComponent.DisableEffect(effectType);
        }
        else
        {
            Debug.Log("calling enable effect");
            mapEffectComponent.EnableEffect(effectType);
        }
    }
}

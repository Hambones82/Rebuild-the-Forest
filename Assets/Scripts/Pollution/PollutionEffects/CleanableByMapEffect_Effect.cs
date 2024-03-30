using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "PollutionCleanableByMapEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Cleanable by MapEffect")]
public class CleanableByMapEffect_Effect : PollutionEffect, IPollutionCleanable
{
    [SerializeField]
    private List<MapEffectType> cleanEnableEffects;
    public List<MapEffectType> CleanEnableEffects { get => new List<MapEffectType>(cleanEnableEffects); }

    
    public bool IsCleanable(Vector2Int targetCell)
    {        
        List<MapEffectType> effectsAtCell =
            MapEffectsManager.Instance.GetEffectsAtCell(targetCell)
            ?.Select(effect => effect.EffectType)?.ToList<MapEffectType>();
        
        if (cleanEnableEffects == null) return true;
        else if (cleanEnableEffects.Count == 0) return true;
        else if (effectsAtCell == null) return false;

        bool isCleanable = true;

        foreach (MapEffectType effectType in cleanEnableEffects)
        {
            if (!effectsAtCell.Contains(effectType))
            {
                isCleanable = false;
                break;
            }
        }
        return isCleanable;
    }

    public void OnClean(Vector2Int targetCell)
    {        
        List<MapEffectObject> effectsToTag =
            MapEffectsManager.Instance.GetEffectsAtCell(targetCell)
            .Where(effect => cleanEnableEffects.Contains(effect.EffectType))
            .ToList<MapEffectObject>();
        foreach (MapEffectObject effect in effectsToTag)
        {
            effect.TagEffect(targetCell);
        }
    }
}

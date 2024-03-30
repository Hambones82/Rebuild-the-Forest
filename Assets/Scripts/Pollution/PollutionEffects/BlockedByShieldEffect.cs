using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PollutionBlockedByShieldEffect", menuName = "ScriptableObjects/Types/PollutionEffects/Blocked By Shield Effect")]

public class BlockedByShieldEffect : PollutionEffect, IPollutionSpawnBlock
{
    [SerializeField]
    private List<MapEffectType> mapBlockingEffects;
    //could even add a bool/toggle for "need all or need some"

    public bool BlocksPollutionGrowth(Vector2Int cell)
    {
        if (mapBlockingEffects.Count == 0) return false;
        List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);

        if (effectsAtCell != null)
        {
            List<MapEffectType> effectTypesAtCell = effectsAtCell.Select(type => type.EffectType).ToList();
            foreach (MapEffectType effectType in mapBlockingEffects)
            {
                if (!effectTypesAtCell.Contains(effectType))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void OnSpawnBlocked(Vector2Int cell)
    {
        List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);
        foreach (MapEffectObject mapEffect in effectsAtCell)
        {
            mapEffect.TagEffect(cell);
        }
    }


    public override void OnDeath(Pollution pollution) { }
    public override void OnSpawn(Pollution pollution) { }
}

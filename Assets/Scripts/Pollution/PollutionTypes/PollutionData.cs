using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PollutionData", menuName = "Pollution/Pollution Data")]
public class PollutionData : ScriptableObject
{
    [SerializeField]
    private List<MapEffectType> mapBlockingEffects;
    //could even add a bool/toggle for "need all or need some"

    //droptable
    [SerializeField]
    private DropTable _dropTable;
    
    //priority
    [SerializeField]
    private int _priority;
    public int Priority { get => _priority; }
    //a/t else?
    
    [SerializeField]
    public List<PollutionEffect> pollutionEffects;
    
    public bool IsBlocked(Vector2Int cell)
    {
        if(mapBlockingEffects.Count == 0) return false;
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
}


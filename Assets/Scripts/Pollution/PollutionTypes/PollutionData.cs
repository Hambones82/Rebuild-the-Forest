using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PollutionData", menuName = "Pollution/Pollution Data")]
public class PollutionData : ScriptableObject
{
    [SerializeField]
    private MapEffectType treeBlockEffect;
    [SerializeField]
    private MapEffectType plantBlockEffect;
    [SerializeField]
    private MapEffectType mushroomBlockEffect;

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
        bool retVal = false;
        List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);
        if (effectsAtCell != null)
        {
            bool mush = false;
            bool plant = false;
            bool tree = false;
            foreach (MapEffectObject effectObject in effectsAtCell)
            {
                if (effectObject.EffectType == treeBlockEffect)
                {
                    tree = true;
                }
                else if (effectObject.EffectType == mushroomBlockEffect)
                {
                    mush = true;
                }
                else if (effectObject.EffectType == plantBlockEffect)
                {
                    plant = true;
                }
            }
            retVal = mush && plant && tree;
        }
        return retVal;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L1BasicPollutionController : BasicPollutionController
{
    [SerializeField]
    private MapEffectType treeBlockEffect;
    [SerializeField]
    private MapEffectType plantBlockEffect;
    [SerializeField]
    private MapEffectType mushroomBlockEffect;

    protected override bool IsBlocked(Vector2Int cell)
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

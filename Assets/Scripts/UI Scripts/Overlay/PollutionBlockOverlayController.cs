using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PollutionBlockOverlayController : MonoBehaviour
{
    private TileDataMap tdm;
    [SerializeField]
    private UIOverlayTile _emptyTile;
    [SerializeField]
    private UIOverlayTile _mushTile;
    [SerializeField]
    private UIOverlayTile _plantTile;
    [SerializeField]
    private UIOverlayTile _treeTile;
    [SerializeField]
    private UIOverlayTile _mushPlantTile;
    [SerializeField]
    private UIOverlayTile _mushTreeTile;
    [SerializeField]
    private UIOverlayTile _PlantTreeTile;
    [SerializeField]
    private UIOverlayTile _mushPlantTreeTile;

    [SerializeField]
    private MapEffectType mushBlockEffectType;
    [SerializeField]
    private MapEffectType plantBlockEffectType;
    [SerializeField]
    private MapEffectType treeBlockEffectType;

    private void Awake()
    {
        MapEffectsManager.Instance.OnMapEffectChange += ProcessMapEffectChange;
        tdm = GetComponent<TileDataMap>();
        foreach(Vector2Int cell in tdm.gridMap.GetMapRect().allPositionsWithin)
        {
            ProcessMapEffectChange(cell);
        }
    }

    //callback for effect add
    private void ProcessMapEffectChange(Vector2Int cell)
    {
        List<MapEffectObject> effects = MapEffectsManager.Instance.GetEffectsAtCell(cell);
        bool mush = false;
        bool tree = false;
        bool plant = false;
        if (effects != null)
        {
            
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].EffectType == mushBlockEffectType)
                {
                    mush = true;
                }
                else if (effects[i].EffectType == plantBlockEffectType)
                {
                    plant = true;
                }
                else if (effects[i].EffectType == treeBlockEffectType)
                {
                    tree = true;
                }
            }
        }
        tdm.SetTileAt(GetTileFor(mush, plant, tree), cell);
    }
    
    private UIOverlayTile GetTileFor(bool mush, bool plant, bool tree)
    {
        if (!mush && !plant && !tree)
            return _emptyTile;
        else if (!mush && !plant && tree)
            return _treeTile;
        else if (!mush && plant && !tree)
            return _plantTile;
        else if (mush && !plant && !tree)
            return _mushTile;
        else if (mush && plant && !tree)
            return _mushPlantTile;
        else if (mush && !plant && tree)
            return _mushTreeTile;
        else if (!mush && plant && tree)
            return _PlantTreeTile;
        else
            return _mushPlantTreeTile;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-7)]
public class MapEffectsManager : MonoBehaviour
{
    //have a ref to building manager, add some functions to on buildbuilding and on remove building

    [SerializeField]
    private GridMap gridMap;

    private List<MapEffect>[,] mapEffects;

    private RectInt extents;

    private static MapEffectsManager _instance;
    public static MapEffectsManager Instance { get => _instance; }

    private void Awake()
    {
        mapEffects = new List<MapEffect>[gridMap.width, gridMap.height];
        if (BuildingManager.Instance == null) Debug.Log("wtf?");
        BuildingManager.Instance.OnBuildingSpawn += AddBuildingEffect;
        extents = gridMap.GetGridRect();
        if (_instance == null) _instance = this;
        else throw new InvalidOperationException("can't instantiate this twice");
    }
    
    

    private void AddBuildingEffect(Building building)
    {
        Debug.Log("calling add building effect");
        foreach(BuildingEffect effect in building.BuildingEffects)
        {
            RectInt effectExtents = building.GetComponent<GridTransform>().GetRect();
            effectExtents.x -= effect.Range;
            effectExtents.y -= effect.Range;//minus equals?
            effectExtents.width += effect.Range * 2;
            effectExtents.height += effect.Range * 2;
            foreach(Vector2Int coord in effectExtents.allPositionsWithin)
            {
                AddEffect(effect.Effect, coord);
            }
        }
    }

    public void AddEffect(MapEffect mapEffect, Vector2Int mapCoords)
    {
        //Debug.Log($"adding effect to coords {mapCoords.ToString()}");
        Debug.Log($"extents: {extents.ToString()}");
        if (!extents.Contains(mapCoords))
        {
            Debug.Log("map effect not contained within map.");
            return;
        }
        //Debug.Log("map coords to add effect: " + mapCoords.ToString());
        List<MapEffect> effectsAtCell = mapEffects[mapCoords.x, mapCoords.y];
        
        if(effectsAtCell == null)
        {
            
            effectsAtCell = new List<MapEffect>();
        }
        Debug.Log("adding effect");
        effectsAtCell.Add(mapEffect);
    }
    
    public bool RemoveEffect(MapEffect mapEffect, Vector2Int mapCoords)
    {
        List<MapEffect> effectsAtCell = mapEffects[mapCoords.x, mapCoords.y];
        if (effectsAtCell == null) return false;
        foreach(MapEffect effect in effectsAtCell)
        {
            if(effect == mapEffect)
            {
                effectsAtCell.Remove(effect);
                return true;
            }
        }
        return false;
    }

    public List<MapEffect> GetEffectsAtCell(Vector2Int mapCoords)
    {
        return mapEffects[mapCoords.x, mapCoords.y];
    }

    //register building effects?
}

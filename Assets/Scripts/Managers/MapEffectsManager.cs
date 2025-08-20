using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-7)]
public class MapEffectsManager : MonoBehaviour
{

    public delegate void MapEffectChangeDelegate(Vector2Int cell);
    public event MapEffectChangeDelegate OnMapEffectChange;
    
    [SerializeField]
    private GridMap gridMap;

    private List<MapEffectObject>[,] mapEffects;

    private RectInt extents;

    private static MapEffectsManager _instance;
    public static MapEffectsManager Instance { get => _instance; }

    private void Awake()
    {
        mapEffects = new List<MapEffectObject>[gridMap.width, gridMap.height];
        extents = new RectInt(0, 0, gridMap.width, gridMap.height);
        if (_instance == null) _instance = this;
        else throw new InvalidOperationException("can't instantiate MapEffectsManager twice");
    }
    
    public void AddEffect(MapEffectObject mapEffect, Vector2Int mapCoords)
    {
        if (!extents.Contains(mapCoords))
        {
            return;
        }
        
        List<MapEffectObject> effectsAtCell = mapEffects[mapCoords.x, mapCoords.y];
        
        if(effectsAtCell == null)
        {
            effectsAtCell = new List<MapEffectObject>();
            mapEffects[mapCoords.x, mapCoords.y] = effectsAtCell;
        }
        //Debug.Log("adding effect");
        if(!effectsAtCell.Contains(mapEffect))
        {
            effectsAtCell.Add(mapEffect);
        }
        OnMapEffectChange?.Invoke(mapCoords);
    }
    
    public bool RemoveEffect(MapEffectObject mapEffect, Vector2Int mapCoords)
    {
        if(mapCoords.x < 0 || mapCoords.y < 0 || mapCoords.x >= gridMap.width || mapCoords.y >= gridMap.height)
        {
            return false;
        }
        List<MapEffectObject> effectsAtCell = mapEffects[mapCoords.x, mapCoords.y];
        if (effectsAtCell == null) return false;
        foreach(MapEffectObject effect in effectsAtCell)
        {
            if(effect == mapEffect)
            {
                effectsAtCell.Remove(effect);
                OnMapEffectChange?.Invoke(mapCoords);
                return true;
            }
        }
        return false;
    }

    public List<MapEffectObject> GetEffectsAtCell(Vector2Int mapCoords)
    {
        //Debug.Log("map effect manager getting effect");
        if (mapEffects[mapCoords.x, mapCoords.y] == null) return null;
        return new List<MapEffectObject> (mapEffects[mapCoords.x, mapCoords.y]);
    }

    public bool EffectExistsAtCell(MapEffectType effectType, Vector2Int cell)
    {
        return mapEffects[cell.x, cell.y]?.Find(effect=> effect.EffectType == effectType) != null;
    }
    //register building effects?
}

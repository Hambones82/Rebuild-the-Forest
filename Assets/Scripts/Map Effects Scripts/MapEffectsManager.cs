using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//fix this entire thing...  the components themselves should register in map effects manager.
//is this even necessary?  can we just check...  not sure...  probably best to have this "register"
[DefaultExecutionOrder(-7)]
public class MapEffectsManager : MonoBehaviour
{
    //have a ref to building manager, add some functions to on buildbuilding and on remove building

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
        //Debug.Log($"adding effect to coords {mapCoords.ToString()}");
        //Debug.Log($"extents: {extents.ToString()}");
        if (!extents.Contains(mapCoords))
        {
            //Debug.Log("map effect not contained within map.");
            return;
        }
        //Debug.Log("map coords to add effect: " + mapCoords.ToString());
        List<MapEffectObject> effectsAtCell = mapEffects[mapCoords.x, mapCoords.y];
        
        if(effectsAtCell == null)
        {
            //Debug.Log("adding effect");
            effectsAtCell = new List<MapEffectObject>();
            mapEffects[mapCoords.x, mapCoords.y] = effectsAtCell;
        }
        //Debug.Log("adding effect");
        if(!effectsAtCell.Contains(mapEffect))
        {
            effectsAtCell.Add(mapEffect);
        }
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
                return true;
            }
        }
        return false;
    }

    public List<MapEffectObject> GetEffectsAtCell(Vector2Int mapCoords)
    {
        //Debug.Log("map effect manager getting effect");
        return mapEffects[mapCoords.x, mapCoords.y];
    }

    //register building effects?
}

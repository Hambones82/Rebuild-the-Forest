using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class MapEffectComponent : MonoBehaviour
{
    [SerializeField]
    private List<MapEffect> _mapEffects;
    
    public List<MapEffect> MapEffects { get { return new List<MapEffect>(_mapEffects); } }

    public bool IsEffectEnablbed(MapEffectType effectType)
    {
        MapEffect foundEffect = _mapEffects.Find(effect => effect.Effect.EffectType == effectType);
        if (foundEffect == null) return false;
        return foundEffect.Enabled;
    }

    public delegate void NotifyEffectDelegate(MapEffectType effectType, Vector2Int cell);
    public event NotifyEffectDelegate TagEffectEvent;

    public MapEffect GetEffect(MapEffectType type)
    {
        return _mapEffects.Find(effect => effect.Effect.EffectType == type);
    }

    //probably need to check if this is enabled... if enabled, and foundeffect.enabled, then enable..
    public void EnableEffect(MapEffectType effectType)
    {        
        MapEffect foundEffect = _mapEffects.Find(effect => effect.Effect.EffectType == effectType);
        if (foundEffect == null)
        {
            MapEffect newEffect = new MapEffect();
            newEffect.Effect = new MapEffectObject();
            newEffect.Range = effectType.DefaultRange;
            newEffect.Enabled = false;
            newEffect.Effect.EffectType = effectType;
            newEffect.Effect.Amount = 1;
            _mapEffects.Add(newEffect);
            foundEffect = newEffect;
        }            
        if (foundEffect.Enabled) return;
        foundEffect.Enabled = true;
        AddEffectRegistration(foundEffect);
    }

    public void DisableEffect(MapEffectType effectType)
    {
        Assert.IsNotNull(_mapEffects, "map effects list is null");
        /*
        Debug.Log($"map effects in list: {_mapEffects.Count}");        
        foreach(MapEffect effect in _mapEffects)
        {
            if (effect.Effect == null) Debug.Log("effect.Effect is null");
            Debug.Log($"{effect.Effect.EffectType.ToString()}");
        }
        */
        MapEffect foundEffect = _mapEffects.Find(effect => effect.Effect.EffectType == effectType);
        if (foundEffect == null) return;
        if (!foundEffect.Enabled) return;
        foundEffect.Enabled = false;
        RemoveEffectRegistration(foundEffect);
    }
    //also need a corresponding disableeffect.  then test both.

    private void OnEnable()
    {
        GetComponent<GridTransform>().OnChangeMapPos += ChangeMapPos;
        foreach (MapEffect effect in _mapEffects)
        {
            if(effect.Enabled)
            {
                AddEffectRegistration(effect);  
            }
        }
    }

    private void OnDisable()
    {
        GetComponent<GridTransform>().OnChangeMapPos -= ChangeMapPos;
        foreach (MapEffect effect in _mapEffects)
        {
            if(effect.Enabled)
            {
                RemoveEffectRegistration(effect);
            }
        }
    }

    private void AddEffectRegistration(MapEffect effect)
    {
        RectInt effectExtents = GetComponent<GridTransform>().GetRect();
        ModifyEffectRegistration(effect, effectExtents, true);
    }

    private void RemoveEffectRegistration(MapEffect effect)
    {
        RectInt effectExtents = GetComponent<GridTransform>().GetRect();
        ModifyEffectRegistration(effect, effectExtents, false);
    }

    private void ModifyEffectRegistration(MapEffect effect, RectInt transformExtents, bool add)//if not add, remove
    {
        RectInt localScopeTransformExtents = transformExtents;
        localScopeTransformExtents.x -= effect.Range;
        localScopeTransformExtents.y -= effect.Range;
        localScopeTransformExtents.width += effect.Range * 2;
        localScopeTransformExtents.height += effect.Range * 2;
        if(add)
        {
            effect.Effect.Source = this;
            foreach (Vector2Int coord in localScopeTransformExtents.allPositionsWithin)
            {                
                MapEffectsManager.Instance.AddEffect(effect.Effect, coord);
            }
        }
        else
        {
            foreach (Vector2Int coord in localScopeTransformExtents.allPositionsWithin)
            {
                MapEffectsManager.Instance.RemoveEffect(effect.Effect, coord);
            }
        }
    }

    public void ChangeMapPos(Vector2Int distanceMoved)
    {
        RectInt newExtents = GetComponent<GridTransform>().GetRect();
        RectInt oldExtents = newExtents;
        oldExtents.x -= distanceMoved.x;
        oldExtents.y -= distanceMoved.y;
        foreach (MapEffect effect in _mapEffects)
        {
            ModifyEffectRegistration(effect, oldExtents, false);
            ModifyEffectRegistration(effect, newExtents, true);
        }
    }

    public void TagEffect(MapEffectType effectType, Vector2Int cell)
    {       
        TagEffectEvent?.Invoke(effectType, cell);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffectComponent : MonoBehaviour
{
    [SerializeField]
    private List<MapEffect> _mapEffects;
    public List<MapEffect> MapEffects { get { return new List<MapEffect>(_mapEffects); } }

    private void OnEnable()
    {
        GetComponent<GridTransform>().OnChangeMapPos += ChangeMapPos;
        foreach (MapEffect effect in _mapEffects)
        {
            AddEffectRegistration(effect);  //problem--> how do i determine whether the effect is already registered?  could jsut do it in the manager...
        }
    }

    private void OnDisable()
    {
        GetComponent<GridTransform>().OnChangeMapPos -= ChangeMapPos;
        foreach (MapEffect effect in _mapEffects)
        {
            RemoveEffectRegistration(effect);
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
}

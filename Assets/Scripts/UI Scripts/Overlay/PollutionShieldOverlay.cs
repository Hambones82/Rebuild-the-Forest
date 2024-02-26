using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionShieldOverlay : MonoBehaviour
{
    [SerializeField]
    private BuildingComponentFueled _fueledComponent;
    [SerializeField]
    private GriddedSpriteOverlay _spriteOverlay;
    [SerializeField]
    private float _fullAlphaThreshold = 10;
    [SerializeField]
    private float minShieldAlpha;
    [SerializeField]
    private float maxShieldAlpha;
    [SerializeField]
    private VFXManager _vFXManager;
    [SerializeField]
    private VFXType _shieldHitVFXType;
    [SerializeField]
    private GridMap _gridMap;

    private void Awake()
    {
        _gridMap = GridMap.Current;
        _vFXManager = FindObjectOfType<VFXManager>();
        _fueledComponent = GetComponentInParent<BuildingComponentFueled>();        
        _spriteOverlay = GetComponent<GriddedSpriteOverlay>();
        _fueledComponent.EnableEffectEvent += OnShieldEnabled;
        _fueledComponent.DisableEffectEvent += OnShieldDisabled;
        _fueledComponent.FuelChangeEvent += OnStrengthChange;
        _fueledComponent.EffectTagEventOut += OnShieldHit;
        if(_fueledComponent.EffectIsEnabled)
        {
            OnShieldEnabled();
        }
        else
        {
            OnShieldDisabled();
        }
    }

    private void OnShieldHit(Vector2Int hitPosition)
    {
        _vFXManager.SpawnEffect(_shieldHitVFXType, _gridMap.MapToWorld(hitPosition));
    }

    Vector2Int GetShieldSize()
    {
        Vector2Int buildingSize = GetComponentInParent<GridTransform>().Size;
        int effectRange = _fueledComponent.GetRange();
        Vector2Int effectSize = new Vector2Int(effectRange * 2, effectRange * 2);
        Vector2Int shieldSize = effectSize + buildingSize;
        return shieldSize;
    }

    private void OnStrengthChange(float strength)
    {
        float newAlpha = Mathf.Lerp(minShieldAlpha, maxShieldAlpha, strength / _fullAlphaThreshold);
        _spriteOverlay.SetAlpha(newAlpha);
    }

    private void OnShieldEnabled()
    {        
        _spriteOverlay.SetSizeInCells(GetShieldSize());
        _spriteOverlay.EnableOverlay();
    }

    private void OnShieldDisabled()
    {        
        _spriteOverlay.DisableOverlay();
    }
}

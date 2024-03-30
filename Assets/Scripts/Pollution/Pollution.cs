using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class Pollution : MonoBehaviour
{
    [SerializeField]
    private PollutionManager pollutionManager;
    public PollutionManager PollutionManager { get => pollutionManager; set => pollutionManager = value; }

    public PollutionTypeController pTypeController;

    [SerializeField]
    private PollutionData _pollutionData;
    public PollutionData PollutionData { get => _pollutionData; }

    public int Priority { get => _pollutionData.Priority; }

    [SerializeField]
    private List<PollutionEffect> pollutionEffects;
    public IReadOnlyList<PollutionEffect> PollutionEffects { get => pollutionEffects; }

    [SerializeField]
    private float maxAmount = 100;
    public float MaxAmount { get => maxAmount; }
    [SerializeField]
    private float amount;
    public float Amount { get => amount; }

    [SerializeField]
    private List<MapEffectType> cleanEnableEffects;
    public List<MapEffectType> CleanEnableEffects { get => new List<MapEffectType>(cleanEnableEffects); }

    public UnityEvent OnDisableEvent;

    //just have a pollution.IsCleanable, use an effect...

    public bool IsCleanable(Vector2Int cell)
    {        
        foreach(var effect in pollutionEffects)
        {
            if(effect is IPollutionCleanable cleanableEffect)
            {
                if(!cleanableEffect.IsCleanable(cell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void OnClean()
    {
        foreach(var effect in pollutionEffects)
        {
            if(effect is IPollutionCleanable cleanableEffect)
            {
                cleanableEffect.OnClean(GetComponent<GridTransform>().topLeftPosMap); //COME BACK HERE!!!
            }
        }
    }

    public bool IsSpawnBlocked(Vector2Int cell)
    {        
        foreach(var effect in pollutionEffects)
        {
            if(effect is IPollutionSpawnBlock blocker)
            {
                if(blocker.BlocksPollutionGrowth(cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnSpawnBlocked(Vector2Int cell)
    {
        foreach(var effect in pollutionEffects)
        {
            if(effect is IPollutionSpawnBlock blocker)
            {
                blocker.OnSpawnBlocked(cell);
            }
        }
    }

    private void Awake()
    {        
        SetAmount(maxAmount);        
    }

    private void OnEnable()
    {
        foreach(PollutionEffect effect in pollutionEffects)
        {
            effect.OnSpawn(this);
        }
    }

    private void Update()
    {
    }

    //returns whether the pollution was removed
    public bool SetAmount(float amt)
    {
        amount = Mathf.Clamp(amt, 0, maxAmount);
        float scale = amt / maxAmount;
        transform.localScale = new Vector3(scale, scale);
        if (amount == 0)
        {
            OnClean();
            pollutionManager.RemovePollution(this);
            return true;
        }
        return false;
    }
    
    private void OnDisable()
    {
        if(OnDisableEvent != null)
        {
            OnDisableEvent.Invoke();
            OnDisableEvent.RemoveAllListeners();
        }
        foreach (PollutionEffect effect in pollutionEffects)
        {
            effect.OnDeath(this);
        }
    }
}

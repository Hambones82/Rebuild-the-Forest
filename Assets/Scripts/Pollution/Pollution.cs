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

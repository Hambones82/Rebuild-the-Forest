using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Pollution : MonoBehaviour
{
    [SerializeField]
    private PollutionManager pollutionManager;
    public PollutionManager PollutionManager { get => pollutionManager; set => pollutionManager = value; }

    [SerializeField]
    public List<PollutionEffect> pollutionEffects;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float maxAmount = 100;
    public float MaxAmount { get => maxAmount; }
    [SerializeField]
    private float amount;
    public float Amount { get => amount; }

    public UnityEvent OnDisableEvent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void SetAmount(float amt)
    {
        amount = Mathf.Clamp(amt, 0, maxAmount);
        float scale = amt / maxAmount;
        transform.localScale = new Vector3(scale, scale);
        if (amount == 0)
        {
            pollutionManager.RemovePollution(this);
        }
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

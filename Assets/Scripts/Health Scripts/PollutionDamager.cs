using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionDamager : MonoBehaviour
{
    private GridTransform gridTransform;
    private ActorUnitHealthComponent health;
    private Timer pollutionTickTimer;
    [SerializeField]
    float _tickPeriod;

    [SerializeField]
    float _pollutionVulnerability;

    private void Awake()
    {
        gridTransform = GetComponent<GridTransform>();
        health = GetComponent<ActorUnitHealthComponent>();
        pollutionTickTimer = new Timer(_tickPeriod);
        pollutionTickTimer.AddEvent(DoDamage);
        pollutionTickTimer.Enabled = true;
    }

    private void DoDamage()
    {
        if(PollutionManager.Instance.IsEffectAtCell(gridTransform.topLeftPosMap, PollutionManager.Instance.DamageEffect))
        {
            health.TakeDamage(PollutionManager.Instance.DamageEffect.DamageAmount);
        }
    }

    private void Update()
    {
        pollutionTickTimer.UpdateTimer();
    }
}

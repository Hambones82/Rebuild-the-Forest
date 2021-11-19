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
        if(gridTransform.AtLeastOneCellIsOccupiedBy(MapLayer.pollution))
        {
            health.TakeDamage(_pollutionVulnerability);
        }
    }

    private void Update()
    {
        pollutionTickTimer.UpdateTimer();
    }
}

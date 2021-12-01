using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthVisualizer : MonoBehaviour
{
    private TextMeshPro textComponent;
    private ActorUnitHealthComponent health;

    private TimerLite updateTimer;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        health = GetComponentInParent<ActorUnitHealthComponent>();
        updateTimer = new TimerLite(UITuning.refreshRate);
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void Update()
    {
        if(updateTimer.UpdateTimer(Time.deltaTime))
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        textComponent.text = $"{(int)health.Health}/{(int)health.MaxHealth}";
    }

    private void OnDisable()
    {
        updateTimer.Reset();
    }


}

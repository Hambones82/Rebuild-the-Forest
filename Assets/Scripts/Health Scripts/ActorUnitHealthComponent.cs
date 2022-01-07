using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorUnitHealthComponent : MonoBehaviour
{
    public event Action OnTakeDamage;

    [SerializeField]
    private float _minHealth;

    [SerializeField]
    private float _maxHealth;
    public float MaxHealth
    {
        get => _maxHealth;
    }

    [SerializeField]
    private float _health;
    public float Health
    {
        get => _health;
    }
    
    private void OnEnable()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        AdjustHealth(0 - damage);
        OnTakeDamage?.Invoke();
    }

    public void Heal(float healAmount)
    {
        AdjustHealth(healAmount);
    }

    private void AdjustHealth(float changeAmount)
    {
        _health += changeAmount;
        _health = Mathf.Clamp(_health, _minHealth, _maxHealth);
        if(_health == _minHealth)
        {
            ActorUnitManager.Instance.KillActorUnit(GetComponent<ActorUnit>());
        }
    }

    private void KillUnit()
    {

    }
}

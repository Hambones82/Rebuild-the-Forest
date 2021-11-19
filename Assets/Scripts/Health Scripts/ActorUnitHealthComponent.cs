using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorUnitHealthComponent : MonoBehaviour
{
    [SerializeField]
    private float _minHealth;
    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private float _health;
    public float Health
    {
        get => _health;
    }

    public void TakeDamage(float damage)
    {
        AdjustHealth(0 - damage);
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
